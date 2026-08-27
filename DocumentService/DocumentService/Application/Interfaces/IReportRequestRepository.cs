using DocumentService.Core.Entities;

namespace DocumentService.Application.Interfaces
{
    public interface IReportRequestRepository
    {
        Task AddAsync(ReportRequest request, CancellationToken cancellation);
        Task<ReportRequest?> GetByIdAsync(Guid requestId, CancellationToken cancellation);
        Task SaveChangesAsync(CancellationToken cancellation);
    }
}