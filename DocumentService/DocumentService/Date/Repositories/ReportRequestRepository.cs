using DocumentService.Application.Interfaces;
using DocumentService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Date.Repositories
{
    public class ReportRequestRepository(
        DocumentDbContext _context,
        ILogger<ReportRequestRepository> _logger) : IReportRequestRepository
    {
        public async Task AddAsync(ReportRequest request, CancellationToken cancellation)
        {
            await _context.ReportRequests.AddAsync(request, cancellation);
            _logger.LogInformation("{DateTime}: Created report request with ID: {RequestId}", DateTime.UtcNow, request.Id);
        }

        public async Task<ReportRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellation)
        {
            return await _context.ReportRequests
                .Include(r => r.Documents) // Подтягиваем связанные DocumentId
                .FirstOrDefaultAsync(r => r.Id == requestId, cancellation);
        }

        public async Task SaveChangesAsync(CancellationToken cancellation)
        {
            await _context.SaveChangesAsync(cancellation);
        }
    }
}
