using System.Diagnostics.Contracts;

namespace DocumentService.Core.Settings
{
    public class S3Settings
    {
        public const string SectionName = "S3Settings";

        public string Endpoint { get; set; } = string.Empty;
        public string AccessKey {  get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }
}
