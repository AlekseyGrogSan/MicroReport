using AI_Service.Core.Models;

namespace AI_Service.Core.Interfaces
{
    public interface IReportExporterService
    {
        Task<string> ExportReportAsync(AIResult result, Guid requstId, string format, CancellationToken token);
    }
}