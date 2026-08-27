using DocumentService.Core.DTOs;

namespace DocumentService.Application.Interfaces
{
    public interface IReportReadRepository
    {
        Task<ReportDto?> GetByIdAsync(Guid reportId, CancellationToken token);
        Task<string?> GetRequestStatusAsync(Guid requestId, CancellationToken token);
        Task<IEnumerable<ReportDto>> GetUserReportsAsync(Guid userId, CancellationToken token);
    }
}