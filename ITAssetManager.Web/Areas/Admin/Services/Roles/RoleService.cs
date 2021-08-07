using ITAssetManager.Data;
using ITAssetManager.Web.Areas.Admin.Services.Roles.Models;
using System.Collections.Generic;
using System.Linq;

namespace ITAssetManager.Web.Areas.UserRolesAdmin.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext data;

        public RoleService(AppDbContext data)
        {
            this.data = data;
        }

        public ICollection<RoleListingServiceModel> GetAll()
        {
            var roles = this.data
                .Roles
                .Select(r => new RoleListingServiceModel
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToList();

            return roles;
        }
    }
}
