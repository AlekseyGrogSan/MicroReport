using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Metadata;
using DocumentService.Core.Entities;
using DocumentService.Core.ValueObjects;

namespace DocumentService.Date.Confugurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<DocumentService.Core.Entities.Document>
    {
        public void Configure(EntityTypeBuilder<DocumentService.Core.Entities.Document> builder)
        {
            builder.ToTable("document");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.UserId)
                .IsRequired();

            builder.Property(d => d.FileName)
                .HasConversion(
                    name => name.Value,
                    value => DocumentName.Create(value)
                )
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(d => d.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.SizeBytes)
                .IsRequired();

            builder.Property(d => d.S3Key)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.HasIndex(d => d.UserId);
        }
    }
}
