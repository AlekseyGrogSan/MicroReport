namespace DocumentService.Core.Entities
{
    public class Report
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public Guid UserId { get; private set; }
        public string ReportName { get; private set; } = null!;
        public string ContentType { get; private set; } = null!;
        public string S3Key { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        private Report() { }

        public static Report Create(Guid requestId, Guid userId, string reportName, string contentType, string s3Key)
        {
            return new Report
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                UserId = userId,
                ReportName = reportName,
                ContentType = contentType,
                S3Key = s3Key,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
