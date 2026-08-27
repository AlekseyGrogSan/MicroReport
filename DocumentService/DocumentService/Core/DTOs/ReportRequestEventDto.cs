namespace DocumentService.Core.DTOs
{
    public record ReportRequestEventDto(
        Guid RequestId,
        Guid UserId,
        string Prompt,
        List<string> DocumentS3Keys);
}
