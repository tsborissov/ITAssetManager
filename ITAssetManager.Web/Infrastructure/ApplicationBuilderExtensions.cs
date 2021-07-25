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

            var data = scopedServices.ServiceProvider.GetService<Data.AppDbContext>();

            data.Database.Migrate();

            SeedBrands(data);
            SeedCategories(data);
            SeedStatuses(data);
            SeedVendors(data);

            // TODO : Seed database

            return app;
        }

        private static void SeedBrands(Data.AppDbContext data)
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

        private static void SeedCategories(Data.AppDbContext data)
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

        private static void SeedStatuses(Data.AppDbContext data)
        {
            if (data.Statuses.Any())
            {
                return;
            }

            data.Statuses.AddRange(new[]
            {
                new Status { Name = "In Use" },
                new Status { Name = "In Stock" },
                new Status { Name = "Disposed" },
                new Status { Name = "Pending Disposal" },
                new Status { Name = "In Repair" },
                new Status { Name = "Staging" },
            });

            data.SaveChanges();
        }

        private static void SeedVendors(Data.AppDbContext data)
        {
            if (data.Vendors.Any())
            {
                return;
            }

            data.Vendors.AddRange(new[]
            {
                new Vendor { Name = "HP Bulgaria EOOD", Vat = "BG000123444", Email = "hp@email.com", Telephone = "0035929991122", Address = "Business Park, Sofia, Bulgaria" },
                new Vendor { Name = "Lenovo Bulgaria OOD", Vat = "BG000123445", Email = "lenovo@email.com", Telephone = "0035929992233", Address = "LenovoStreet 8, Sofia, Bulgaria" },
                new Vendor { Name = "Dell Bulgaria OOD", Vat = "BG000123455", Email = "dell@email.com", Telephone = "0035929993344", Address = "Tech Park, Sofia, Bulgaria" },
                new Vendor { Name = "Apple Bulgaria EOOD", Vat = "BG000123466", Email = "apple@email.com", Telephone = "0035929994321", Address = "Apple Park, Sofia, Bulgaria" },
                new Vendor { Name = "Cisco AD", Vat = "BG000555444", Email = "cisco@email.com", Telephone = "0035929998888", Address = "Business Park, Sofia, Bulgaria" },
                new Vendor { Name = "Acer OOD", Vat = "BG000123555", Email = "acer@email.com", Telephone = "0035923334455", Address = "Business Park, Sofia, Bulgaria" },
            });

            data.SaveChanges();
        }
    }
}