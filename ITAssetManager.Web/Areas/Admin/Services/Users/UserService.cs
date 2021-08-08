using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Areas.Admin.Services.Users.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Areas.Admin.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext data;
        private readonly UserManager<ApplicationUser> userManager;

        public UserService(AppDbContext data, UserManager<ApplicationUser> userManager)
        {
            this.data = data;
            this.userManager = userManager;
        }

        public async Task<ICollection<UserListingServiceModel>> GetAllExcept(string id)
        {
            var users = this.data
                   .Users
                   .Where(u => u.Id != id)
                   .OrderBy(u => u.UserName)
                   .Select(u => new UserListingServiceModel
                   {
                       Id = u.Id,
                       UserName = u.UserName
                   })
                   .ToList();

            foreach (var user in users)
            {
                var targetUser = await userManager.FindByIdAsync(user.Id);

                user.IsAdmin = await userManager.IsInRoleAsync(targetUser, AdministratorRoleName);
            }

            return users;
        }

        public async Task<IdentityResult> AddAdmin(string userId)
        {
            var targetUser = await userManager.FindByIdAsync(userId);

            return await userManager.AddToRoleAsync(targetUser, AdministratorRoleName);
        }

        public async Task<IdentityResult> RemoveAdmin(string userId)
        {
            var targetUser = await userManager.FindByIdAsync(userId);

            return await userManager.RemoveFromRoleAsync(targetUser, AdministratorRoleName);
        }
    }
}
