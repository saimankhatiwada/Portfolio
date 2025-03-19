using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Portfolio.Application.Abstractions.Clock;
using Portfolio.Application.Abstractions.Storage;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Infrastructure.Storage;

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

    public async Task<Result> DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.BlobName);

        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result<string>> GetPreSignedUrlAsync(string fileName, CancellationToken cancellationToken = default)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobOptions.BlobName);

        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

        Uri uri = blobClient.GenerateSasUri(BlobSasPermissions.Read, _dateTimeProvider.UtcNow.AddMinutes(_blobOptions.ExpiresInMinute));

        await Task.CompletedTask;

        return uri.ToString();
    }

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
