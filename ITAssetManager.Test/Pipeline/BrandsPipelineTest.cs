using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Brands.Models;
using MyTested.AspNetCore.Mvc;
using Shouldly;
using System.Linq;
using Xunit;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Pipeline
{
    public class BrandsPipelineTest
    {
        [Fact]
        public void GetAddShouldShouldReturnView()
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithLocation("/Brands/Add")
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName }))
                .To<BrandsController>(c => c.Add())
                .Which()
                .ShouldReturn()
                .View();

        [Theory]
        [InlineData("Test Brand")]
        public void PostAddShouldSaveBrandHaveValidModelStateAndRedirectToAll(string brandName)
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Brands/Add")
                    .WithFormFields(new
                    {
                        Name = brandName,
                    })
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName })
                    .WithAntiForgeryToken())
                .To<BrandsController>(c => c.Add(new BrandAddFormServiceModel
                {
                    Name = brandName,
                }))
                .Which()
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Brand>(set =>
                    {
                        set.ShouldNotBeEmpty();
                        set.SingleOrDefault(b => b.Name == brandName).ShouldNotBeNull();
                    }))
                .AndAlso()
                    .ShouldHave()
                    .TempData(tempData => tempData
                        .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .To<BrandsController>(c => c.All(null)));

        [Theory]
        [InlineData(1, null, null, 1, "Test Brand")]
        public void GetEditShouldReturnViewWithCorrectModel(int id, string searchString, string sortOrder, int currentPage, string brandName)
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithLocation($"/Brands/Edit/{id}?currentPage={currentPage}")
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName }))
                .To<BrandsController>(c => c.Edit(id, searchString, sortOrder, currentPage))
                .Which(controller => controller
                    .WithData(new Brand { Id = id, Name = brandName} ))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<BrandEditServiceModel>()
                    .Passing(brand =>
                    {
                        brand.Id.ShouldBe(id);
                        brand.Name.ShouldBe(brandName);
                    }));

        [Theory]
        [InlineData(1, null, null, 1, 2, "Test Brand")]
        public void GetEditShouldRedirectToErrorViewWhenBrandNotExisting(int id, string searchString, string sortOrder, int currentPage, int wrongBrandId, string brandName)
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithLocation($"/Brands/Edit/{wrongBrandId}?currentPage={currentPage}")
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName }))
                .To<BrandsController>(c => c.Edit(wrongBrandId, searchString, sortOrder, currentPage))
                .Which(controller => controller
                    .WithData(new Brand { Id = id, Name = brandName }))
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .To<HomeController>(c => c.Error()));
    }
}
