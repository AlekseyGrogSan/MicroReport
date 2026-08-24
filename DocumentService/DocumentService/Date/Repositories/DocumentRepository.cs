using DocumentService.Core.Entities;
using DocumentService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Date.Repositories
{
    public class DocumentRepository(DocumentDbContext _context, ILogger<DocumentRepository> _logger) : IDocumentRepository
    {
        public async Task AddAsync(Document document, CancellationToken cancellation)
        {
            await _context.Documents.AddAsync(document, cancellation);
            _logger.LogInformation("{DateTime}; Add new document with ID: {document}", DateTime.UtcNow, document.Id);
        }

        public async Task<Document?> GetByIdAsync(Guid documentId, CancellationToken cancellation)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId, cancellation);

            if (document == null)
            {
                _logger.LogWarning("{DateTime}; NotFound a document with ID: {documentId}", DateTime.UtcNow, documentId);
                return null;
            }

            return document;
        }

        public async Task DeleteAsync(Guid documentId, CancellationToken cancellation)
        {
            await _context.Documents
                .Where(d => d.Id == documentId)
                .ExecuteDeleteAsync(cancellation);

            _logger.LogInformation("Deleted {documentId}", documentId);
        }

        public async Task SaveChangesAsync(CancellationToken token)
        {
            await _context.SaveChangesAsync(token);
        }
    }
}
