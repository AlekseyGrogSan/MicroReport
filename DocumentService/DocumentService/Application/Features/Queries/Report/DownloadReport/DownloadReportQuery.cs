using MediatR;
using DocumentService.Core.DTOs;

namespace DocumentService.Application.Features.Queries.Report.DownloadReport
{
    public record DownloadReportQuery(
        Guid id
        ) : IRequest<DownloadfResult>;
}
