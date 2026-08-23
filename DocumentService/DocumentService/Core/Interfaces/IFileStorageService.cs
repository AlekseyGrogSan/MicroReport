namespace DocumentService.Core.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, Guid Id, CancellationToken cancellation);
        Task<Stream> DownloadFileAsync(string S3Key, CancellationToken cancellation);
        Task DeleteFileAsync(string S3Key, CancellationToken cancellation);
    }
}
