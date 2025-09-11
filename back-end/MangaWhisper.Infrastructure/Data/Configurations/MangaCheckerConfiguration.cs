using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Infrastructure.Data.Configurations;

public class MangaCheckerConfiguration : IEntityTypeConfiguration<MangaChecker>
{
    public void Configure(EntityTypeBuilder<MangaChecker> builder)
    {
        builder.ToTable("MangaCheckers");

        builder.HasKey(mc => mc.Id);

        builder.Property(mc => mc.MangaId)
            .IsRequired();

        builder.Property(mc => mc.LastKnownChapter)
            .IsRequired();

        builder.Property(mc => mc.CheckIntervalMinutes)
            .IsRequired();

        builder.Property(mc => mc.IsActive)
            .IsRequired();

        builder.Property(mc => mc.CheckerStatus)
            .IsRequired();

        builder.Property(mc => mc.CreatedAt)
            .IsRequired();

        builder.Property(mc => mc.SiteIdentifier)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(mc => mc.Manga)
            .WithMany(m => m.MangaChecker)
            .HasForeignKey(mc => mc.MangaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
