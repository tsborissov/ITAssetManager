using AutoMapper;
using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Infrastructure;
using ITAssetManager.Web.Services.Statuses;
using ITAssetManager.Web.Services.Statuses.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using Xunit;

namespace ITAssetManager.Test.Services
{
    public class StatusServiceTest
    {
        [Fact]
        public void IsExistingStatusShouldReturnTrueIfStatusFoundById()
        {
            //Arrange
            var status = new Status
            {
                Id = 1,
                Name = "Test Status"
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.IsExistingStatus(1);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Status")]
        public void IsExistingNameShouldReturnTrueIfStatusFoundByName(int statusId,string statusName)
        {
            //Arrange
            var status = new Status
            {
                Id = statusId,
                Name = statusName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.IsExistingName(statusName);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Status")]
        public void IsInUseShouldReturnTrueIfStatusIsInUse(int statusId, string statusName)
        {
            //Arrange
            var status = new Status
            {
                Id = statusId,
                Name = statusName
            };

            status.Assets.Add(new Asset { Id = 1 });

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.IsInUse(statusId);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(1, "Test Status")]
        public void DeleteShouldDeleteStatusAndReturnDeletedStatusName(int statusId, string statusName)
        {
            //Arrange
            var status = new Status
            {
                Id = statusId,
                Name = statusName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.Delete(statusId);

            //Assert
            Assert.Empty(dbContext.Statuses);
            Assert.Equal(statusName, result);
        }

        [Theory]
        [InlineData(1, "CurrentName", "NewName")]
        public void UpdateShouldUpdateStatusNameAndReturnNumberOfStateEntriesGreaterThanZeroIfSuccess(
            int statusId,
            string currentStatusName,
            string newStatusName)
        {
            //Arrange
            var status = new Status
            {
                Id = statusId,
                Name = currentStatusName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusModel = new StatusEditServiceModel
            {
                Id = statusId,
                Name = newStatusName
            };

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.Update(statusModel);

            //Assert
            var resultStatusName = dbContext.Statuses.Find(statusId).Name;

            Assert.True(result > 0);
            Assert.Equal(newStatusName, resultStatusName);
        }

        [Theory]
        [InlineData(1, "Test Status")]
        public void DetailsShouldReturnCorrectModel(int statusId, string statusName)
        {
            //Arrange
            var status = new Status
            {
                Id = statusId,
                Name = statusName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.Details(statusId);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<StatusEditServiceModel>(result);
        }

        [Theory]
        [InlineData("Test Status")]
        public void AddShouldCreateNewStatusAndReturnName(string statusName)
        {
            //Arrange
            var statusModel = new StatusAddFormServiceModel
            {
                Name = statusName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var statusService = new StatusService(dbContext, mapper);

            //Act
            var result = statusService.Add(statusModel);

            //Assert
            Assert.NotEmpty(dbContext.Statuses);
            Assert.Equal(statusName, result);
        }

        [Theory]
        [InlineData(1, "Test Status")]
        public void AllShouldReturnCorrectModelAndAllStatuses(int statusId, string statusName)
        {
            //Arrange
            var status = new Status
            {
                Id = statusId,
                Name = statusName
            };

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

            var dbContext = new AppDbContext(optionsBuilder.Options);

            dbContext.Statuses.Add(status);
            dbContext.SaveChanges();

            var statusService = new StatusService(dbContext, Mock.Of<IMapper>());

            //Act
            var result = statusService.All(null, null, 1);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<StatusQueryServiceModel>(result);
            Assert.Equal(1, result.Statuses.Count);
        }
    }
}
