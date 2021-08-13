using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.Vendors;
using ITAssetManager.Web.Services.Vendors.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using Xunit;

namespace ITAssetManager.Test.Services
{
    public class VendorServiceTest
    {
        [Theory]
        [InlineData(1)]
        public void IsExistingVendorShouldReturnTrueIfVendorFoundById(int vendorId)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId,
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.IsExistingVendor(1);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Vendor")]
        public void IsExistingNameShouldReturnTrueIfVendorFoundByName(int vendorId, string vendorName)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId,
                Name = vendorName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.IsExistingName(vendorName);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, 1)]
        public void IsInUseShouldReturnTrueIfVendorIsInUse(int vendorId, int assetId)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId
            };

            vendor.Assets.Add(new Asset { Id = assetId });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.IsInUse(vendorId);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "BG000111222")]
        public void IsExistingVatShouldReturnTrueIfVendorWithSameVatExists(int vendorId, string vendorVat)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId,
                Vat = vendorVat
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.IsExistingVat(vendorVat);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Vendor")]
        public void DeleteShouldDeleteVendorAndReturnDeletedVendorName(int vendorId, string vendorName)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId,
                Name = vendorName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.Delete(vendorId);

            //Assert
            Assert.Empty(dbContext.Vendors);
            Assert.Equal(vendorName, result);
        }

        [Theory]
        [InlineData(1)]
        public void UpdateShouldUpdateVendorAndReturnNumberOfStateEntriesGreaterThanZeroIfSuccess(int vendorId)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId,
                Name = "VendorName",
                Address = "VendorAddress",
                Email = "vendor@email.com",
                Telephone = "0888999777",
                Vat = "BG000111222"
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorModel = new VendorEditServiceModel
            {
                Id = vendorId,
                Name = "NewName",
                Address = "NewAddress",
                Email = "new.vendor@email.com",
                Telephone = "0888999666",
                Vat = "BG000111333"
            };

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.Update(vendorModel);

            //Assert
            var updatedVendor = dbContext.Vendors.Find(vendorId);

            Assert.True(result > 0);
            Assert.Equal(vendorModel.Name, updatedVendor.Name);
            Assert.Equal(vendorModel.Address, updatedVendor.Address);
            Assert.Equal(vendorModel.Email, updatedVendor.Email);
            Assert.Equal(vendorModel.Telephone, updatedVendor.Telephone);
            Assert.Equal(vendorModel.Vat, updatedVendor.Vat);
        }

        [Theory]
        [InlineData(1)]
        public void DetailsShouldReturnCorrectModel(int vendorId)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId,
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.Details(vendorId, null, null, 0);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<VendorDetailsServiceModel>(result);
        }

        [Theory]
        [InlineData("Test Name")]
        public void AddShouldCreateNewVendorAndReturnName(string vendorName)
        {
            //Arrange
            var vendorModel = new VendorAddFormServiceModel
            {
                Name = vendorName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var vendorService = new VendorService(dbContext, mapper);

            //Act
            var result = vendorService.Add(vendorModel);

            //Assert
            Assert.NotEmpty(dbContext.Vendors);
            Assert.Equal(vendorName, result);
        }

        [Theory]
        [InlineData(1)]
        public void AllShouldReturnCorrectModelAndAllVendors(int vendorId)
        {
            //Arrange
            var vendor = new Vendor
            {
                Id = vendorId
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Vendors.Add(vendor);
            dbContext.SaveChanges();

            var vendorService = new VendorService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = vendorService.All(null, null, 1);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<VendorQueryServiceModel>(result);
            Assert.Equal(1, result.Vendors.Count);
        }
    }
}
