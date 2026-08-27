using DocumentService.Core.DTOs;

namespace DocumentService.Application.Interfaces
{
    public interface IDocumentReadRepository
    {
        Task<IEnumerable<DocumentDto>> GetActiveByUserIdAsync(Guid UserId, CancellationToken token);
        Task<DocumentDto?> GetByIdAsync(Guid documentId, CancellationToken token);
        Task<IEnumerable<DocumentDto>> GetTrashByUserIdAsync(Guid UserId, CancellationToken token);
        Task<IEnumerable<string>> GetS3KeysByDocumnetIdsAsync(IEnumerable<Guid> documnetIds, CancellationToken token);
    }
}