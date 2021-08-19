using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Categories.Models;
using MyTested.AspNetCore.Mvc;
using Xunit;
using Shouldly;
using System.Linq;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Categories;
using System.Collections.Generic;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class CategoriesControllerTest
    {
        [Fact]
        public void GetAddShouldReturnViewForAuthorizedUsers()
            => MyController<CategoriesController>
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
            => MyController<CategoriesController>
                .Instance()
                .Calling(c => c.Add(With.Default<CategoryAddFormServiceModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName)
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostAddShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<CategoriesController>
                .Instance()
                .Calling(c => c.Add(With.Default<CategoryAddFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<CategoryAddFormServiceModel>()
                    .Passing(category => category.Name == null));

        [Theory]
        [InlineData("Test Category")]
        public void PostAddShouldRedirectWithTempDataMessageAndShouldSaveCategoryWithValidCategory(string name)
            => MyController<CategoriesController>
                .Instance()
                .WithUser(user => user.InRole(AdministratorRoleName))
                .Calling(c => c.Add(new CategoryAddFormServiceModel
                {
                    Name = name
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Category>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(category => category.Name == name).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect(result => result
                    .To<CategoriesController>(c => c.All(With.Empty<CategoriesQueryModel>())));

        [Theory]
        [InlineData(1, "Test Category")]
        public void AllShouldReturnCorrectCategoriesForAuthorizedUsers(int id, string name)
            => MyController<CategoriesController>
                .Instance()
                .WithData(new Category
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.All(With.Default<CategoriesQueryModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<CategoriesQueryModel>()
                    .Passing(model =>
                    {
                        model.CurrentPage.ShouldBe(1);
                        model.Categories.Count().ShouldBe(1);
                        model.Categories.FirstOrDefault(category => category.Id == 1).ShouldNotBeNull();
                    }));

        [Theory]
        [InlineData(1, "Test Category")]
        public void GetEditShouldReturnViewWithCorrectCategoryIfCategoryExistsForAuthorizedUsers(int id, string name)
            => MyController<CategoriesController>
                .Instance()
                .WithData(new Category
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
                    .WithModelOfType<CategoryEditServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.Name.ShouldBe(name);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "Test Category")]
        public void GetEditShouldRedirectToHomeErrorIfCategoryDoesNotExist(int id, int wrongId, string name)
            => MyController<CategoriesController>
                .Instance()
                .WithData(new Category
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
            => MyController<CategoriesController>
                .Calling(c => c.Edit(With.Default<CategoryEditServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<CategoryEditServiceModel>()
                    .Passing(s => s.Name == null));

        [Theory]
        [InlineData(1, "Old Name", "New Name")]
        public void PostEditShouldRedirectWithTempDataMessageAndShouldUpdateCategoryWithValidCategoryForAuthorizedUsers(int id, string oldName, string newName)
            => MyController<CategoriesController>
                .Instance()
                .WithData(new Category
                {
                    Id = id,
                    Name = oldName
                })
                .Calling(c => c.Edit(new CategoryEditServiceModel
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
                    .WithSet<Category>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(category => category.Name == newName).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, 2, "Test Category")]
        public void DeleteShouldRedirectToHomeErrorIfCategoryDoesNotExist(int id, int wrongId, string name)
           => MyController<CategoriesController>
               .Instance()
               .WithData(new Category
               {
                   Id = id,
                   Name = name
               })
               .Calling(c => c.Delete(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test Category")]
        public void DeleteShouldRedirectToHomeErrorIfCategoryIsInUse(int id, string name)
           => MyController<CategoriesController>
               .Instance()
               .WithData(new Category
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
        [InlineData(1, "Test Category")]
        public void DeleteShouldRedirectWithTempDataMessageAndShouldDeleteCategoryForAuthorizedUsers(int id, string name)
            => MyController<CategoriesController>
                .Instance()
                .WithData(new Category
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
                    .WithSet<Category>(set =>
                    {
                        set.FirstOrDefault(category => category.Id == id).ShouldBeNull();
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
