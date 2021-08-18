using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Statuses.Models;
using MyTested.AspNetCore.Mvc;
using Shouldly;
using Xunit;
using System.Linq;
using ITAssetManager.Web.Models.Statuses;
using System.Collections.Generic;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class StatusesControllerTest
    {
        [Fact]
        public void GetAddShouldReturnViewForAuthorizedUsers()
            => MyController<StatusesController>
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
            => MyController<StatusesController>
                .Instance()
                .Calling(c => c.Add(With.Default<StatusAddFormServiceModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName)
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostAddShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<StatusesController>
                .Instance()
                .Calling(c => c.Add(With.Default<StatusAddFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<StatusAddFormServiceModel>()
                    .Passing(status => status.Name == null));

        [Theory]
        [InlineData("Test Status")]
        public void PostAddShouldRedirectWithTempDataMessageAndShouldSaveStatusWithValidStatus(string name)
            => MyController<StatusesController>
                .Instance()
                .WithUser(user => user.InRole(AdministratorRoleName))
                .Calling(c => c.Add(new StatusAddFormServiceModel
                {
                    Name = name
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Status>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(status => status.Name == name).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect(result => result
                    .To<StatusesController>(c => c.All(With.Empty<StatusesQueryModel>())));

        [Theory]
        [InlineData(1, "Test Status")]
        public void AllShouldReturnCorrectStatusesForAuthorizedUsers(int id, string name)
            => MyController<StatusesController>
                .Instance()
                .WithData(new Status
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.All(With.Default<StatusesQueryModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<StatusesQueryModel>()
                    .Passing(model =>
                    {
                        model.CurrentPage.ShouldBe(1);
                        model.Statuses.Count().ShouldBe(1);
                        model.Statuses.FirstOrDefault(status => status.Id == 1);
                    }));

        [Theory]
        [InlineData(1, "Test Status")]
        public void GetEditShouldReturnViewWithCorrectStatusIfStatusExistsForAuthorizedUsers(int id, string name)
            => MyController<StatusesController>
                .Instance()
                .WithData(new Status
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
                    .WithModelOfType<StatusEditServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.Name.ShouldBe(name);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "Test Status")]
        public void GetEditShouldRedirectToHomeErrorIfStatusDoesNotExist(int id, int wrongId, string name)
            => MyController<StatusesController>
                .Instance()
                .WithData(new Status
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
            => MyController<StatusesController>
                .Calling(c => c.Edit(With.Default<StatusEditServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<StatusEditServiceModel>()
                    .Passing(s => s.Name == null));

        [Theory]
        [InlineData(1, "Old Name", "New Name")]
        public void PostEditShouldRedirectWithTempDataMessageAndShouldUpdateStatusWithValidStatusForAuthorizedUsers(int id, string oldName, string newName)
            => MyController<StatusesController>
                .Instance()
                .WithData(new Status
                {
                    Id = id,
                    Name = oldName
                })
                .Calling(c => c.Edit(new StatusEditServiceModel
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
                    .WithSet<Status>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(status => status.Name == newName).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, 2, "Test Status")]
        public void DeleteShouldRedirectToHomeErrorIfStatusDoesNotExist(int id, int wrongId, string name)
           => MyController<StatusesController>
               .Instance()
               .WithData(new Status
               {
                   Id = id,
                   Name = name
               })
               .Calling(c => c.Delete(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test Status")]
        public void DeleteShouldRedirectToHomeErrorIfStatusIsInUse(int id, string name)
           => MyController<StatusesController>
               .Instance()
               .WithData(new Status
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
               .Calling(c => c.Delete(id, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test Status")]
        public void DeleteShouldRedirectWithTempDataMessageAndShouldDeleteStatusForAuthorizedUsers(int id, string name)
            => MyController<StatusesController>
                .Instance()
                .WithData(new Status
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
                    .WithSet<Status>(set =>
                    {
                        set.FirstOrDefault(status => status.Id == id).ShouldBeNull();
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
