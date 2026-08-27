namespace DocumentService.Core.DTOs
{
    public record ReportDto(
        Guid Id,
        Guid ReportRequestId,
        Guid UserId,
        string ReportName,
        string S3Key,
        long SizeBytes,
        DateTime CreatedAt
    );
}