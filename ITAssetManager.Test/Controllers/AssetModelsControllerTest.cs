using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.AssetModels.Models;
using MyTested.AspNetCore.Mvc;
using Xunit;
using System.Linq;
using Shouldly;
using System.Collections.Generic;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class AssetModelsControllerTest
    {
        [Fact]
        public void GetAddShouldReturnViewForAuthorizedUsers()
            => MyController<AssetModelsController>
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
            => MyController<AssetModelsController>
                .Instance()
                .Calling(c => c.Add(With.Default<AssetModelsAddFormServiceModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName)
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostAddShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<AssetModelsController>
                .Instance()
                .Calling(c => c.Add(With.Default<AssetModelsAddFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetModelsAddFormServiceModel>()
                    .Passing(am => am.Name == null));

        [Theory]
        [InlineData("Test AssetModel")]
        public void PostAddShouldRedirectWithTempDataMessageAndShouldSaveAssetModelWithValidAssetModel(string name)
            => MyController<AssetModelsController>
                .Instance()
                .WithData(
                    new Brand { Id = 1},
                    new Category { Id = 1})
                .WithUser(user => user.InRole(AdministratorRoleName))
                .Calling(c => c.Add(new AssetModelsAddFormServiceModel
                {
                    Name = name,
                    BrandId = 1,
                    CategoryId = 1,
                    Details = "Test Details",
                    ImageUrl = "https://www.lotus-qa.com/wp-content/uploads/2020/02/testing.jpg"
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<AssetModel>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(am => am.Name == name).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, "Test AssetModel")]
        public void AllShouldReturnCorrectAssetModelsForAuthenticatedUsers(int id, string name)
            => MyController<AssetModelsController>
                .Instance()
                .WithData(new AssetModel
                {
                    Id = id,
                    Name = name,
                    BrandId = 1,
                    Brand = new Brand { Id = 1 },
                    CategoryId = 1,
                    Category = new Category { Id = 1 },
                    Details = "Test Details",
                    ImageUrl = "https://www.lotus-qa.com/wp-content/uploads/2020/02/testing.jpg"
                })
                .Calling(c => c.All(null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetModelQueryServiceModel>()
                    .Passing(model =>
                    {
                        model.AssetModels.Count().ShouldBe(1);
                        model.AssetModels.FirstOrDefault(am => am.Id == 1).ShouldNotBeNull();
                    }));

        [Theory]
        [InlineData(1, "Test AssetModel")]
        public void GetEditShouldReturnViewWithCorrectAssetModelIfAssetModelExistsForAuthorizedUsers(int id, string name)
            => MyController<AssetModelsController>
                .Instance()
                .WithData(new AssetModel
                {
                    Id = id,
                    Name = name,
                    BrandId = 1,
                    Brand = new Brand { Id = 1 },
                    CategoryId = 1,
                    Category = new Category { Id = 1 },
                    Details = "Test Details",
                    ImageUrl = "https://www.lotus-qa.com/wp-content/uploads/2020/02/testing.jpg"
                })
                .Calling(c => c.Edit(id, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetModelEditFormServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.Name.ShouldBe(name);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "Test AssetModel")]
        public void GetEditShouldRedirectToHomeErrorIfAssetModelDoesNotExist(int id, int wrongId, string name)
            => MyController<AssetModelsController>
                .Instance()
                .WithData(new AssetModel
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.Edit(wrongId, null, 1))
                .ShouldReturn()
                .Redirect(result => result
                    .To<HomeController>(c => c.Error()));

        [Fact]
        public void PostEditShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<AssetModelsController>
                .Calling(c => c.Edit(With.Default<AssetModelEditFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetModelEditFormServiceModel>()
                    .Passing(s => s.Name == null));

        [Theory]
        [InlineData(1, "Old Name", "New Name")]
        public void PostEditShouldRedirectWithTempDataMessageAndShouldUpdateAssetModelWithValidAssetModelForAuthorizedUsers(int id, string oldName, string newName)
            => MyController<AssetModelsController>
                .Instance()
                .WithData(new AssetModel
                {
                    Id = id,
                    Name = oldName,
                    BrandId = 1,
                    Brand = new Brand { Id = 1 },
                    CategoryId = 1,
                    Category = new Category { Id = 1 },
                    Details = "Test Details",
                    ImageUrl = "https://www.lotus-qa.com/wp-content/uploads/2020/02/testing.jpg"
                })
                .Calling(c => c.Edit(new AssetModelEditFormServiceModel
                {
                    Id = id,
                    Name = newName,
                    BrandId = 1,
                    CategoryId = 1,
                    Details = "Other Details",
                    ImageUrl = "https://www.lotus-qa.com/wp-content/uploads/2020/02/testing.jpg"
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
                    .WithSet<AssetModel>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(am => am.Name == oldName).ShouldBeNull();
                        set.FirstOrDefault(am => am.Name == newName).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, 2, "Test AssetModel")]
        public void DeleteShouldRedirectToHomeErrorIfAssetModelDoesNotExist(int id, int wrongId, string name)
           => MyController<AssetModelsController>
               .Instance()
               .WithData(new AssetModel
               {
                   Id = id,
                   Name = name
               })
               .Calling(c => c.Delete(wrongId))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test AssetModel")]
        public void DeleteShouldRedirectToHomeErrorIfAssetModelIsInUse(int id, string name)
           => MyController<AssetModelsController>
               .Instance()
               .WithData(new AssetModel
               {
                   Id = id,
                   Name = name,
                   Assets = new List<Asset>
                   {
                       new Asset
                       {
                           Id = 1
                       }
                   }
               })
               .Calling(c => c.Delete(id))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test AssetModel")]
        public void DeleteShouldRedirectWithTempDataMessageAndShouldDeleteAssetModelForAuthorizedUsers(int id, string name)
            => MyController<AssetModelsController>
                .Instance()
                .WithData(new AssetModel
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.Delete(id))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<AssetModel>(set =>
                    {
                        set.FirstOrDefault(am => am.Id == id).ShouldBeNull();
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
