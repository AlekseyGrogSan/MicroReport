using DocumentService.Core.Enums;

namespace DocumentService.Core.Entities
{
    public class ReportRequest
    {
        private readonly List<ReportRequestDocument> _documents = new();

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string TargetContentType { get; private set; } = null!;
        public ReportRequestStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<ReportRequestDocument> Documents => _documents.AsReadOnly();

        private ReportRequest() { }

        public static ReportRequest Create(Guid userId, string targetContentType, IEnumerable<Guid> documentIds)
        {
            var idsList = documentIds.Distinct().ToList();
            if (!idsList.Any())
                throw new ArgumentException("Запрос на отчет должен содержать хотя бы один документ.", nameof(documentIds));

            var request = new ReportRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TargetContentType = targetContentType,
                Status = ReportRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var docId in idsList)
            {
                request._documents.Add(new ReportRequestDocument(request.Id, docId));
            }

            return request;
        }

        public void MarkAsProcessing()
        {
            Status = ReportRequestStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            Status = ReportRequestStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            Status = ReportRequestStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
