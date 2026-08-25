using DocumentService.Core.Enums;

namespace DocumentService.Core.DTOs
{
    public record DocumentDto(
        Guid Id,
        Guid UserId,
        string FileName,
        string ContentType,
        long SizeBytes,
        string S3Key,
        DocumentStatus Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
