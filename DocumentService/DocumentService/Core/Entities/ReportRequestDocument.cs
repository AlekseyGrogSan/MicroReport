namespace DocumentService.Core.Entities
{
    public class ReportRequestDocument
    {
        public Guid ReportRequestId { get; private set; }
        public Guid DocumentId { get; private set; }

        private ReportRequestDocument() { }

        public ReportRequestDocument(Guid reportRequestId, Guid documentId)
        {
            ReportRequestId = reportRequestId;
            DocumentId = documentId;
        }
    }
}
