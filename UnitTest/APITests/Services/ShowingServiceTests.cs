using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using Moq;
using SharedLibrary.Domain.Entities;
using Xunit;

namespace UnitTest.APITests.Services;

public class ShowingServiceTests
{
    private readonly Mock<IShowingRepository> _repoMock;
    private readonly Mock<IPricingService> _pricingMock;
    private readonly Mock<ITicketTypeService> _ticketTypeServiceMock;
    private readonly Mock<ITicketRuleService> _ruleMock;
    private readonly Mock<ITicketTypeRepository> _ticketTypeRepoMock;
    private readonly Mock<IReservationRepository> _reservationMock;

    private readonly ShowingService _sut;

    public ShowingServiceTests()
    {
        _repoMock = new Mock<IShowingRepository>();
        _pricingMock = new Mock<IPricingService>();
        _ticketTypeServiceMock = new Mock<ITicketTypeService>();
        _ruleMock = new Mock<ITicketRuleService>();
        _ticketTypeRepoMock = new Mock<ITicketTypeRepository>();
        _reservationMock = new Mock<IReservationRepository>();

        _sut = new ShowingService(
            _repoMock.Object,
            _pricingMock.Object,
            _ticketTypeServiceMock.Object,
            _ticketTypeRepoMock.Object,
            _reservationMock.Object,
            _ruleMock.Object);
    }

    private static Showing BuildShowing() => new Showing
    {
        Id = 1,
        Movie = new Movie { Title = "Test" },
        StartsAt = DateTime.Now.AddHours(1),
        IsThreeD = false
    };

    private static List<TicketType> BuildTicketTypes() => new()
    {
        new TicketType { Name = "Adult" },
        new TicketType { Name = "Child" },
        new TicketType { Name = "Student" },
        new TicketType { Name = "Senior" }
    };

    [Fact]
    public async Task GetShowingAsync_WhenSuccess_ReturnsResult()
    {
        _repoMock
            .Setup(r => r.GetShowingAsync(1))
            .ReturnsAsync(ResultOf<Showing>.Success(BuildShowing()));

        _ticketTypeServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(ResultOf<List<TicketType>>.Success(BuildTicketTypes()));

        _pricingMock
            .Setup(p => p.CalculatePriceAsync(It.IsAny<Movie>(), It.IsAny<bool>(), It.IsAny<TicketType>()))
            .ReturnsAsync(ResultOf<decimal>.Success(10));

        _ruleMock
            .Setup(r => r.IsTicketTypeAvailable(It.IsAny<TicketType>(), It.IsAny<Showing>(), It.IsAny<DateTime>()))
            .Returns(true);

        var result = await _sut.GetShowingAsync(1);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetShowingsAsync_WhenSuccess_ReturnsList()
    {
        var showings = new List<Showing> { BuildShowing() };

        _repoMock
            .Setup(r => r.GetShowingsAsync())
            .ReturnsAsync(ResultOf<ICollection<Showing>>.Success(showings));

        _ticketTypeServiceMock
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(ResultOf<List<TicketType>>.Success(BuildTicketTypes()));

        _pricingMock
            .Setup(p => p.CalculatePriceAsync(It.IsAny<Movie>(), It.IsAny<bool>(), It.IsAny<TicketType>()))
            .ReturnsAsync(ResultOf<decimal>.Success(10));

        _ruleMock
            .Setup(r => r.IsTicketTypeAvailable(It.IsAny<TicketType>(), It.IsAny<Showing>(), It.IsAny<DateTime>()))
            .Returns(true);

        var result = await _sut.GetShowingsAsync();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetShowingsAsync_WhenRepoFails_ReturnsFailure()
    {
        _repoMock
            .Setup(r => r.GetShowingsAsync())
            .ReturnsAsync(ResultOf<ICollection<Showing>>.Failure("error"));

        var result = await _sut.GetShowingsAsync();

        Assert.True(result.IsFailure);
    }
}