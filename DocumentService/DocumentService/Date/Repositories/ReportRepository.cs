using DocumentService.Application.Interfaces;
using DocumentService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Date.Repositories
{
    public class ReportRepository(
        DocumentDbContext _context,
        ILogger<ReportRepository> _logger) : IReportRepository
    {
        public async Task AddReportAsync(Report report, CancellationToken token)
        {
            await _context.Reports.AddAsync(report, token);
            _logger.LogInformation("{DateTime}: Created report with ID: {RequestId}", DateTime.UtcNow, report.Id);
        }

        public async Task<Report?> GetByIdAsync(Guid id, CancellationToken token)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                _logger.LogWarning("{DateTime}; NotFound a report with ID: {documentId}", DateTime.UtcNow, id);
            }

            return report;
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await _context.SaveChangesAsync(token);
        }
    }
}
