using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TukTruk.Api.Controllers;
using TukTruk.Api.Core.IConfiguration;
using TukTruk.Api.Core.IRepositories;
using TukTruk.Api.Enums;
using TukTruk.Api.Models;
using Xunit;


namespace TukTruk.UniTests;

public class TrucksControllerTests
{
    private readonly Mock<ITrucksRepository> repositoryStub = new();

    private readonly Mock<IUnitOfWork> wowStub = new();
    private readonly Mock<ILogger<TrucksController>> loggerStub = new();

    private readonly IValidator<Truck> truckValidator = new TruckValidator();

    private readonly Random rand = new();

    [Fact]
    public async Task GetTruckAsync_Truck_Not_Exists_Returns_Not_Found()
    {
        repositoryStub.Setup(repo => repo.GetById(It.IsAny<Guid>()))
            .ReturnsAsync((Truck)null);

        var controller = GetTruckController();

        var result = await controller.GetTruckAsync(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetTruckAsync_TruckExists_Returns_Expected_Truck()
    {
        var expectedTruck = CreateRandomTruck();
        repositoryStub.Setup(repo => repo.GetById(It.IsAny<Guid>()))
            .ReturnsAsync(expectedTruck);
        var controller = GetTruckController();

        var result = await controller.GetTruckAsync(Guid.NewGuid()) as OkObjectResult;

        result?.Value.Should().BeEquivalentTo(
            expectedTruck,
            options => options.ComparingByMembers<Truck>());
    }

    [Fact]
    public async Task GetTruckAsync_With_Existing_Trucks_Returns_All_Trucks()
    {
        var expectedTrucks = new[]{
            CreateRandomTruck(),
            CreateRandomTruck(),
            CreateRandomTruck(),
            CreateRandomTruck()
        };
        repositoryStub.Setup(repo => repo.All())
            .ReturnsAsync(expectedTrucks);

        var controller = GetTruckController();

        var result = await controller.GetAsync() as OkObjectResult;

        result?.Value.Should().BeEquivalentTo(
            expectedTrucks,
            options => options.ComparingByMembers<Truck>());
    }

    [Fact]
    public async Task CreateTruckAsync_With_Valid_Truck_Object_Return_New_Truck()
    {

        var truck = new Truck
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = 2022,
            ModelYear = 2022,
            Model = TruckModelEnum.FM
        };

        var controller = GetTruckController();

        var result = await controller.CreateTruckAsync(truck);
        var createdTruck = (result as CreatedAtActionResult).Value as Truck;

        createdTruck.Should().BeEquivalentTo(
            truck,
            options => options.ComparingByMembers<Truck>());

        createdTruck?.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateTruckAsync_With_Invalid_Truck_Object_Return_Bad_Request()
    {
        var truck = new Truck
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = 2021,
            ModelYear = 2022,
            Model = TruckModelEnum.FM
        };

        var controller = GetTruckController();

        var result = await controller.CreateTruckAsync(truck);

        result?.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateTruckAsync_With_Existing_Truck_Returns_NoContent()
    {
        var existingTruck = CreateRandomTruck();

        repositoryStub.Setup(repo => repo.GetById(It.IsAny<Guid>()))
        .ReturnsAsync(existingTruck);

        var controller = GetTruckController();
        var truckId = existingTruck.Id;

        var truckToUpdate = new Truck
        {
            Model = existingTruck.Model,
            ManufacturingYear = existingTruck.ManufacturingYear,
            ModelYear = DateTime.Now.Year + 1,
        };

        var result = await controller.UpdateTruckAsync((Guid)truckId, truckToUpdate);

        result.Should().BeOfType<NoContentResult>();
    }


    // Model tests
    [Fact]
    public async Task Should_NOT_Have_When_Validating_Truck()
    {
        var truck = CreateRandomTruck();

        var result = truckValidator.TestValidate(truck);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Have_Error_When_MANUFACTURINGYEAR_Is_Lesser_Than_Current_Year()
    {

        var truck = new Truck
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = 2021,
            ModelYear = 2022,
            Model = TruckModelEnum.FM
        };

        var result = truckValidator.TestValidate(truck);
        result.ShouldHaveValidationErrorFor(truck => truck.ManufacturingYear);
    }

    [Fact]
    public async Task Should_Have_Error_When_MODELYAR_Is_Greater_Than_Subsequent_Year()
    {
        var truck = new Truck
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = 2022,
            ModelYear = 2099,
            Model = TruckModelEnum.FM
        };

        var result = truckValidator.TestValidate(truck);
        result.ShouldHaveValidationErrorFor(truck => truck.ModelYear);
    }


    private Truck CreateRandomTruck()
    {
        var manufacturingYear = DateTime.Now.Year;
        return new()
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = manufacturingYear,
            ModelYear = rand.Next(manufacturingYear, manufacturingYear + 1),
            Model = (TruckModelEnum)rand.Next((int)TruckModelEnum.FH, (int)TruckModelEnum.FM)
        };
    }

    private TrucksController GetTruckController()
    {
        return new TrucksController(repositoryStub.Object, wowStub.Object, truckValidator);
    }
}