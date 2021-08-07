using ITAssetManager.Web.Areas.Admin.Services.Roles.Models;
using System.Collections.Generic;

namespace ITAssetManager.Web.Areas.UserRolesAdmin.Services.Roles
{
    public interface IRoleService
    {
        ICollection<RoleListingServiceModel> GetAll();
    }
}
