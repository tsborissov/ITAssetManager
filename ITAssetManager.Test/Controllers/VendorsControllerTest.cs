using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Services.Vendors.Models;
using MyTested.AspNetCore.Mvc;
using Xunit;
using System.Linq;
using Shouldly;
using ITAssetManager.Web.Models.Vendors;
using System.Collections.Generic;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class VendorsControllerTest
    {
        [Fact]
        public void GetAddShouldReturnViewForAuthorizedUsers()
            => MyController<VendorsController>
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
            => MyController<VendorsController>
                .Instance()
                .Calling(c => c.Add(With.Default<VendorAddFormServiceModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName)
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostAddShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<VendorsController>
                .Instance()
                .Calling(c => c.Add(With.Default<VendorAddFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<VendorAddFormServiceModel>()
                    .Passing(vendor => vendor.Name == null));

        [Theory]
        [InlineData("Test Vendor")]
        public void PostAddShouldRedirectWithTempDataMessageAndShouldSaveVendorWithValidVendor(string name)
            => MyController<VendorsController>
                .Instance()
                .WithUser(user => user.InRole(AdministratorRoleName))
                .Calling(c => c.Add(new VendorAddFormServiceModel
                {
                    Name = name,
                    Vat = "BG000111222",
                    Email = "test@email.com",
                    Telephone = "0888777666",
                    Address = "Test Address"
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Vendor>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(vendor => vendor.Name == name).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect(result => result
                    .To<VendorsController>(c => c.All(With.Empty<VendorsQueryModel>())));

        [Theory]
        [InlineData(1, "Test Vendor")]
        public void AllShouldReturnCorrectVendorsForAuthorizedUsers(int id, string name)
            => MyController<VendorsController>
                .Instance()
                .WithData(new Vendor
                {
                    Id = id,
                    Name = name
                })
                .Calling(c => c.All(With.Default<VendorsQueryModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<VendorsQueryModel>()
                    .Passing(model =>
                    {
                        model.CurrentPage.ShouldBe(1);
                        model.Vendors.Count().ShouldBe(1);
                        model.Vendors.FirstOrDefault(vendor => vendor.Id == 1).ShouldNotBeNull();
                    }));

        [Theory]
        [InlineData(1, "Test Vendor")]
        public void GetEditShouldReturnViewWithCorrectVendorIfVendorExistsForAuthorizedUsers(int id, string name)
            => MyController<VendorsController>
                .Instance()
                .WithData(new Vendor
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
                    .WithModelOfType<VendorEditServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.Name.ShouldBe(name);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "Test Vendor")]
        public void GetEditShouldRedirectToHomeErrorIfVendorDoesNotExist(int id, int wrongId, string name)
            => MyController<VendorsController>
                .Instance()
                .WithData(new Vendor
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
            => MyController<VendorsController>
                .Calling(c => c.Edit(With.Default<VendorEditServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<VendorEditServiceModel>()
                    .Passing(s => s.Name == null));

        [Theory]
        [InlineData(1, "Old Name", "New Name")]
        public void PostEditShouldRedirectWithTempDataMessageAndShouldUpdateVendorWithValidVendorForAuthorizedUsers(int id, string oldName, string newName)
            => MyController<VendorsController>
                .Instance()
                .WithData(new Vendor
                {
                    Id = id,
                    Name = oldName,
                    Vat = "BG000111222",
                    Email = "test@email.com",
                    Telephone = "0888777666",
                    Address = "Test Address"
                })
                .Calling(c => c.Edit(new VendorEditServiceModel
                {
                    Id = id,
                    Name = newName,
                    Email = "other@email.com",
                    Vat = "BG000111222",
                    Telephone = "0888777555",
                    Address = "Other Address"
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
                    .WithSet<Vendor>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(vendor => vendor.Name == oldName).ShouldBeNull();
                        set.FirstOrDefault(vendor => vendor.Name == newName).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, 2, "Test Vendor")]
        public void DeleteShouldRedirectToHomeErrorIfVendorDoesNotExist(int id, int wrongId, string name)
           => MyController<VendorsController>
               .Instance()
               .WithData(new Vendor
               {
                   Id = id,
                   Name = name
               })
               .Calling(c => c.Delete(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "Test Vendor")]
        public void DeleteShouldRedirectToHomeErrorIfVendorIsInUse(int id, string name)
           => MyController<VendorsController>
               .Instance()
               .WithData(new Vendor
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
        [InlineData(1, "Test Vendor")]
        public void DeleteShouldRedirectWithTempDataMessageAndShouldDeleteVendorForAuthorizedUsers(int id, string name)
            => MyController<VendorsController>
                .Instance()
                .WithData(new Vendor
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
                    .WithSet<Vendor>(set =>
                    {
                        set.FirstOrDefault(vendor => vendor.Id == id).ShouldBeNull();
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
