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
using Xunit;

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

    }
}

