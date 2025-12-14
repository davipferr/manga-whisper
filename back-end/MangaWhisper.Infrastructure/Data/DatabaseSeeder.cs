using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MangaWhisper.Common.Enums;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedRolesAsync(roleManager);
        await SeedAdminUserAsync(userManager);
        await SeedMangaAsync(dbContext);
        await SeedMangaCheckerAsync(dbContext);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        if (string.IsNullOrEmpty(adminPassword) || string.IsNullOrEmpty(adminEmail))
        {
            throw new InvalidOperationException(
                "ADMIN_PASSWORD or ADMIN_EMAIL environment variable is not set. " +
                "Please configure them in Azure App Service Configuration or local environment.");
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrator",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {errors}");
            }
        }
    }

    private static async Task SeedMangaAsync(ApplicationDbContext dbContext)
    {
        const string onePieceTitle = "One Piece";

        var existingManga = await dbContext.Mangas
            .FirstOrDefaultAsync(m => m.Title == onePieceTitle);

        if (existingManga == null)
        {
            var manga = new Manga
            {
                Title = onePieceTitle,
                CoverImageUrl = "",
                Status = MangaStatus.Ongoing,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.Mangas.Add(manga);
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedMangaCheckerAsync(ApplicationDbContext dbContext)
    {
        const string siteIdentifier = "mugiwara-oficial";

        var existingChecker = await dbContext.MangaCheckers
            .FirstOrDefaultAsync(mc => mc.SiteIdentifier == siteIdentifier);

        if (existingChecker == null)
        {
            var onePieceManga = await dbContext.Mangas
                .FirstOrDefaultAsync(m => m.Title == "One Piece");

            if (onePieceManga != null)
            {
                var mangaChecker = new MangaChecker
                {
                    MangaId = onePieceManga.Id,
                    LastKnownChapter = 0,
                    CheckIntervalMinutes = 1,
                    IsActive = true,
                    CheckerStatus = MangaCheckerStatus.Idle,
                    CreatedAt = DateTime.UtcNow,
                    SiteIdentifier = siteIdentifier
                };

                dbContext.MangaCheckers.Add(mangaChecker);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
