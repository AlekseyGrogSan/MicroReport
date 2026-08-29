namespace DocumentService.Core.DTOs
{
    public record ReportCompletedEventDto(
        Guid RequestId,
        Guid UserId,
        string ReportName,
        string ContentType,
        string S3Key
        );
}
