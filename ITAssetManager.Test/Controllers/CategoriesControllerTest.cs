using ITAssetManager.Web.Controllers;
using MyTested.AspNetCore.Mvc;
using Xunit;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class CategoriesControllerTest
    {
        [Fact]
        public void ControllerShouldBeAccessibleForAuthorizedRequestsOnly()
            => MyController<CategoriesController>
                .Instance()
                .ShouldHave()
                .Attributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());

        [Fact]
        public void GetAddShouldReturnCorrectView()
            => MyController<CategoriesController>
                .Instance()
                .WithUser(null, new string[] { AdministratorRoleName })
                .Calling(c => c.Add())
                .ShouldReturn()
                .View();
    }
}
