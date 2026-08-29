using MediatR;

namespace DocumentService.Application.Features.Queries.ReportRequest.GetStatus
{
    public record GetStatusQuery(Guid RequestId) : IRequest<string>;
}
