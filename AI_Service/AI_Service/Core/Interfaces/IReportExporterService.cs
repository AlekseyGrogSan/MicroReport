using AI_Service.Core.Models;

namespace AI_Service.Core.Interfaces
{
    public interface IReportExporterService
    {
        Task<(string s3key, string filename, string format)> ExportReportAsync(AIResult result, Guid requstId, string format, CancellationToken token);
    }
}