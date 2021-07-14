using ITAssetManager.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITAssetManager.Data
{
    public class ItAssetManagerDbContext : IdentityDbContext
    {
        public ItAssetManagerDbContext(DbContextOptions<ItAssetManagerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; init; }

        public DbSet<AssetModel> AssetModels { get; init; }

        public DbSet<Brand> Brands { get; init; }

        public DbSet<Category> Categories { get; init; }

        public DbSet<Status> Statuses { get; init; }

        public DbSet<Vendor> Vendors { get; init; }
    }
}
