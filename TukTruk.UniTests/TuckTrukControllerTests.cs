using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TukTruk.Api.Controllers;
using TukTruk.Api.Core.IConfiguration;
using TukTruk.Api.Core.IRepositories;
using TukTruk.Api.Models;
using Xunit;

namespace TukTruk.UniTests;

public class TrucksControllerTests
{

    private readonly Mock<ITrucksRepository> repositoryStub = new();

    private readonly Mock<IUnitOfWork> wowStub = new();
    private readonly Mock<ILogger<TrucksController>> loggerStub = new();

    private readonly Random rand = new();

    [Fact]
    public async Task GetTruckAsync_TruckNotExists_ReturnsNotFound()
    {
        repositoryStub.Setup(repo => repo.GetById(It.IsAny<Guid>()))
            .ReturnsAsync((Truck)null);

        var controller = new TrucksController(repositoryStub.Object, wowStub.Object, loggerStub.Object);

        var result = await controller.GetTruckAsync(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetTruckAsync_TruckExists_ReturnsExpectedTruck()
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
    public async Task GetTruckAsync_WithExistingTrucks_ReturnsAllTrucks()
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
    public async Task CreateTruckAsync_WithTruckObject_ReturnNewTruck()
    {

        var truck = new Truck
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = 2022,
            ModelYear = 2022,
            Model = 1
        };

        var controller = GetTruckController();

        var result = await controller.CreateTruckAsync(truck);
        var createdTruck = (result as CreatedAtActionResult).Value as Truck;

        createdTruck.Should().BeEquivalentTo(
            truck,
            options => options.ComparingByMembers<Truck>());

        createdTruck?.Id.Should().NotBeEmpty();
    }

    private Truck CreateRandomTruck()
    {
        var manufacturingYear = rand.Next(2022, 2030);
        return new()
        {
            Id = Guid.NewGuid(),
            ManufacturingYear = manufacturingYear,
            ModelYear = rand.Next(manufacturingYear, manufacturingYear + 1),
            Model = 1
        };
    }

    private TrucksController GetTruckController()
    {
        return new TrucksController(repositoryStub.Object, wowStub.Object, loggerStub.Object);
    }
}