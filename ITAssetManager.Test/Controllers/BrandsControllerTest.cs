using ITAssetManager.Web.Controllers;
using MyTested.AspNetCore.Mvc;
using Xunit;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class BrandsControllerTest
    {
        [Fact]
        public void ControllerShouldBeAccessibleForAuthorizedRequestsOnly()
            => MyController<BrandsController>
                .Instance()
                .ShouldHave()
                .Attributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());

        [Fact]
        public void GetAddShouldReturnCorrectView()
            => MyController<BrandsController>
                .Instance()
                .WithUser(null, new string[] { AdministratorRoleName })
                .Calling(c => c.Add())
                .ShouldReturn()
                .View();
    }
}
