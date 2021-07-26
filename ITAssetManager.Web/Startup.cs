using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.AssetModels;
using ITAssetManager.Web.Services.Assets;
using ITAssetManager.Web.Services.Brands;
using ITAssetManager.Web.Services.Categories;
using ITAssetManager.Web.Services.Statuses;
using ITAssetManager.Web.Services.Vendors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ITAssetManager.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<Data.AppDbContext>(options => options
                    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services
                .AddDatabaseDeveloperPageExceptionFilter();

            services
                .AddDefaultIdentity<ApplicationUser>(options => 
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<Data.AppDbContext>();

            services.AddControllersWithViews();

            services
                .AddTransient<IBrandService, BrandService>()
                .AddTransient<ICategoryService, CategoryService>()
                .AddTransient<IStatusService, StatusService>()
                .AddTransient<IVendorService, VendorService>()
                .AddTransient<IAssetModelService, AssetModelService>()
                .AddTransient<IAssetService, AssetService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.PrepareDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages();
                });
        }
    }
}
