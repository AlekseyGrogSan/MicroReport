namespace DocumentService.Core.DTOs
{
    public record ReportDto(
        Guid Id,
        Guid RequestId,
        Guid UserId,
        string ReportName,
        string ContentType,
        string S3Key,
        DateTime CreatedAt
    );
}