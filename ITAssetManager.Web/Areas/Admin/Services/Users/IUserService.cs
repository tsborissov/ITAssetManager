using ITAssetManager.Web.Areas.Admin.Services.Users.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITAssetManager.Web.Areas.Admin.Services.Users
{
    public interface IUserService
    {
        Task<ICollection<UserListingServiceModel>> GetAllExcept(string id);

        Task<IdentityResult> AddAdmin(string userId);

        Task<IdentityResult> RemoveAdmin(string userId);
    }
}
