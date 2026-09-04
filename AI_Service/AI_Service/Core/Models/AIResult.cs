namespace AI_Service.Core.Models
{
    public record AIResult(
        string GeneratedContent,
        int PromptTokens,
        int CompletionTokens,
        TimeSpan Duration
    );
}
