using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Storage;

/// <summary>
/// Defines the contract for a blob storage service, providing methods for uploading, retrieving, 
/// and deleting files in a storage system.
/// </summary>
/// <remarks>
/// This interface abstracts the operations for interacting with a blob storage system, 
/// enabling the management of files through methods such as uploading streams, 
/// generating pre-signed URLs for access, and deleting files.
/// </remarks>
public interface IBlobStorage
{
    /// <summary>
    /// Uploads a file to the blob storage.
    /// </summary>
    /// <param name="stream">The stream containing the file data to upload.</param>
    /// <param name="contentType">The MIME type of the file being uploaded.</param>
    /// <param name="fileName">The name of the file to be stored in the blob storage.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the success or failure of the upload operation.
    /// </returns>
    /// <remarks>
    /// This method uploads the provided file stream to the blob storage with the specified content type and file name.
    /// It ensures that the target blob container exists before performing the upload.
    /// </remarks>
    Task<Result> UploadAsync(Stream stream, string contentType, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a pre-signed URL for accessing a blob in the storage system.
    /// </summary>
    /// <param name="fileName">The name of the file for which the pre-signed URL is generated.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a 
    /// <see cref="Result{TValue}"/> with the pre-signed URL as a string if the operation succeeds.
    /// </returns>
    /// <remarks>
    /// The pre-signed URL allows temporary access to the specified blob without requiring authentication.
    /// This method is typically used for scenarios where secure, time-limited access to a file is needed.
    /// </remarks>
    Task<Result<string>> GetPreSignedUrlAsync(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the blob storage.
    /// </summary>
    /// <param name="fileName">The name of the file to be deleted from the blob storage.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the success or failure of the delete operation.
    /// </returns>
    /// <remarks>
    /// This method attempts to delete the specified file from the blob storage. 
    /// If the file does not exist, the operation will still succeed without throwing an error.
    /// </remarks>
    Task<Result> DeleteAsync(string fileName, CancellationToken cancellationToken = default);
}
