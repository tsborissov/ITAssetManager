using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.AssetModels;
using ITAssetManager.Web.Services.AssetModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using Xunit;

namespace ITAssetManager.Test.Services
{
    public class AssetModelServiceTest
    {
        [Theory]
        [InlineData(1)]
        public void IsExistingAssetModelShouldReturnTrueIfAssetModelFoundById(int assetModelId)
        {
            //Arrange
            var assetModel = new AssetModel
            {
                Id = assetModelId
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModel);
            dbContext.SaveChanges();

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.IsExistingModel(1);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, 1)]
        public void IsInUseShouldReturnTrueIfAssetModelIsInUse(int assetModelId, int assetId)
        {
            //Arrange
            var assetModel = new AssetModel
            {
                Id = assetModelId
            };

            assetModel.Assets.Add(new Asset { Id = assetId });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModel);
            dbContext.SaveChanges();

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.IsInUse(assetModelId);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1)]
        public void IsExistingModelShouldReturnTrueIfAssetModelExists(int assetModelId)
        {
            //Arrange
            var assetModel = new AssetModel
            {
                Id = assetModelId
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModel);
            dbContext.SaveChanges();

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.IsExistingModel(assetModelId);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Name")]
        public void DeleteShouldDeleteAssetModelAndReturnDeletedAssetModelName(int assetModelId, string name)
        {
            //Arrange
            var assetModel = new AssetModel
            {
                Id = assetModelId,
                Name = name
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModel);
            dbContext.SaveChanges();

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.Delete(assetModelId);

            //Assert
            Assert.Empty(dbContext.AssetModels);
            Assert.Equal(name, result);
        }

        [Theory]
        [InlineData(1)]
        public void UpdateShouldUpdateAssetModelAndReturnNumberOfStateEntriesGreaterThanZeroIfSuccess(int assetModelId)
        {
            //Arrange
            var assetModelData = new AssetModel
            {
                Id = assetModelId,
                Name = "Name",
                Details = "Details",
                ImageUrl = "ImageUrl"
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModelData);
            dbContext.SaveChanges();

            var assetModel = new AssetModelEditFormServiceModel
            {
                Id = assetModelId,
                Name = "Changed",
                Details = "Changed",
                ImageUrl = "Changed"
            };

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.Update(assetModel);

            //Assert
            var updatedAsetModel = dbContext.AssetModels.Find(assetModelId);

            Assert.True(result > 0);
            Assert.Equal(assetModel.Name, updatedAsetModel.Name);
            Assert.Equal(assetModel.Details, updatedAsetModel.Details);
            Assert.Equal(assetModel.ImageUrl, updatedAsetModel.ImageUrl);
        }

        [Theory]
        [InlineData(1)]
        public void DetailsShouldReturnCorrectModel(int id)
        {
            //Arrange
            var assetModel = new AssetModel
            {
                Id = id,
                Brand = new Brand { Id = 1},
                Category = new Category { Id = 1}
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModel);
            dbContext.SaveChanges();

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.Details(id);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AssetModelDetailsServiceModel>(result);
        }

        [Theory]
        [InlineData("Test Name")]
        public void AddShouldCreateNewAssetModelAndReturnName(string name)
        {
            //Arrange
            var assetModel = new AssetModelsAddFormServiceModel
            {
                Name = name
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var assetModelService = new AssetModelService(dbContext, mapper, Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.Add(assetModel);

            //Assert
            Assert.NotEmpty(dbContext.AssetModels);
            Assert.Equal(name, result);
        }

        [Theory]
        [InlineData(1)]
        public void AllShouldReturnCorrectModelAndAllAssetModels(int id)
        {
            //Arrange
            var assetModel = new AssetModel
            {
                Id = id,
                Brand = new Brand { Id = 1 },
                Category = new Category { Id = 1 }
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.AssetModels.Add(assetModel);
            dbContext.SaveChanges();

            var assetModelService = new AssetModelService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetModelService.All(null, 1);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AssetModelQueryServiceModel>(result);
            Assert.Equal(1, result.AssetModels.Count);
        }
    }
}
