using API.Controllers;
using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using SharedLibrary.DTOs.Responses;
using Xunit;

namespace UnitTest.APITests.Controllers
{
    public class ShowingControllerTests
    {
        private readonly Mock<IShowingRepository> _repositoryMock;
        private readonly Mock<IShowingService> _serviceMock;
        private readonly ShowingController _sut;

        public ShowingControllerTests()
        {
            _repositoryMock = new Mock<IShowingRepository>();
            _serviceMock    = new Mock<IShowingService>();
            _sut = new ShowingController(_repositoryMock.Object, _serviceMock.Object);
        }

        // ── GetAll ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_WhenRepositorySucceeds_ReturnsOkWithCollection()
        {
            ICollection<Showing> showings = new List<Showing> { new Showing(), new Showing() };
            _repositoryMock
                .Setup(r => r.GetShowingsAsync())
                .ReturnsAsync(ResultOf<ICollection<Showing>>.Success(showings));
            
            var actionResult = await _sut.GetAll();
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(showings, ok.Value);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryFails_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetShowingsAsync())
                .ReturnsAsync(ResultOf<ICollection<Showing>>.Failure("DB error"));
            
            var actionResult = await _sut.GetAll();
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetShowingsAsync())
                .ThrowsAsync(new Exception("Unexpected"));
            
            var actionResult = await _sut.GetAll();
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetShowingsWithPrices ─────────────────────────────────────────

        [Fact]
        public async Task GetShowingsWithPrices_WhenServiceSucceeds_ReturnsOkWithData()
        {
            var pricedShowings = new List<ShowingsWithPricesResponse> { new ShowingsWithPricesResponse() };
            _serviceMock
                .Setup(s => s.GetShowingsAsync())
                .ReturnsAsync(ResultOf<List<ShowingsWithPricesResponse>>.Success(pricedShowings));

            var actionResult = await _sut.GetShowingsWithPrices();
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(pricedShowings, ok.Value);
        }

