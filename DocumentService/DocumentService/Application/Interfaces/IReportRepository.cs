using DocumentService.Core.Entities;

namespace DocumentService.Application.Interfaces
{
    public interface IReportRepository
    {
        Task AddReportAsync(Report report, CancellationToken token);
        Task<Report?> GetByIdAsync(Guid id, CancellationToken token);
        Task SaveChangesAsync(CancellationToken token);
    }
}