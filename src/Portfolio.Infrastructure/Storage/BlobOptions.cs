namespace Portfolio.Infrastructure.Storage;

public sealed class BlobOptions
{
    public const string Blob = "Blob";
    public string BlobName { get; init; }
    public int ExpiresInMinute { get; init; }
}
