using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Infrastructure.Data.Configurations;

public class ScrapingJobConfiguration : IEntityTypeConfiguration<ScrapingJob>
{
    public void Configure(EntityTypeBuilder<ScrapingJob> builder)
    {
        builder.ToTable("ScrapingJobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Type)
            .IsRequired();

        builder.Property(j => j.CronExpression)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.TimeZone)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(j => j.IsEnabled)
            .IsRequired();

        builder.Property(j => j.CreatedAt)
            .IsRequired();

        builder.HasOne(j => j.MangaChecker)
            .WithMany()
            .HasForeignKey(j => j.MangaCheckerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
