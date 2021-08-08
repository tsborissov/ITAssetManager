using System.Collections.Generic;

namespace ITAssetManager.Web.Areas.Admin.Services.Users.Models
{
    public class UserListingServiceModel
    {
        public string Id { get; init; }

        public string UserName { get; init; }

        public bool IsAdmin { get; set; }
    }
}
