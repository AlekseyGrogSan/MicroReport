using AI_Service.Core.Interfaces;
using AI_Service.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;

namespace AI_Service.Infrastructure
{
    public class AIService(
        Kernel _kernel
        ) : IAIService
    {
        public async Task<AIResult> GenerateAIReport(RequestModel request, string aggregatedContext, CancellationToken token)
        {
            var stopWatch = Stopwatch.StartNew();

            var chatCompletition = _kernel.GetRequiredService<IChatCompletionService>();

            var promt = $"""
                Ты — аналитический ассистент. Создай структурированный отчет на основе предоставленного контекста документов.

                Требование пользователя:
                {request.Prompt}

                Контекст документов:
                {aggregatedContext}
                """;

            var response = await chatCompletition.GetChatMessageContentAsync(promt, cancellationToken: token);

            stopWatch.Stop();

            return new AIResult(
                GeneratedContent: response.Content ?? string.Empty,
                PromptTokens: 0, // При необходимости вытаскивается из response.Metadata
                CompletionTokens: 0,
                Duration: stopWatch.Elapsed
            );
        }
    }
}
