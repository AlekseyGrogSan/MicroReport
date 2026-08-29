namespace DocumentService.Core.DTOs
{
    public record ReportDto(
        Guid Id,
        Guid ReportRequestId,
        Guid UserId,
        string ReportName,
        string ContentType,
        string S3Key,
        DateTime CreatedAt
    );
}