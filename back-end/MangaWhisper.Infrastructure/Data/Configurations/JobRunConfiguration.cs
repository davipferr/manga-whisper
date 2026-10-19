using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Infrastructure.Data.Configurations;

public class JobRunConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.ToTable("JobRuns");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Type)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.RequestedAt)
            .IsRequired();

        builder.Property(r => r.ErrorMessage)
            .HasMaxLength(1000);

        builder.HasOne(r => r.MangaChecker)
            .WithMany()
            .HasForeignKey(r => r.MangaCheckerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ScrapingJob)
            .WithMany()
            .HasForeignKey(r => r.ScrapingJobId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
