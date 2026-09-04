using AI_Service.Core.Interfaces;
using AI_Service.Core.Settings;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Options;
using System.Text;

namespace AI_Service.Infrastructure
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly S3Settings _s3Settings;

        public S3Service(IAmazonS3 amazonS3, IOptions<S3Settings> s3Settings)
        {
            _amazonS3 = amazonS3;
            _s3Settings = s3Settings.Value;
        }

        public async Task<string> UploadAsync(Stream fileStream, string filename, string contentType, Guid fileId, CancellationToken cancellationToken)
        {
            string s3Key = $"{DateTime.UtcNow:yyyy/MM}/{fileId}_{filename}";

            var request = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = s3Key,
                InputStream = fileStream,
                ContentType = contentType,
                AutoCloseStream = false
            };

            await _amazonS3.PutObjectAsync(request, cancellationToken).ConfigureAwait(false);

            return s3Key;
        }

        public async Task<Stream> DownloadFileAsync(string s3Key, CancellationToken token)
        {
            var downloadRequest = new Amazon.S3.Model.GetObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = s3Key
            };

            var fileStream = await _amazonS3.GetObjectAsync(downloadRequest, token);

            return fileStream.ResponseStream;
        }
    }
}