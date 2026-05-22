using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.Commands;
using CatchUpPlatform.API.News.Domain.Repositories;
using CatchUpPlatform.API.News.Domain.Services;
using CatchUpPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatchUpPlatform.API.News.Application.Internal.CommandServices;

/// <summary>
///     Application service for handling favorite source creation commands.
/// </summary>
/// <remarks>
///     This service acts as the command handler for creating new favorite sources.
///     It enforces duplicate detection through both application-level checks and database constraints,
///     then coordinates with the unit of work to persist changes.
///     Logs warnings for duplicate detections and errors for persistence failures.
/// </remarks>
/// <param name="favoriteSourceRepository">Repository for accessing favorite source data.</param>
/// <param name="unitOfWork">Unit of work for managing transaction scope.</param>
/// <param name="logger">Logger for diagnostic and error reporting.</param>
/// See
/// <see cref="IFavoriteSourceRepository">IFavoriteSourceRepository</see>
/// ,
/// <see cref="IUnitOfWork">IUnitOfWork</see>
public class FavoriteSourceCommandService(
    IFavoriteSourceRepository favoriteSourceRepository,
    IUnitOfWork unitOfWork,
    ILogger<FavoriteSourceCommandService> logger)
    : IFavoriteSourceCommandService
{
    /// <inheritdoc />
    public async Task<FavoriteSource?> Handle(CreateFavoriteSourceCommand command,
        CancellationToken cancellationToken = default)
    {
        var favoriteSource =
            await favoriteSourceRepository.FindByNewsApiKeyAndSourceIdAsync(command.NewsApiKey, command.SourceId,
                cancellationToken);
        if (favoriteSource != null)
        {
            logger.LogWarning(
                "Duplicate favorite source rejected for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey,
                command.SourceId);
            return null;
        }

        favoriteSource = new FavoriteSource(command);
        try
        {
            await favoriteSourceRepository.AddAsync(favoriteSource, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex,
                "Database update failed creating favorite source for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey,
                command.SourceId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating favorite source for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
                command.NewsApiKey,
                command.SourceId);
            throw;
        }

        return favoriteSource;
    }
}