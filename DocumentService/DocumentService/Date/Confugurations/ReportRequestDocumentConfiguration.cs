using DocumentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentService.Date.Confugurations
{
    public class ReportRequestDocumentConfiguration : IEntityTypeConfiguration<ReportRequestDocument>
    {
        public void Configure(EntityTypeBuilder<ReportRequestDocument> builder)
        {
            builder.ToTable("report_request_documents");

            builder.HasKey(rd => new {rd.ReportRequestId, rd.DocumentId});

            builder.HasOne<Document>()
                .WithMany()
                .HasForeignKey(rd => rd.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
