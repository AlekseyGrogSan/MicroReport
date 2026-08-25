using DocumentService.Core.Entities;

namespace DocumentService.Application.Interfaces
{
    public interface IDocumentRepository
    {
        Task AddAsync(Document document, CancellationToken cancellation);
        Task DeleteAsync(Guid documentId, CancellationToken cancellation);
        Task<Document?> GetByIdAsync(Guid documentId, CancellationToken cancellation);
        Task SaveChangesAsync(CancellationToken token);
    }
}