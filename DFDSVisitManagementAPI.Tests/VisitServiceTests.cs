using DFDSVisitManagementAPI.Domain.src.DTOs.Visits;
using DFDSVisitManagementAPI.Domain.src.DTOs.Activities;
using DFDSVisitManagementAPI.Domain.src.Entities.Activities;
using DFDSVisitManagementAPI.Domain.src.Entities.Drivers;
using DFDSVisitManagementAPI.Domain.src.Entities.Visits;
using DFDSVisitManagementAPI.Domain.src.Data;
using DFDSVisitManagementAPI.Business.src.Services;
using Microsoft.EntityFrameworkCore;

namespace DFDSVisitManagementAPI.Tests
{
    public class VisitServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // Helper to build a valid CreateVisitDto with activities list
        private static CreateVisitDto BuildCreateDto(
            Guid driverId,
            string plate = "DK-AB-12345",
            VisitStatus status = VisitStatus.PreRegistered) =>
            new CreateVisitDto
            {
                Status = status,
                TruckLicensePlate = plate,
                DriverId = driverId,
                CreatedBy = "test",
                Activities = new List<CreateActivityDto>
                {
                    new CreateActivityDto
                    {
                        UnitNumber = "DFDS12345",
                        Type = ActivityType.Delivery,
                        CreatedBy = "test"
                    }
                }
            };

        private static Driver BuildDriver(string first = "James", string last = "Okafor") =>
            new Driver { FirstName = first, LastName = last, CreatedBy = "test" };

