using ITAssetManager.Web.Areas.Admin.Services.Users;
using ITAssetManager.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ITAssetManager.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminController
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService) 
            => this.userService = userService;

        public async Task<IActionResult> All()
        {
            var currentUserId = User.Id();

            var users = await this.userService.GetAllExcept(currentUserId);

            return View(users);
        }

        public async Task<IActionResult> AddAdmin(string id)
        {
            if (!ValidUser(id).Result)
            {
                return RedirectToAction("Error", "Home");
            }

            await userService.AddAdmin(id);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> RemoveAdmin(string id)
        {
            if (!ValidUser(id).Result)
            {
                return RedirectToAction("Error", "Home");
            }

            await userService.RemoveAdmin(id);

            return RedirectToAction(nameof(All));
        }

        private async Task<bool> ValidUser(string userId)
        {
            var validUsers = await this.userService.GetAllExcept(User.Id());

            return validUsers.Any(u => u.Id == userId);
        }
    }
}
