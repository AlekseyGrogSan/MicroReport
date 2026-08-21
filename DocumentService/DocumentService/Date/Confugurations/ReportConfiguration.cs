using Microsoft.EntityFrameworkCore;
using DocumentService.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentService.Date.Confugurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("report");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReportName)
            .HasMaxLength(255)
            .IsRequired();

            builder.Property(r => r.S3Key)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.RequestId);
        }
    }
}
