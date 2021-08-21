using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ITAssetManager.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserAsset> UserAssets { get; init; } = new HashSet<UserAsset>();

        public ICollection<Request> SubmittedRequests { get; init; } = new HashSet<Request>();

        public ICollection<Request> ReviewedRequests { get; init; } = new HashSet<Request>();
    }
}
