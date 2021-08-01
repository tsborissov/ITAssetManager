﻿using ITAssetManager.Data;
using ITAssetManager.Web.Services.AssetModels;
using ITAssetManager.Web.Services.AssetModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Web.Controllers
{
    public class AssetModelsController : Controller
    {
        private readonly IAssetModelService assetModelService;

        public AssetModelsController(IAssetModelService assetModelService, AppDbContext data) 
            => this.assetModelService = assetModelService;

        public IActionResult Add() => View(
            new AssetModelsAddFormServiceModel
            {
                Brands = this.assetModelService.GetBrands(),
                Categories = this.assetModelService.GetCategories()
            });

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Add(AssetModelsAddFormServiceModel assetModel)
        {
            if (!this.assetModelService.IsBrandValid(assetModel.BrandId))
            {
                this.ModelState.AddModelError(nameof(assetModel.BrandId), "Invalid 'Brand' selected!");
            }

            if (!this.assetModelService.IsCategoryValid(assetModel.CategoryId))
            {
                this.ModelState.AddModelError(nameof(assetModel.CategoryId), "Invalid 'Category' selected!");
            }

            if (!this.ModelState.IsValid)
            {
                assetModel.Categories = this.assetModelService.GetCategories();
                assetModel.Brands = this.assetModelService.GetBrands();

                return View(assetModel);
            }

            this.assetModelService.Add(assetModel);

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        public IActionResult All(string searchString, string sortOrder, int currentPage)
        {
            var queryResult = this.assetModelService.All(searchString, currentPage);

            return View(queryResult);
        }

        [Authorize]
        public IActionResult Details(int id, string searchString, int currentPage)
        {
            if (!assetModelService.IsExistingModel(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var assetModel = this.assetModelService.Details(id);

            assetModel.SearchString = searchString;
            assetModel.CurrentPage = currentPage;

            var referer = HttpContext.Request.Headers["Referer"].ToString().Split('/').Skip(3).ToArray();

            var refererController = referer[0];
            var refererAction = referer[1].Split('?').Take(1).ToArray()[0];

            if (refererAction == nameof(Edit))
            {
                refererController = ControllerContext.ActionDescriptor.ControllerName;
                refererAction = nameof(All);
            }

            ViewBag.RefererController = refererController;
            ViewBag.RefererAction = refererAction;

            return View(assetModel);
        }

        [Authorize(Roles = AdministratorRoleName)]
        public IActionResult Edit(int id, string searchString, int currentPage)
        {
            if (!assetModelService.IsExistingModel(id))
            {
                return RedirectToAction("Error", "Home");
            }

            var assetModel = this.assetModelService.Details(id);

            var brands = this.assetModelService.GetBrands();
            var currentBrandId = brands
                .Where(b => b.Name == assetModel.Brand)
                .Select(b => b.Id)
                .FirstOrDefault();
            
            var categories = this.assetModelService.GetCategories();
            var currentCategoryId = categories
                .Where(c => c.Name == assetModel.Category)
                .Select(c => c.Id)
                .FirstOrDefault();

            return View(
                new AssetModelEditFormServiceModel 
                { 
                    Id = assetModel.Id,
                    BrandId = currentBrandId,
                    Brands = brands,
                    CategoryId = currentCategoryId,
                    Categories= categories,
                    Name = assetModel.Name,
                    ImageUrl = assetModel.ImageUrl,
                    Details = assetModel.Details,
                    SearchString = searchString,
                    CurrentPage = currentPage
                });
        }

        [Authorize(Roles = AdministratorRoleName)]
        [HttpPost]
        public IActionResult Edit(AssetModelEditFormServiceModel assetModel)
        {
            if (!this.ModelState.IsValid)
            {
                var brands = this.assetModelService.GetBrands();
                assetModel.Brands = brands;

                var categories = this.assetModelService.GetCategories();
                assetModel.Categories = categories;

                return View(assetModel);
            }

            if (!this.assetModelService.IsExistingModel(assetModel.Id))
            {
                return RedirectToAction("Error", "Home");
            }

            this.assetModelService.Update(assetModel);

            return RedirectToAction(nameof(All));
        }
    }
}
