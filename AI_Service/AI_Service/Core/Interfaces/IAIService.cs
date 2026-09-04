using AI_Service.Core.Models;

namespace AI_Service.Core.Interfaces
{
    public interface IAIService
    {
        Task<AIResult> GenerateAIReport(RequestModel request, string aggregatedContext, CancellationToken token);
    }
}