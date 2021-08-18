using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Categories.Models;
using MyTested.AspNetCore.Mvc;
using Shouldly;
using System.Linq;
using Xunit;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Pipeline
{
    public class CategoriesPipelineTest
    {
        [Fact]
        public void GetAddShouldShouldReturnView()
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithLocation("/Categories/Add")
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName }))
                .To<CategoriesController>(c => c.Add())
                .Which()
                .ShouldReturn()
                .View();

        [Theory]
        [InlineData("Test Category")]
        public void PostAddShouldSaveCategoryHaveValidModelStateAndRedirectToAll(string name)
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithMethod(HttpMethod.Post)
                    .WithLocation("/Categories/Add")
                    .WithFormFields(new
                    {
                        Name = name,
                    })
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName })
                    .WithAntiForgeryToken())
                .To<CategoriesController>(c => c.Add(new CategoryAddFormServiceModel
                {
                    Name = name,
                }))
                .Which()
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Category>(set =>
                    {
                        set.ShouldNotBeEmpty();
                        set.SingleOrDefault(b => b.Name == name).ShouldNotBeNull();
                    }))
                .AndAlso()
                    .ShouldHave()
                    .TempData(tempData => tempData
                        .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .To<CategoriesController>(c => c.All(null)));

        [Theory]
        [InlineData(1, null, null, 1, "Test Category")]
        public void GetEditShouldReturnViewWithCorrectModel(int id, string searchString, string sortOrder, int currentPage, string name)
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithLocation($"/Categories/Edit/{id}?currentPage={currentPage}")
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName }))
                .To<CategoriesController>(c => c.Edit(id, searchString, sortOrder, currentPage))
                .Which(controller => controller
                    .WithData(new Category { Id = id, Name = name }))
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<CategoryEditServiceModel>()
                    .Passing(c =>
                    {
                        c.Id.ShouldBe(id);
                        c.Name.ShouldBe(name);
                    }));

        [Theory]
        [InlineData(1, null, null, 1, 2, "Test Category")]
        public void GetEditShouldRedirectToErrorViewWhenCategoryNotExisting(int id, string searchString, string sortOrder, int currentPage, int wrongCategoryId, string name)
            => MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithLocation($"/Categories/Edit/{wrongCategoryId}?currentPage={currentPage}")
                    .WithUser(AdministratorUsername, new string[] { AdministratorRoleName }))
                .To<CategoriesController>(c => c.Edit(wrongCategoryId, searchString, sortOrder, currentPage))
                .Which(controller => controller
                    .WithData(new Category { Id = id, Name = name }))
                .ShouldReturn()
                .Redirect(redirect => redirect
                    .To<HomeController>(c => c.Error()));
    }
}
