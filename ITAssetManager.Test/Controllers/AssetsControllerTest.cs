using ITAssetManager.Data.Models;
using ITAssetManager.Web.Controllers;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Web.Services.Assets.Models;
using MyTested.AspNetCore.Mvc;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Controllers
{
    public class AssetsControllerTest
    {
        [Fact]
        public void GetAddShouldReturnViewForAuthorizedUsers()
            => MyController<AssetsController>
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
            => MyController<AssetsController>
                .Instance()
                .Calling(c => c.Add(With.Default<AssetAddFormServiceModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName)
                    .RestrictingForHttpMethod(HttpMethod.Post));

        [Fact]
        public void PostAddShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<AssetsController>
                .Instance()
                .Calling(c => c.Add(With.Default<AssetAddFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetAddFormServiceModel>()
                    .Passing(asset => asset.SerialNr == null));

        [Theory]
        [InlineData("BG123456")]
        public void PostAddShouldRedirectWithTempDataMessageAndShouldSaveAssetWithValidAsset(string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(
                    new AssetModel { Id = 1 },
                    new Status { Id = 1 },
                    new Vendor { Id = 1 })
                .WithUser(user => user.InRole(AdministratorRoleName))
                .Calling(c => c.Add(new AssetAddFormServiceModel
                {
                    InventoryNr = inventoryNr,
                    AssetModelId = 1,
                    InvoiceNr = "Test Invoice",
                    Price = 1,
                    PurchaseDate = DateTime.UtcNow,
                    SerialNr = "Test Serial",
                    StatusId = 1,
                    VendorId = 1
                }))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Asset>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(asset => asset.InventoryNr == inventoryNr).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, "BG123456")]
        public void AllShouldReturnCorrectAssetsForAuthenticatedUsers(int id, string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(new Asset
                {
                    Id = id,
                    InventoryNr = inventoryNr,
                    AssetModel = new AssetModel { Brand = new Brand { Name = "Test Brand" } },
                    Status = new Status { Id = 1, Name = "Test Status"}
                })
                .WithUser(new string[] { AdministratorRoleName })
                .Calling(c => c.All(With.Default<AssetsQueryModel>()))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetsQueryModel>()
                    .Passing(model =>
                    {
                        model.Assets.Count().ShouldBe(1);
                        model.Assets.FirstOrDefault(asset => asset.Id == 1).ShouldNotBeNull();
                    }));

        [Theory]
        [InlineData(1, "BG123456")]
        public void GetEditShouldReturnViewWithCorrectAssetIfAssetExistsForAuthorizedUsers(int id, string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(new Asset
                {
                    Id = id,
                    InventoryNr = inventoryNr
                })
                .Calling(c => c.Edit(id, null, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetEditFormServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.InventoryNr.ShouldBe(inventoryNr);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "BG123456")]
        public void GetEditShouldRedirectToHomeErrorIfAssetDoesNotExist(int id, int wrongId, string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(new Asset
                {
                    Id = id,
                    InventoryNr = inventoryNr
                })
                .Calling(c => c.Edit(wrongId, null, null, 1))
                .ShouldReturn()
                .Redirect(result => result
                    .To<HomeController>(c => c.Error()));

        [Fact]
        public void PostEditShouldReturnViewWithTheSameModelWhenModelStateIsInvalid()
            => MyController<AssetsController>
                .Calling(c => c.Edit(With.Default<AssetEditFormServiceModel>()))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetEditFormServiceModel>()
                    .Passing(s => s.InventoryNr == null));

        [Theory]
        [InlineData(1, "BG123456", "BG654321")]
        public void PostEditShouldRedirectWithTempDataMessageAndShouldUpdateAssetWithValidAssetForAuthorizedUsers(int id, string oldInventoryNr, string newInventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(new Asset
                {
                    Id = id,
                    InventoryNr = oldInventoryNr,
                    AssetModelId = 1,
                    InvoiceNr = "Test Invoice",
                    Price = 1,
                    PurchaseDate = DateTime.UtcNow,
                    SerialNr = "Test Serial",
                    StatusId = 1,
                    VendorId = 1
                })
                .Calling(c => c.Edit(new AssetEditFormServiceModel
                {
                    Id = id,
                    InventoryNr = newInventoryNr,
                    AssetModelId = 1,
                    InvoiceNr = "Test Invoice",
                    Price = 1,
                    PurchaseDate = DateTime.UtcNow,
                    SerialNr = "Test Serial",
                    StatusId = 1,
                    VendorId = 1
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
                    .WithSet<Asset>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(asset => asset.InventoryNr == oldInventoryNr).ShouldBeNull();
                        set.FirstOrDefault(asset => asset.InventoryNr == newInventoryNr).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, 2, "BG123456")]
        public void DeleteShouldRedirectToHomeErrorIfAssetDoesNotExist(int id, int wrongId, string inventoryNr)
           => MyController<AssetsController>
               .Instance()
               .WithData(new Asset
               {
                   Id = id,
                   InventoryNr = inventoryNr
               })
               .Calling(c => c.Delete(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "BG123456")]
        public void DeleteShouldRedirectToHomeErrorIfAssetIsInUse(int id, string inventoryNr)
           => MyController<AssetsController>
               .Instance()
               .WithData(new Asset
               {
                   Id = id,
                   InventoryNr = inventoryNr,
                   AssetUsers = new List<UserAsset> 
                   {
                       new UserAsset 
                       {
                           AssetId = id,
                           UserId = "Test User"
                       }
                   }
               })
               .Calling(c => c.Delete(id, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "BG123456")]
        public void DeleteShouldRedirectWithTempDataMessageAndShouldDeleteAssetForAuthorizedUsers(int id, string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(new Asset
                {
                    Id = id,
                    InventoryNr = inventoryNr
                })
                .Calling(c => c.Delete(id, null, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<Asset>(set =>
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

        [Theory]
        [InlineData(1, "BG123456")]
        public void GetAssignShouldReturnViewWithCorrectAssetIfAssetExistsForAuthorizedUsers(int id, string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(
                new Asset
                {
                    Id = id,
                    InventoryNr = inventoryNr,
                    AssetModel = new AssetModel { Brand = new Brand { Name = "Test Brand" } },
                    Status = new Status { Id = 1, Name = "Test Status" }
                },
                new ApplicationUser
                {
                    Id = "Test Id",
                    UserName = "user@email.com"
                })
                .Calling(c => c.Assign(id, null, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetAssignServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(id);
                        model.InventoryNr.ShouldBe(inventoryNr);
                        model.CurrentPage.ShouldBe(1);
                    }));

        [Theory]
        [InlineData(1, 2, "BG123456")]
        public void GetAssignShouldRedirectToHomeErrorIfAssetDoesNotExist(int id, int wrongId, string inventoryNr)
           => MyController<AssetsController>
               .Instance()
               .WithData(new Asset
               {
                   Id = id,
                   InventoryNr = inventoryNr
               })
               .Calling(c => c.Assign(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "TestUserId", "BG654321")]
        public void PostAssignShouldRedirectWithTempDataMessageAndShouldAssignAssetToUserForAuthorizedUsers(int id, string userId, string inventoryNr)
            => MyController<AssetsController>
                .Instance()
                .WithData(
                new Asset
                {
                    Id = id,
                    InventoryNr = inventoryNr
                },
                new ApplicationUser 
                {
                    Id = userId,
                    UserName = "test@email.com"
                })
                .Calling(c => c.Assign(new AssetAssignServiceModel
                {
                    Id = id,
                    UserId = userId
                }))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<UserAsset>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(ua => ua.AssetId == id).ShouldNotBeNull();
                        set.FirstOrDefault(ua => ua.UserId == userId).ShouldNotBeNull();
                        set.Where(ua => ua.AssetId == id && ua.UserId == userId).Select(ua => ua.AssignDate).ShouldNotBeNull();
                    }))
                .AndAlso()
                .ShouldHave()
                .TempData(tmp => tmp
                    .ContainingEntryWithKey(SuccessMessageKey))
                .AndAlso()
                .ShouldReturn()
                .Redirect();

        [Theory]
        [InlineData(1, "BG654321", "TestUserId", "testuser@email.com")]
        public void GetCollectShouldReturnViewWithCorrectAssetIfAssetExistsForAuthorizedUsers(int assetId, string inventoryNr, string userId, string username)
            => MyController<AssetsController>
                .Instance()
                .WithData(
                new Asset
                {
                    Id = assetId,
                    InventoryNr = inventoryNr,
                    AssetModel = new AssetModel { Brand = new Brand { Name = "Test Brand" } },
                    Status = new Status { Id = 1, Name = "Test Status" }
                },
                new ApplicationUser
                {
                    Id = userId,
                    UserName = username
                },
                new UserAsset
                {
                    AssetId = assetId,
                    UserId = userId,
                    AssignDate = DateTime.UtcNow
                })
                .Calling(c => c.Collect(assetId, null, null, 1))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldReturn()
                .View(result => result
                    .WithModelOfType<AssetCollectServiceModel>()
                    .Passing(model =>
                    {
                        model.Id.ShouldBe(assetId);
                        model.UserId.ShouldBe(userId);
                    }));

        [Theory]
        [InlineData(1, 2, "BG123456")]
        public void GetCollectShouldRedirectToHomeErrorIfAssetDoesNotExist(int id, int wrongId, string inventoryNr)
           => MyController<AssetsController>
               .Instance()
               .WithData(new Asset
               {
                   Id = id,
                   InventoryNr = inventoryNr
               })
               .Calling(c => c.Collect(wrongId, null, null, 1))
               .ShouldReturn()
               .Redirect(result => result
                   .To<HomeController>(c => c.Error()));

        [Theory]
        [InlineData(1, "TestUserId")]
        public void PostCollectShouldRedirectWithTempDataMessageAndShouldAssignAssetToUserForAuthorizedUsers(int assetId, string userId)
            => MyController<AssetsController>
                .Instance()
                .WithData(
                new Asset
                {
                    Id = assetId,
                    AssetModel = new AssetModel { Brand = new Brand { Name = "Test Brand" } },
                    Status = new Status { Id = 1, Name = "Test Status" }
                },
                new ApplicationUser
                {
                    Id = userId
                },
                new UserAsset
                {
                    AssetId = assetId,
                    UserId = userId,
                    AssignDate = DateTime.UtcNow
                })
                .Calling(c => c.Collect(new AssetCollectServiceModel
                {
                    Id = assetId,
                    UserId = userId,
                    ReturnDate = DateTime.UtcNow
                }))
                .ShouldHave()
                .ActionAttributes(att => att
                    .RestrictingForAuthorizedRequests(AdministratorRoleName))
                .AndAlso()
                .ShouldHave()
                .Data(data => data
                    .WithSet<UserAsset>(set =>
                    {
                        set.ShouldNotBeNull();
                        set.FirstOrDefault(ua => ua.AssetId == assetId).ShouldNotBeNull();
                        set.FirstOrDefault(ua => ua.UserId == userId).ShouldNotBeNull();
                        set.Where(ua => ua.AssetId == assetId && ua.UserId == userId).Select(ua => ua.ReturnDate).ShouldNotBeNull();
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
