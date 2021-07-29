using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var services = scopedServices.ServiceProvider;

            MigrateDatabase(services);

            SeedAdministrator(services);

            SeedBrands(services);
            SeedCategories(services);
            SeedStatuses(services);
            SeedVendors(services);
            SeedModels(services);

            // TODO : Seed database

            return app;
        }

        private static void MigrateDatabase(IServiceProvider services)
        {
            var data = services.GetRequiredService<AppDbContext>();

            data.Database.Migrate();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                    if (await roleManager.RoleExistsAsync(AdministratorRoleName))
                    {
                        return;
                    }

                    var role = new IdentityRole { Name = AdministratorRoleName };

                    await roleManager.CreateAsync(role);

                    const string adminEmail = "admin@email.com";
                    const string adminPassword = "Admin135.";

                    var user = new ApplicationUser
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                    };

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, role.Name);
                })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedBrands(IServiceProvider services)
        {
            var data = services.GetRequiredService<AppDbContext>();

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
                new Brand { Name = "‎ClearClick" },
            });

            data.SaveChanges();
        }

        private static void SeedCategories(IServiceProvider services)
        {
            var data = services.GetRequiredService<AppDbContext>();

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
                new Category { Name = "Monitor" },
                new Category { Name = "Other" },
            });

            data.SaveChanges();
        }

        private static void SeedStatuses(IServiceProvider services)
        {
            var data = services.GetRequiredService<AppDbContext>();

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

        private static void SeedVendors(IServiceProvider services)
        {
            var data = services.GetRequiredService<AppDbContext>();

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

        private static void SeedModels(IServiceProvider services)
        {
            var data = services.GetRequiredService<AppDbContext>();

            if (data.AssetModels.Any())
            {
                return;
            }

            data.AssetModels.AddRange(new[]
            {
                new AssetModel
                {
                    BrandId = data.Brands.Where(b => b.Name == "HP").Select(b => b.Id).FirstOrDefault(),
                    Name = "EliteBook x360 1030 G2",
                    CategoryId = data.Categories.Where(c => c.Name == "Laptop").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://support.hp.com/doc-images/891/c05356084.png",
                    Details = "Intel Core i5-7300U 2.6 GHz (up to 3.5 GHz) with Intel Turbo Boost technology 3 MB cache 2 cores Intel HD Graphics 620"
                },
                new AssetModel 
                {
                    BrandId = data.Brands.Where(b => b.Name == "HP").Select(b => b.Id).FirstOrDefault(),
                    Name = "Elite x2 G8 Tablet PC",
                    CategoryId = data.Categories.Where(c => c.Name == "Laptop").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://www.hp.com/us-en/shop/app/assets/images/product/28R54AV_MB/left_facing.png?_=1618567009228&imwidth=270&imdensity=1",
                    Details = "11th Generation Intel® Core™ i5 processor 8 GB memory; 256 GB NVMe SSD 13.0\" WUXGA+ (1920x1280) Touchscreen display Intel® Iris® Xe Graphics"
                },
                new AssetModel
                {
                    BrandId = data.Brands.Where(b => b.Name == "HP").Select(b => b.Id).FirstOrDefault(),
                    Name = "EliteBook x360 1040 G8",
                    CategoryId = data.Categories.Where(c => c.Name == "Laptop").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://ssl-product-images.www8-hp.com/digmedialib/prodimg/lowres/c06971923.png?imwidth=278&imdensity=1",
                    Details = "11th Generation Intel® Core™ i5 processor 16 GB memory; 256 GB SSD storage 14\" diagonal FHD touch display Intel® Iris® Xᵉ Graphics"
                },
                new AssetModel
                {
                    BrandId = data.Brands.Where(b => b.Name == "HP").Select(b => b.Id).FirstOrDefault(),
                    Name = "EliteDisplay E243 23.8-inch",
                    CategoryId = data.Categories.Where(c => c.Name == "Monitor").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://support.hp.com/doc-images/725/c05804491.jpg",
                    Details = "Display panel type 23.8 inches IPS, Micro edge LED backlight Viewable image area (diagonal) 60.45 cm (23.8 in) widescreen Panel active area (W x H) 52.70 x 29.64 cm (20.75 x 11.67 in)"
                },
                new AssetModel
                {
                    BrandId = data.Brands.Where(b => b.Name == "Lenovo").Select(b => b.Id).FirstOrDefault(),
                    Name = "ThinkPad T490",
                    CategoryId = data.Categories.Where(c => c.Name == "Laptop").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://www.lenovo.com/medias/ThinkPad-T490.png?context=bWFzdGVyfHJvb3R8ODI0NjR8aW1hZ2UvcG5nfGg3MC9oNzAvMTExMjYyNjE5MDc0ODYucG5nfGJlZWM1NjcyMDI4MWQ5OWQ3NjQxMWVlMDliMzcxMjg1MGQ1ZjQ1Nzc1ZDhiZmIxZjBjNTBjZTA3NmI2YTJiNDY",
                    Details = "10th Generation Intel® Core™ i7-10510U Processor (4 Cores / 8 Threads, 1.80 GHz, up to 4.90 GHz with Turbo Boost) 8 GB DDR4 512 GB M.2 2280 SSD"
                },
                new AssetModel
                {
                    BrandId = data.Brands.Where(b => b.Name == "ClearClick").Select(b => b.Id).FirstOrDefault(),
                    Name = "VR42 Vintage Retro Style Wooden Radio",
                    CategoryId = data.Categories.Where(c => c.Name == "Other").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://cdn.shopify.com/s/files/1/1222/4226/products/01_large.jpg?v=1536605498",
                    Details = "Hardware Interface: ‎Bluetooth Special Features: ‎Radio Speaker Connectivity: ‎Wireless Audio Wattage: ‎10 Watts"
                },
                new AssetModel
                {
                    BrandId = data.Brands.Where(b => b.Name == "Apple").Select(b => b.Id).FirstOrDefault(),
                    Name = "iPad Pro 12.9 (2021)",
                    CategoryId = data.Categories.Where(c => c.Name == "Tablet").Select(c => c.Id).FirstOrDefault(),
                    ImageUrl = "https://i.insider.com/607f39f174da0300181e2d14?width=1136&format=jpeg",
                    Details = "PLATFORM OS iPadOS 14.5.1 upgradable to iPadOS 14.6, Chipset Apple M1 CPU, Octa-core GPU Apple GPU (8-core graphics), Internal 128GB 8GB RAM, 256GB 8GB RAM, 512GB 8GB RAM, 1TB 16GB"
                }
            });
        }
    }
}