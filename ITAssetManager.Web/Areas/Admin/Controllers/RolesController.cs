using ITAssetManager.Web.Areas.UserRolesAdmin.Services.Roles;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetManager.Web.Areas.Admin.Controllers
{
    public class RolesController : AdminController
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public IActionResult All()
        {
            var roles = this.roleService.GetAll();

            return View(roles);
        }
    }
}
