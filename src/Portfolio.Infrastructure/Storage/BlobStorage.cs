using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Portfolio.Application.Abstractions.Clock;
using Portfolio.Application.Abstractions.Storage;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Storage;

/// <summary>
/// Provides functionality for interacting with Azure Blob Storage, including uploading, deleting, 
/// and generating pre-signed URLs for blobs. This class is a sealed implementation of the <see cref="IBlobStorage"/> interface.
/// </summary>
/// <remarks>
/// This class utilizes <see cref="BlobServiceClient"/> for communication with Azure Blob Storage, 
/// and relies on <see cref="BlobOptions"/> for configuration settings. It also uses <see cref="IDateTimeProvider"/> 
/// to handle time-sensitive operations, such as generating SAS tokens with expiration times.
/// </remarks>
internal sealed class BlobStorage : IBlobStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobOptions _blobOptions;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BlobStorage(BlobServiceClient blobServiceClient, IOptions<BlobOptions> options, IDateTimeProvider dateTimeProvider)
    {
        _blobServiceClient = blobServiceClient;
        _blobOptions = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Deletes a blob from the Azure Blob Storage container.
    /// </summary>
    /// <param name="fileName">The name of the blob to be deleted.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the success or failure of the delete operation.
    /// </returns>
    /// <remarks>
    /// This method attempts to delete the specified blob from the container configured in <see cref="BlobOptions"/>.
    /// If the blob does not exist, the operation will succeed without throwing an exception.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="fileName"/> is <see langword="null"/> or empty.
    /// </exception>
    /// <example>
    /// <code>
    /// var result = await blobStorage.DeleteAsync("example.txt", cancellationToken);
    /// if (result.IsSuccess)
    /// {
    ///     Console.WriteLine("Blob deleted successfully.");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Failed to delete blob: {result.Error.Message}");
    /// }
    /// </code>
    /// </example>
    public async Task<Result> DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.BlobName);

        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Generates a pre-signed URL for accessing a blob in Azure Blob Storage with read permissions.
    /// </summary>
    /// <param name="fileName">The name of the blob for which the pre-signed URL is to be generated.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the pre-signed URL as a string if the operation succeeds,
    /// or an error if the operation fails.
    /// </returns>
    /// <remarks>
    /// The generated URL includes a Shared Access Signature (SAS) token with read permissions and an expiration time
    /// defined by the <see cref="BlobOptions.ExpiresInMinute"/> configuration.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the blob container or blob client cannot be accessed or if the SAS URI generation fails.
    /// </exception>
    public async Task<Result<string>> GetPreSignedUrlAsync(string fileName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.BlobName);

        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        Uri uri = blobClient.GenerateSasUri(BlobSasPermissions.Read, _dateTimeProvider.UtcNow.AddMinutes(_blobOptions.ExpiresInMinute));

        await Task.CompletedTask;

        return uri.ToString();
    }

    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> containing the file data to upload.</param>
    /// <param name="contentType">The MIME type of the file being uploaded.</param>
    /// <param name="fileName">The name of the file to be stored in the blob container.</param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete. 
    /// Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the success or failure of the upload operation.
    /// </returns>
    /// <remarks>
    /// This method ensures that the target blob container exists before uploading the file. 
    /// If the container does not exist, it will be created. The uploaded file will be stored 
    /// with the specified content type.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="stream"/>, <paramref name="contentType"/>, or <paramref name="fileName"/> is null.
    /// </exception>
    /// <exception cref="Azure.RequestFailedException">
    /// Thrown if an error occurs during the upload process to Azure Blob Storage.
    /// </exception>
    public async Task<Result> UploadAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.BlobName);

        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}
