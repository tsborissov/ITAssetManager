using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.Categories;
using ITAssetManager.Web.Services.Categories.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using Xunit;

namespace ITAssetManager.Test.Services
{
    public class CategoryServiceTest
    {
        [Fact]
        public void IsExistingCategoryShouldReturnTrueIfCategoryFoundById()
        {
            //Arrange
            var category = new Category
            {
                Id = 1,
                Name = "Test Category"
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.IsExistingCategory(1);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("Test Category")]
        public void IsExistingNameShouldReturnTrueIfCategoryFoundByName(string categoryName)
        {
            //Arrange
            var category = new Category
            {
                Id = 1,
                Name = categoryName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.IsExistingName(categoryName);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Category")]
        public void IsInUseShouldReturnTrueIfCategoryIsInUse(int categoryId, string categoryName)
        {
            //Arrange
            var category = new Category
            {
                Id = categoryId,
                Name = categoryName
            };

            category.AssetModels.Add(new AssetModel { Id = 1 });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.IsInUse(categoryId);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Category")]
        public void DeleteShouldDeleteCategoryAndReturnDeletedCategoryName(int categoryId, string categoryName)
        {
            //Arrange
            var category = new Category
            {
                Id = categoryId,
                Name = categoryName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.Delete(categoryId);

            //Assert
            Assert.Empty(dbContext.Categories);
            Assert.Equal(categoryName, result);
        }

        [Theory]
        [InlineData(1, "CurrentName", "NewName")]
        public void UpdateShouldUpdateCategoryNameAndReturnNumberOfStateEntriesGreaterThanZeroIfSuccess(
            int categoryId,
            string currentCategoryName,
            string newCategoryName)
        {
            //Arrange
            var category = new Category
            {
                Id = categoryId,
                Name = currentCategoryName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryModel = new CategoryEditServiceModel
            {
                Id = categoryId,
                Name = newCategoryName
            };

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.Update(categoryModel);

            //Assert
            var resultCategoryName = dbContext.Categories.Find(categoryId).Name;

            Assert.True(result > 0);
            Assert.Equal(newCategoryName, resultCategoryName);
        }

        [Theory]
        [InlineData(1, "Test Category")]
        public void DetailsShouldReturnCorrectModel(int categoryId, string categoryName)
        {
            //Arrange
            var category = new Category
            {
                Id = categoryId,
                Name = categoryName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.Details(categoryId);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryEditServiceModel>(result);
        }

        [Theory]
        [InlineData("Test Category")]
        public void AddShouldCreateNewCategoryAndReturnName(string categoryName)
        {
            //Arrange
            var categoryModel = new CategoryAddFormServiceModel
            {
                Name = categoryName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var categoryService = new CategoryService(dbContext, mapper);

            //Act
            var result = categoryService.Add(categoryModel);

            //Assert
            Assert.NotEmpty(dbContext.Categories);
            Assert.Equal(categoryName, result);
        }

        [Theory]
        [InlineData(1, "Test Category")]
        public void AllShouldReturnCorrectModelAndAllCategories(int categoryId, string categoryName)
        {
            //Arrange
            var category = new Category
            {
                Id = categoryId,
                Name = categoryName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            var categoryService = new CategoryService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = categoryService.All(null, null, 1);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryQueryServiceModel>(result);
            Assert.Equal(1, result.Categories.Count);
        }
    }
}
