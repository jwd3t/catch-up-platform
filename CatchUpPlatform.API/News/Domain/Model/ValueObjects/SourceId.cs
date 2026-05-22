namespace CatchUpPlatform.API.News.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a validated source identifier.
/// </summary>
public sealed record SourceId
{
    private const int MaxLength = 255;

    public SourceId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SourceId cannot be null, empty, or whitespace.", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"SourceId cannot be longer than {MaxLength} characters.", nameof(value));

        Value = value;
    }

    /// <summary>
    ///     Gets the underlying primitive value.
    /// </summary>
    public string Value { get; }

    public override string ToString() => Value;
}

