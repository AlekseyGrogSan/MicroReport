using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using MediatR;

namespace DocumentService.Application.Features.Queries.Report.GetAllReports
{
    public class GetAllReportsHandler(
        IReportReadRepository _repository
        ) : IRequestHandler<GetAllReportQuery, IEnumerable<ReportDto>>
    {
        public async Task<IEnumerable<ReportDto>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetUserReportsAsync(request.UserID, cancellationToken);
        }
    }
}
