using CatchUpPlatform.API.News.Domain.Model.Aggregates;
using CatchUpPlatform.API.News.Domain.Model.Commands;

namespace CatchUpPlatform.API.News.Domain.Services;

/// <summary>
///     Command service interface for favorite source operations.
/// </summary>
/// <remarks>
///     This interface defines the contract for handling favorite source creation commands.
///     It ensures duplicate detection and persistence of new favorite sources.
/// </remarks>
public interface IFavoriteSourceCommandService
{
    /// <summary>
    ///     Handle the create favorite source command.
    /// </summary>
    /// <remarks>
    ///     This method handles the create favorite source command. It checks if the favorite source already exists for the
    ///     given NewsApiKey and SourceId.
    ///     If it does not exist, it creates a new favorite source and adds it to the database.
    /// </remarks>
    /// <param name="command">CreateFavoriteSourceCommand command</param>
    /// <returns>
    ///     The created FavoriteSource object, or null if a duplicate pair (NewsApiKey, SourceId) is detected.
    /// </returns>
    /// <exception cref="Exception">Thrown if persistence fails while handling the command.</exception>
    Task<FavoriteSource?> Handle(CreateFavoriteSourceCommand command, CancellationToken cancellationToken = default);
}