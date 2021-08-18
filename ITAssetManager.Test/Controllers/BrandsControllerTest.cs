using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Brands.Models;
using MyTested.AspNetCore.Mvc;
using Xunit;
using System.Linq;
using Shouldly;

using static ITAssetManager.Data.DataConstants;
using ITAssetManager.Web.Models.Brands;
using System.Collections.Generic;

namespace ITAssetManager.Test.Controllers
{
    public class BrandsControllerTest
    {
        [Fact]
        public void GetAddShouldReturnViewForAuthorizedUsers()
            => MyController<BrandsController>
                .Instance()
                .Calling(c => c.Add())
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View();

        [Fact]
        public void PostAddShouldBeAllowedForPostRequestOnlyAndAuthorizedUsers()
            => MyController<BrandsController>
                .Instance()
                .Calling(c => c.Add(With.Default<BrandAddFormServiceModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName)
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostAddShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<BrandsController>
                .Instance()
                .Calling(c => c.Add(With.Default<BrandAddFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<BrandAddFormServiceModel>()
                    .Passing(brand => brand.Name == null));

        [Theory]
        [InlineData("Test Brand")]
        public void PostAddShouldRedirectWithTempDataMessageAndShouldSaveBrandWithValidBrand(string name)
            => MyController<BrandsController>
                .Instance()
                .WithUser(user => user.InRole(AdministratorRoleName))
                .Calling(c => c.Add(new BrandAddFormServiceModel
                {
                    Name = name
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Brand>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(brand => brand.Name == name).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect(result => result
                    .To<BrandsController>(c => c.All(With.Empty<BrandsQueryModel>())));

        [Theory]
        [InlineData(1, "Test Brand")]
        public void AllShouldReturnCorrectBrandsForAuthorizedUsers(int id, string name)
            => MyController<BrandsController>
                .Instance()
                .WithData(new Brand
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.All(With.Default<BrandsQueryModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<BrandsQueryModel>()
                    .Passing(model =>
                    {
                        model.CurrentPage.ShouldBe(1);
                        model.Brands.Count().ShouldBe(1);
                        model.Brands.FirstOrDefault(brand => brand.Id == 1);
                    }));

        [Theory]
        [InlineData(1, "Test Brand")]
        public void GetEditShouldReturnViewWithCorrectBrandIfBrandExistsForAuthorizedUsers(int id, string name)
            => MyController<BrandsController>
                .Instance()
                .WithData(new Brand
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.Edit(id, null, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<BrandEditServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.Name.ShouldBe(name);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "Test Brand")]
        public void GetEditShouldRedirectToHomeErrorIfBrandDoesNotExist(int id, int wrongId, string name)
            => MyController<BrandsController>
                .Instance()
                .WithData(new Brand
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.Edit(wrongId, null, null, 1))
                .ShouldReturn()
                .Redirect(result => result
                    .To<HomeController>(c => c.Error()));

        [Fact]
        public void PostEditShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<BrandsController>
                .Calling(c => c.Edit(With.Default<BrandEditServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<BrandEditServiceModel>()
                    .Passing(s => s.Name == null));

        [Theory]
        [InlineData(1, "Old Name", "New Name")]
        public void PostEditShouldRedirectWithTempDataMessageAndShouldUpdateBrandWithValidBrandForAuthorizedUsers(int id, string oldName, string newName)
            => MyController<BrandsController>
                .Instance()
                .WithData(new Brand
                {
                    Id = id,
                    Name = oldName
                })
                .Calling(c => c.Edit(new BrandEditServiceModel
                {
                    Id = id,
                    Name = newName
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Brand>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(brand => brand.Name == newName).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, 2, "Test Brand")]
        public void DeleteShouldRedirectToHomeErrorIfBrandDoesNotExist(int id, int wrongId, string name)
           => MyController<BrandsController>
               .Instance()
               .WithData(new Brand
               {
                   Id = id,
                   Name = name
               })
               .Calling(c => c.Delete(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test Brand")]
        public void DeleteShouldRedirectToHomeErrorIfBrandIsInUse(int id, string name)
           => MyController<BrandsController>
               .Instance()
               .WithData(new Brand
               {
                   Id = id,
                   Name = name,
                   AssetModels = new List<AssetModel>
                   {
                       new AssetModel
                       {
                           Id = 1
                       }
                   }
               })
               .Calling(c => c.Delete(id, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test Brand")]
        public void DeleteShouldRedirectWithTempDataMessageAndShouldDeleteBrandForAuthorizedUsers(int id, string name)
            => MyController<BrandsController>
                .Instance()
                .WithData(new Brand
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.Delete(id, null, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Brand>(set =>
                    {
                        set.FirstOrDefault(brand => brand.Id == id).ShouldBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();
    }
}
