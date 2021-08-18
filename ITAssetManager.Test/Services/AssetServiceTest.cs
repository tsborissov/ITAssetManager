using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.Assets;
using ITAssetManager.Web.Services.Assets.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

using static ITAssetManager.Data.DataConstants;

namespace ITAssetManager.Test.Services
{
    public class AssetServiceTest
    {
        [Fact]
        public void AddShouldCreateNewAssetAndReturnTrueIfSuccess()
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var asset = new AssetAddFormServiceModel
            {
                AssetModelId = 1
            };

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            var mapper = mockMapper.CreateMapper();

            var assetService = new AssetService(dbContext, mapper, Mock.Of<IMemoryCache>());

            //Act
            var result = assetService.Add(asset);

            //Assert
            Assert.NotEmpty(dbContext.Assets);
            Assert.True(result);
        }

        [Theory]
        [InlineData(1)]
        public void AllShouldReturnCorrectModel(int id)
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var asset = new Asset
            {
                Id = id,
                AssetModelId = id,
                AssetModel = new AssetModel 
                { 
                    Id = id,
                    Brand = new Brand 
                    {
                        Id = id,
                        Name = "Brand"
                    }
                },
            };

            dbContext.Assets.Add(asset);
            dbContext.SaveChanges();

            var assetService = new AssetService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetService.All(null, null, 1, null);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AssetsQueryServiceModel>(result);
        }

        [Theory]
        [InlineData(1)]
        public void GetByIdShouldReturnCorrectModelAndAssetIfFound(int id)
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var asset = new Asset
            {
                Id = id,
                AssetModelId = id,
                AssetModel = new AssetModel
                {
                    Id = id,
                    Brand = new Brand
                    {
                        Id = id,
                    }
                },
            };

            dbContext.Assets.Add(asset);
            dbContext.SaveChanges();

            var assetService = new AssetService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetService.GetById(id, null, null, 1);

            //Assert
            Assert.IsType<AssetAssignServiceModel>(result);
            Assert.Equal(id, result.Id);
        }

        [Theory]
        [InlineData(1, 1, "UserId")]
        public void AssignShouldChangeTargetStatusAndReturnTrue(int assetId, int statusId, string userId)
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var asset = new Asset
            {
                Id = assetId
            };

            var status = new Status
            {
                Id = statusId,
                Name = AssetTargetAssignStatus
            };
            
            dbContext.Assets.Add(asset);
            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var assetService = new AssetService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetService.Assign(userId, assetId);

            //Assert
            Assert.True(result);
            Assert.Equal(statusId, asset.StatusId);
        }

        [Theory]
        [InlineData(1, "UserId")]
        public void GetUserAssetByIdShouldReturnCorrectModel(int assetId, string userId)
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var userAsset = new UserAsset
            {
                AssetId = assetId,
                Asset = new Asset 
                {
                    Id = assetId,
                    AssetModelId = 1,
                    AssetModel = new AssetModel 
                    {
                        Id = 1,
                        Name = "TestModel"
                    }
                },
                UserId = userId,
                User = new ApplicationUser 
                {
                    Id = userId,
                    UserName = "user@email.com"
                },
                AssignDate = DateTime.UtcNow
            };

            dbContext.UsersAssets.Add(userAsset);
            dbContext.SaveChanges();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var assetService = new AssetService(dbContext, mapper, Mock.Of<IMemoryCache>());

            //Act
            var result = assetService.GetUserAssetById(assetId);

            //Assert
            Assert.IsType<AssetCollectServiceModel>(result);
        }

        [Theory]
        [InlineData(1, 1, "UserId")]
        public void CollectShouldChangeTargetStatusAndReturnTrue(int assetId, int statusId, string userId)
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var asset = new Asset
            {
                Id = assetId
            };

            var userAsset = new UserAsset
            {
                AssetId = assetId,
                UserId = userId,
            };

            var status = new Status
            {
                Id = statusId,
                Name = AssetTargetCollectStatus
            };

            dbContext.Assets.Add(asset);
            dbContext.UsersAssets.Add(userAsset);
            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var assetService = new AssetService(dbContext, Mock.Of<IMapper>(), Mock.Of<IMemoryCache>());

            //Act
            var result = assetService.Collect(userId, assetId, DateTime.UtcNow);

            //Assert
            Assert.True(result);
            Assert.Equal(statusId, asset.StatusId);
        }
    }
}

