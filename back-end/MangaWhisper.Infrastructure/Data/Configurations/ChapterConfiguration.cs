using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Infrastructure.Data.Configurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("Chapters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MangaId)
            .IsRequired();

        builder.Property(c => c.Number)
            .IsRequired();

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.PublishedDate)
            .IsRequired();

        builder.HasOne(c => c.Manga)
            .WithMany(m => m.Chapters)
            .HasForeignKey(c => c.MangaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.MangaId, c.Number })
            .IsUnique();
    }
}
