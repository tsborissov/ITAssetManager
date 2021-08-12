using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.Brands;
using ITAssetManager.Web.Services.Brands.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ITAssetManager.Test.Services
{
    public class BrandServiceTest
    {
        [Fact]
        public void IsExistingBrandShouldReturnTrueIfBrandFoundById()
        {
            //Arrange
            var brand = new Brand
            {
                Id = 1,
                Name = "Test Brand"
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.IsExistingBrand(1);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("Test Brand")]
        public void IsExistingNameShouldReturnTrueIfBrandFoundByName(string brandName)
        {
            //Arrange
            var brand = new Brand
            {
                Id = 1,
                Name = brandName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.IsExistingName(brandName);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Brand")]
        public void IsInUseShouldReturnTrueIfBrandIsInUse(int brandId, string brandName)
        {
            //Arrange
            var brand = new Brand
            {
                Id = brandId,
                Name = brandName
            };

            brand.AssetModels.Add(new AssetModel { Id = 1 });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.IsInUse(brandId);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Brand")]
        public void DeleteShouldDeleteBrandAndReturnDeletedBrandName(int brandId, string brandName)
        {
            //Arrange
            var brand = new Brand
            {
                Id = brandId,
                Name = brandName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.Delete(brandId);

            //Assert
            Assert.Empty(dbContext.Brands);
            Assert.Equal(brandName, result);
        }

        [Theory]
        [InlineData(1, "Current Brand Name", "New Brand Name")]
        public void UpdateShouldUpdateBrandNameAndReturnNumberOfStateEntriesGreaterThanZeroIfSuccess(
            int brandId, 
            string currentBrandName, 
            string newBrandName)
        {
            //Arrange
            var brand = new Brand
            {
                Id = brandId,
                Name = currentBrandName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandModel = new BrandEditServiceModel
            {
                Id = brandId,
                Name = newBrandName
            };

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.Update(brandModel);

            //Assert
            var resultBrandName = dbContext.Brands.Find(brandId).Name;

            Assert.True(result > 0);
            Assert.Equal(newBrandName, resultBrandName);
        }

        [Theory]
        [InlineData(1, "Test Brand")]
        public void DetailsShouldReturnCorrectModel(int brandId, string brandName)
        {
            //Arrange
            var brand = new Brand
            {
                Id = brandId,
                Name = brandName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.Details(brandId);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<BrandEditServiceModel>(result);
        }

        [Theory]
        [InlineData("Test Brand")]
        public void AddShouldCreateNewBrandAndReturnName(string brandName)
        {
            //Arrange
            var brandModel = new BrandAddFormServiceModel
            {
                Name = brandName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var brandService = new BrandService(dbContext, mapper);

            //Act
            var result = brandService.Add(brandModel);

            //Assert
            Assert.NotEmpty(dbContext.Brands);
            Assert.Equal(brandName, result);
        }

        [Theory]
        [InlineData(1, "Test Brand")]
        public void AllShouldReturnCorrectModelAndAllBrands(int brandId, string brandName)
        {
            //Arrange
            var brand = new Brand
            {
                Id = brandId,
                Name = brandName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TetsDbContext");

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Add(brand);
            dbContext.SaveChanges();

            var brandService = new BrandService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = brandService.All(null, null, 1);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<BrandQueryServiceModel>(result);
            Assert.Equal(1, result.Brands.Count);
        }
    }
}
