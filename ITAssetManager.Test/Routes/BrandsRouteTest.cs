using MyTested.AspNetCore.Mvc;
using Xunit;
using Shouldly;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Brands.Models;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Routes
{
    public class BrandsRouteTest
    {
        [Fact]
        public void GetAddShouldBeRoutedCorrectly()
            => MyRouting
                .Configuration()
                .ShouldMap("/Brands/Add/")
                .To<BrandsController>(c => c.Add());

        [Fact]
        public void PostAddShouldBeRoutedCorrectly()
            => MyRouting
                .Configuration()
                .ShouldMap(req => req
                    .WithLocation("/Brands/Add")
                    .WithMethod(HttpMethod.Post)
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName })
                    .WithAntiForgeryToken())
                .To<BrandsController>(c => c.Add(With.Any<BrandAddFormServiceModel>()));
    }
}
