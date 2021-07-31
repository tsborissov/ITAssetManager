using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ITAssetManager.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserAsset> UserAssets { get; init; } = new HashSet<UserAsset>();
    }
}
