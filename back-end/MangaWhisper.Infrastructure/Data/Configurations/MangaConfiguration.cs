using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Infrastructure.Data.Configurations;

public class MangaConfiguration : IEntityTypeConfiguration<Manga>
{
    public void Configure(EntityTypeBuilder<Manga> builder)
    {
        builder.ToTable("Mangas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .IsRequired();

        builder.HasMany(m => m.MangaChecker)
            .WithOne(mc => mc.Manga)
            .HasForeignKey(mc => mc.MangaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Chapters)
            .WithOne(c => c.Manga)
            .HasForeignKey(c => c.MangaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
