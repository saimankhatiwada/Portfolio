using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Storage;

public interface IBlobStorage
{
    Task<Result> UploadAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken = default);
    Task<Result<string>> GetPreSignedUrlAsync(string fileName, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string fileName, CancellationToken cancellationToken = default);
}
