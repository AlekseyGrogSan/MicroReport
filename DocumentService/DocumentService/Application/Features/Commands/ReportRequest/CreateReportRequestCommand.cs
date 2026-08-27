using MediatR;

namespace DocumentService.Application.Features.Commands.ReportRequest
{
    public record CreateReportRequestCommand(
        Guid UserId,
        string UserPromt,
        string TargetContentType,
        IEnumerable<Guid> documentsIds
        ) : IRequest<Guid>;
}