        [Fact]
        public async Task GetShowingsWithPrices_WhenServiceFails_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetShowingsAsync())
                .ReturnsAsync(ResultOf<List<ShowingsWithPricesResponse>>.Failure("error"));
            
            var actionResult = await _sut.GetShowingsWithPrices();
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetShowingWithPrices ──────────────────────────────────────────

        [Fact]
        public async Task GetShowingWithPrices_WhenFound_ReturnsOk()
        {
            var response = new ShowingsWithPricesResponse();
            _serviceMock
                .Setup(s => s.GetShowingAsync(1))
                .ReturnsAsync(ResultOf<ShowingsWithPricesResponse>.Success(response));
            
            var actionResult = await _sut.GetShowingWithPrices(1);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task GetShowingWithPrices_WhenNotFound_Returns404()
        {
            _serviceMock
                .Setup(s => s.GetShowingAsync(99))
                .ReturnsAsync(ResultOf<ShowingsWithPricesResponse>.Failure("NotFound"));
            
            var actionResult = await _sut.GetShowingWithPrices(99);
            
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetShowingWithPrices_WhenServiceFailsWithOtherError_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetShowingAsync(1))
                .ReturnsAsync(ResultOf<ShowingsWithPricesResponse>.Failure("DB error"));
            
            var actionResult = await _sut.GetShowingWithPrices(1);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetShowingStateById ───────────────────────────────────────────

        [Fact]
        public async Task GetShowingStateById_WhenServiceSucceeds_ReturnsOkWithState()
        {
            var showing  = new Showing();
            var seats    = new List<SeatInfo>();
            var occupied = new HashSet<string>();
            var state    = new ShowingStateDto(showing, seats, occupied);

            _serviceMock
                .Setup(s => s.GetShowingStateAsync(1))
                .ReturnsAsync(ResultOf<ShowingStateDto>.Success(state));
            
            var actionResult = await _sut.GetShowingStateById(1);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(state, ok.Value);
        }

        [Fact]
        public async Task GetShowingStateById_WhenServiceFails_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetShowingStateAsync(1))
                .ReturnsAsync(ResultOf<ShowingStateDto>.Failure("Something went wrong"));
            
            var actionResult = await _sut.GetShowingStateById(1);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetShowingStateById_WhenServiceThrows_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetShowingStateAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Unexpected"));
            
            var actionResult = await _sut.GetShowingStateById(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── DeleteById ────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteById_WhenShowingExists_ReturnsOk()
        {
            var showing = new Showing { Id = 1 };
            _repositoryMock
                .Setup(r => r.DeleteShowingByIdAsync(1))
                .ReturnsAsync(ResultOf<Showing>.Success(showing));
            
            var actionResult = await _sut.DeleteById(1);
            
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task DeleteById_WhenShowingNotFound_Returns404()
        {
            _repositoryMock
                .Setup(r => r.DeleteShowingByIdAsync(99))
                .ReturnsAsync(ResultOf<Showing>.Failure("Showing not found"));
            
            var actionResult = await _sut.DeleteById(99);
            
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task DeleteById_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.DeleteShowingByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB connection lost"));
            
            var actionResult = await _sut.DeleteById(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetShowingById ────────────────────────────────────────────────

        [Fact]
        public async Task GetShowingById_WhenFound_ReturnsOkWithShowing()
        {
            var showing = new Showing { Id = 1 };
            _repositoryMock
                .Setup(r => r.GetShowingAsync(1))
                .ReturnsAsync(ResultOf<Showing>.Success(showing));
            
            var actionResult = await _sut.GetShowingById(1);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(showing, ok.Value);
        }

        [Fact]
        public async Task GetShowingById_WhenNotFound_Returns404()
        {
            _repositoryMock
                .Setup(r => r.GetShowingAsync(99))
                .ReturnsAsync(ResultOf<Showing>.Failure("Showing not found"));
            
            var actionResult = await _sut.GetShowingById(99);
            
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetShowingById_WhenRepositoryFailsWithOtherError_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetShowingAsync(1))
                .ReturnsAsync(ResultOf<Showing>.Failure("Unexpected DB error"));
            
            var actionResult = await _sut.GetShowingById(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetShowingById_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetShowingAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB crash"));
            
            var actionResult = await _sut.GetShowingById(1);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── AddShowingById ────────────────────────────────────────────────

        [Fact]
        public async Task AddShowingById_WhenSuccessful_ReturnsOkWithNewShowing()
        {
            var newShowing = new Showing { Id = 10 };
            _repositoryMock
                .Setup(r => r.AddShowingAsync(It.IsAny<CreateShowingRequest>()))
                .ReturnsAsync(newShowing);

            var actionResult = await _sut.AddShowingById(
                movieId: 1,
                auditoriumId: 2,
                startsAt: DateTimeOffset.UtcNow.AddDays(1));
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(newShowing, ok.Value);
        }

        [Fact]
        public async Task AddShowingById_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.AddShowingAsync(It.IsAny<CreateShowingRequest>()))
                .ThrowsAsync(new Exception("Constraint violation"));
            
            var actionResult = await _sut.AddShowingById(1, 2, DateTimeOffset.UtcNow);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetShowingDisplayById ─────────────────────────────────────────

        [Fact]
        public async Task GetShowingDisplayById_WhenFound_ReturnsOk()
        {
            var display = new ShowingDisplayResponse();
            _repositoryMock
                .Setup(r => r.GetShowingDisplayByIdAsync(1))
                .ReturnsAsync(ResultOf<ShowingDisplayResponse>.Success(display));

            var actionResult = await _sut.GetShowingDisplayById(1);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(display, ok.Value);
        }

        [Fact]
        public async Task GetShowingDisplayById_WhenNotFound_Returns404()
        {
            _repositoryMock
                .Setup(r => r.GetShowingDisplayByIdAsync(99))
                .ReturnsAsync(ResultOf<ShowingDisplayResponse>.Failure("Showing not found"));
            
            var actionResult = await _sut.GetShowingDisplayById(99);

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetShowingDisplayById_WhenRepositoryFails_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetShowingDisplayByIdAsync(1))
                .ReturnsAsync(ResultOf<ShowingDisplayResponse>.Failure("DB error"));
            
            var actionResult = await _sut.GetShowingDisplayById(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetUpcomingShowingsByMovieId ───────────────────────────────────

        [Fact]
        public async Task GetUpcomingShowingsByMovieId_WhenServiceSucceeds_ReturnsOkWithList()
        {
            IReadOnlyList<ShowingResponse> upcoming = new List<ShowingResponse>
            {
                new ShowingResponse(), new ShowingResponse()
            };
            _serviceMock
                .Setup(s => s.GetUpcomingShowingsByMovieIdAsync(5))
                .ReturnsAsync(ResultOf<IReadOnlyList<ShowingResponse>>.Success(upcoming));
            
            var actionResult = await _sut.GetUpcomingShowingsByMovieId(5);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(upcoming, ok.Value);
        }

        [Fact]
        public async Task GetUpcomingShowingsByMovieId_WhenServiceFails_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetUpcomingShowingsByMovieIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ResultOf<IReadOnlyList<ShowingResponse>>.Failure("error"));
            
            var actionResult = await _sut.GetUpcomingShowingsByMovieId(5);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetShowingDisplay ─────────────────────────────────────────────

        [Fact]
        public async Task GetShowingDisplay_WithNoDate_ReturnsOkWithAllShowings()
        {
            ICollection<ShowingDisplayResponse> displays = new List<ShowingDisplayResponse>
            {
                new ShowingDisplayResponse()
            };
            _serviceMock
                .Setup(s => s.GetShowingDisplayAsync(null))
                .ReturnsAsync(ResultOf<ICollection<ShowingDisplayResponse>>.Success(displays));
            
            var actionResult = await _sut.GetShowingDisplay(null);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(displays, ok.Value);
        }

        [Fact]
        public async Task GetShowingDisplay_WithSpecificDate_PassesDateToService()
        {
            var date = new DateOnly(2025, 6, 1);
            ICollection<ShowingDisplayResponse> displays = new List<ShowingDisplayResponse>();
            _serviceMock
                .Setup(s => s.GetShowingDisplayAsync(It.Is<DateOnly?>(d => d == date)))
                .ReturnsAsync(ResultOf<ICollection<ShowingDisplayResponse>>.Success(displays));

            var actionResult = await _sut.GetShowingDisplay(date);
            
            Assert.IsType<OkObjectResult>(actionResult);
            _serviceMock.Verify(s => s.GetShowingDisplayAsync(date), Times.Once);
        }

        [Fact]
        public async Task GetShowingDisplay_WhenServiceFails_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetShowingDisplayAsync(It.IsAny<DateOnly?>()))
                .ReturnsAsync(ResultOf<ICollection<ShowingDisplayResponse>>.Failure("error"));

            var actionResult = await _sut.GetShowingDisplay();

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }
    }
}
