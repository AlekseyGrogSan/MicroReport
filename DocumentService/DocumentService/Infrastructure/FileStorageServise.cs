using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using DocumentService.Core.Settings;
using DocumentService.Application.Interfaces;

namespace DocumentService.Infrastructure
{
    public class FileStorageServise : IFileStorageService
    {
        private readonly IAmazonS3 _amazonClient;
        private readonly S3Settings _s3Settings;

        public FileStorageServise(IAmazonS3 client, IOptions<S3Settings> settings) {
            
            _amazonClient = client;
            _s3Settings = settings.Value;
        }

        public async Task DeleteFileAsync(string S3Key, CancellationToken cancellation)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = S3Key
            };

            await _amazonClient.DeleteObjectAsync(deleteRequest, cancellation);
        }

        public async Task<Stream> DownloadFileAsync(string S3Key, CancellationToken cancellation)
        {
            var downloadRequest = new GetObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = S3Key
            };

            var response = await _amazonClient.GetObjectAsync(downloadRequest, cancellation);

            return response.ResponseStream;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, Guid Id, CancellationToken cancellation)
        {
            string s3Key = $"{DateTime.UtcNow:yyyy/MM}/{Id}_{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = s3Key,
                InputStream = fileStream,
                ContentType = contentType,
                AutoCloseStream = false
            };

            await _amazonClient.PutObjectAsync(request, cancellation);

            return s3Key;
        }
    }
}
