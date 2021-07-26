using ITAssetManager.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITAssetManager.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; init; }

        public DbSet<AssetModel> AssetModels { get; init; }

        public DbSet<Brand> Brands { get; init; }

        public DbSet<Category> Categories { get; init; }

        public DbSet<Status> Statuses { get; init; }

        public DbSet<Vendor> Vendors { get; init; }

        public DbSet<UserAsset> UsersAssets { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Asset>()
                .HasOne(b => b.AssetModel)
                .WithMany(b => b.Assets)
                .HasForeignKey(b => b.AssetModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Asset>()
                .HasOne(s => s.Status)
                .WithMany(s => s.Assets)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Asset>()
                .HasOne(v => v.Vendor)
                .WithMany(v => v.Assets)
                .HasForeignKey(v => v.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<AssetModel>()
                .HasOne(b => b.Brand)
                .WithMany(b => b.AssetModels)
                .HasForeignKey(b => b.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<AssetModel>()
                .HasOne(c => c.Category)
                .WithMany(c => c.AssetModels)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<UserAsset>()
                .HasKey(k => new { k.AssetId, k.UserId });

            base.OnModelCreating(builder);
        }
    }
}
