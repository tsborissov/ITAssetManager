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


    }
}
