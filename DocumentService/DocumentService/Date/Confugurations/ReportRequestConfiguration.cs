using DocumentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentService.Date.Confugurations
{
    public class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> builder)
        {
            builder.ToTable("report_request");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
            .IsRequired();

            builder.Property(r => r.TargetContentType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasMany(r => r.Documents)
                .WithOne()
                .HasForeignKey(rd => rd.ReportRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(r => r.Documents)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
