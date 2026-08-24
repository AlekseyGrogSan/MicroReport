namespace DocumentService.Core.DTOs
{
    public record DocumentDto(
        Guid Id,
        Guid UserId,
        string FileName,
        string ContentType,
        long SizeBytes,
        string S3Key,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
