namespace AI_Service.Core.Models
{
    public record RequestModel
    (
        Guid RequestId,
        Guid UserId,
        string Prompt,
        List<string> DocumentS3Keys
    );
}
