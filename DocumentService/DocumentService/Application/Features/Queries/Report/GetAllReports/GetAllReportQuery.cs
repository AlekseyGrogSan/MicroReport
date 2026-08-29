using DocumentService.Core.DTOs;
using MediatR;

namespace DocumentService.Application.Features.Queries.Report.GetAllReports
{
    public record GetAllReportQuery(Guid UserID) : IRequest<IEnumerable<ReportDto>>;
}
