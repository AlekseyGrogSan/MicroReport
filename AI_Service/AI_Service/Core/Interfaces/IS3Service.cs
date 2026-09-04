namespace AI_Service.Core.Interfaces
{
    public interface IS3Service
    {
        Task<string> UploadAsync(Stream fileStream, string filename, string contentType, Guid fileId, CancellationToken cancellationToken);
        Task<Stream> DownloadFileAsync(string s3Key, CancellationToken token);
    }
}