        [Fact]
        public async Task CreateAsync_ValidDto_ReturnsVisitResponse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver();
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            var dto = BuildCreateDto(driver.Id);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("DK-AB-12345", result.TruckLicensePlate);
            Assert.Equal(VisitStatus.PreRegistered, result.Status);
            Assert.Single(result.Activities);
            Assert.Equal("DFDS12345", result.Activities[0].UnitNumber);
        }

        [Fact]
        public async Task CreateAsync_LicensePlateIsUppercased()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver();
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            var dto = BuildCreateDto(driver.Id, plate: "dk-ab-12345"); // lowercase

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("DK-AB-12345", result.TruckLicensePlate); // should be uppercased
        }

        [Fact]
        public async Task CreateAsync_ActivityUnitNumberIsUppercased()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver();
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            var dto = new CreateVisitDto
            {
                Status = VisitStatus.PreRegistered,
                TruckLicensePlate = "DK-AB-12345",
                DriverId = driver.Id,
                CreatedBy = "test",
                Activities = new List<CreateActivityDto>
                {
                    new CreateActivityDto
                    {
                        UnitNumber = "dfds12345", // lowercase
                        Type = ActivityType.Delivery,
                        CreatedBy = "test"
                    }
                }
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("DFDS12345", result.Activities[0].UnitNumber); // should be uppercased
        }

        [Fact]
        public async Task CreateAsync_InvalidDriverId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new VisitService(context);
            var dto = BuildCreateDto(Guid.NewGuid()); // non-existent driver

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_MultipleActivities_AllPersisted()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver();
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            var dto = new CreateVisitDto
            {
                Status = VisitStatus.PreRegistered,
                TruckLicensePlate = "DK-AB-12345",
                DriverId = driver.Id,
                CreatedBy = "test",
                Activities = new List<CreateActivityDto>
                {
                    new CreateActivityDto
                    {
                        UnitNumber = "DFDS11111",
                        Type = ActivityType.Delivery,
                        CreatedBy = "test"
                    },
                    new CreateActivityDto
                    {
                        UnitNumber = "DFDS22222",
                        Type = ActivityType.Collection,
                        CreatedBy = "test"
                    }
                }
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Activities.Count);
        }


        [Fact]
        public async Task GetAllAsync_ReturnsAllVisits()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver("Maria", "Hansen");
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            await service.CreateAsync(BuildCreateDto(driver.Id, "DK-XY-99999", VisitStatus.AtGate));

            // Act
            var results = await service.GetAllAsync(new VisitQueryDto());

            // Assert
            Assert.NotEmpty(results.Data);  // ← was results, now results.Data
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsVisit()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver();
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            var created = await service.CreateAsync(BuildCreateDto(driver.Id));

            // Act
            var result = await service.GetByIdAsync(created!.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new VisitService(context);

            // Act
            var result = await service.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidId_UpdatesStatusAndReturnsDto()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var driver = BuildDriver("Lars", "Jensen");
            context.Drivers.Add(driver);
            await context.SaveChangesAsync();

            var service = new VisitService(context);
            var created = await service.CreateAsync(BuildCreateDto(driver.Id, "DK-LJ-11111"));

            // Act
            var updated = await service.UpdateAsync(created!.Id, new UpdateVisitDto
            {
                Status = VisitStatus.Completed,
                TruckLicensePlate = "DK-LJ-11111",
                UpdatedBy = "test"
            });

            // Assert
            Assert.NotNull(updated);
            Assert.Equal(VisitStatus.Completed, updated.Status);
            Assert.Equal("test", updated.UpdatedBy);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public async Task UpdateAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new VisitService(context);

            // Act
            var result = await service.UpdateAsync(Guid.NewGuid(), new UpdateVisitDto
            {
                Status = VisitStatus.Completed,
                TruckLicensePlate = "DK-XX-00000",
                UpdatedBy = "test"
            });

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
public async Task GetAllAsync_FilterByStatus_ReturnsOnlyMatchingVisits()
{
    // Arrange
    var context = CreateInMemoryContext();
    var driver = BuildDriver();
    context.Drivers.Add(driver);
    await context.SaveChangesAsync();

    var service = new VisitService(context);
    await service.CreateAsync(BuildCreateDto(driver.Id, "DK-AA-11111", VisitStatus.PreRegistered));
    await service.CreateAsync(BuildCreateDto(driver.Id, "DK-BB-22222", VisitStatus.AtGate));

    // Act
    var results = await service.GetAllAsync(new VisitQueryDto
    {
        Status = "PreRegistered"
    });

    // Assert
    Assert.Single(results.Data);
    Assert.All(results.Data, v => Assert.Equal(VisitStatus.PreRegistered, v.Status));
}

[Fact]
public async Task GetAllAsync_FilterByDriverId_ReturnsOnlyThatDriversVisits()
{
    // Arrange
    var context = CreateInMemoryContext();
    var driver1 = BuildDriver("James", "Okafor");
    var driver2 = BuildDriver("Maria", "Hansen");
    context.Drivers.AddRange(driver1, driver2);
    await context.SaveChangesAsync();

    var service = new VisitService(context);
    await service.CreateAsync(BuildCreateDto(driver1.Id, "DK-AA-11111"));
    await service.CreateAsync(BuildCreateDto(driver2.Id, "DK-BB-22222"));

    // Act
    var results = await service.GetAllAsync(new VisitQueryDto
    {
        DriverId = driver1.Id.ToString()
    });

    // Assert
    Assert.Single(results.Data);
    Assert.Equal(driver1.Id, results.Data[0].Driver!.Id);
}

[Fact]
public async Task GetAllAsync_PageSize_LimitsResults()
{
    // Arrange
    var context = CreateInMemoryContext();
    var driver = BuildDriver();
    context.Drivers.Add(driver);
    await context.SaveChangesAsync();

    var service = new VisitService(context);

    // Create 5 visits
    for (int i = 0; i < 5; i++)
        await service.CreateAsync(BuildCreateDto(driver.Id, $"DK-AA-0000{i}"));

    // Act — request only 2
    var results = await service.GetAllAsync(new VisitQueryDto { PageSize = 2 });

    // Assert
    Assert.Equal(2, results.Data.Count);
    Assert.True(results.HasMore);
    Assert.NotNull(results.NextCursor);
}
    }
}