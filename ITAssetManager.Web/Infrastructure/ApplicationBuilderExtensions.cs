using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ITAssetManager.Web.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var data = scopedServices.ServiceProvider.GetService<Data.ItAssetManagerDbContext>();

            data.Database.Migrate();

            SeedBrands(data);
            SeedCategories(data);

            // TODO : Seed database

            return app;
        }

        private static void SeedBrands(ItAssetManagerDbContext data)
        {
            if (data.Brands.Any())
            {
                return;
            }

            data.Brands.AddRange(new[]
            {
                new Brand { Name = "HP" },
                new Brand { Name = "Dell" },
                new Brand { Name = "IBM" },
                new Brand { Name = "Lenovo" },
                new Brand { Name = "Cisco" },
                new Brand { Name = "Acer" },
                new Brand { Name = "Brother" },
                new Brand { Name = "Asus" },
                new Brand { Name = "Apple" },
                new Brand { Name = "Microsoft" },
                new Brand { Name = "Google" },
            });

            data.SaveChanges();
        }

        private static void SeedCategories(ItAssetManagerDbContext data)
        {
            if (data.Categories.Any())
            {
                return;
            }

            data.Categories.AddRange(new[]
            {
                new Category { Name = "Laptop" },
                new Category { Name = "Desktop" },
                new Category { Name = "Tablet" },
                new Category { Name = "Server" },
                new Category { Name = "Printer" },
                new Category { Name = "Projector" },
                new Category { Name = "Switch" },
                new Category { Name = "Router" },
            });

            data.SaveChanges();
        }
    }
}
