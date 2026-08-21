using DocumentService.Core.Enums;
using DocumentService.Core.ValueObjects;

namespace DocumentService.Core.Entities
{
    public class Document
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public DocumentName FileName { get; private set; } = null!;
        public string ContentType { get; private set; } = null!;
        public long SizeBytes { get; private set; }
        public string S3Key { get; private set; } = null!;
        public DocumentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Document() { }

        public static Document Create(Guid userId, string fileName, string contentType, long sizeBytes, string s3Key)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId не может быть пустым.", nameof(userId));

            if (sizeBytes <= 0)
                throw new ArgumentException("Размер файла должен быть больше 0.", nameof(sizeBytes));

            return new Document
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = DocumentName.Create(fileName),
                ContentType = contentType,
                SizeBytes = sizeBytes,
                S3Key = s3Key,
                Status = DocumentStatus.Uploaded,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsDeleted()
        {
            Status = DocumentStatus.Deleted;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
