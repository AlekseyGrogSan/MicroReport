using AI_Service.Core.Interfaces;
using AI_Service.Core.Models;
using Markdig;
using System.Drawing;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace AI_Service.Infrastructure
{
    public class ReportExporterService(IS3Service _storage) : IReportExporterService
    {
        public async Task<string> ExportReportAsync(AIResult result, Guid requstId, string format, CancellationToken token)
        {
            string filename = $"reports/{DateTime.UtcNow:yyyy/MM}/{requstId}_{format.ToLower()}";
            string contentType = format.ToLower() switch
            {
                "pdf" => "application/pdf",
                "html" => "text/html",
                _ => "text/markdown"
            };
            var context = Encoding.UTF8.GetBytes(result.GeneratedContent);
            using var stream = new MemoryStream(context);

            return await _storage.UploadAsync(stream, filename, contentType, requstId, token);
        }
    }
}
