namespace Portfolio.Infrastructure.Storage;

/// <summary>
/// Represents the configuration options for interacting with blob storage.
/// </summary>
/// <remarks>
/// This class encapsulates settings required for blob storage operations, such as the name of the blob container
/// and the expiration time for generated URLs. It is typically used in conjunction with dependency injection
/// to provide configuration values.
/// </remarks>
public sealed class BlobOptions
{
    public const string Blob = "Blob";
    public string BlobName { get; init; }
    public int ExpiresInMinute { get; init; }
}
