using API.Domain.Common;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.src.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using Xunit;

namespace UnitTest.APITests.Controllers
{
    public class ReservationControllerTests
    {
        private readonly Mock<IReservationRepository> _repositoryMock;
        private readonly Mock<IReservationService>    _serviceMock;
        private readonly ReservationController        _sut;

        public ReservationControllerTests()
        {
            _repositoryMock = new Mock<IReservationRepository>();
            _serviceMock    = new Mock<IReservationService>();
            _sut = new ReservationController(_repositoryMock.Object, _serviceMock.Object);
        }

        // ── GetReservationById ────────────────────────────────────────────

        [Fact]
        public async Task GetReservationById_WhenFound_ReturnsOkWithReservation()
        {
            var id          = Guid.NewGuid();
            var reservation = new Reservation { Id = id };
            _repositoryMock
                .Setup(r => r.GetReservationByIdAsync(id))
                .ReturnsAsync(ResultOf<Reservation>.Success(reservation));

            var actionResult = await _sut.GetReservationById(id);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(reservation, ok.Value);
        }

        [Fact]
        public async Task GetReservationById_WhenNotFound_Returns404()
        {
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetReservationByIdAsync(id))
                .ReturnsAsync(ResultOf<Reservation>.Failure("Reservation not found"));

            var actionResult = await _sut.GetReservationById(id);

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetReservationById_WhenRepositoryFailsWithOtherError_Returns500()
        {
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetReservationByIdAsync(id))
                .ReturnsAsync(ResultOf<Reservation>.Failure("Unexpected error"));
            
            var actionResult = await _sut.GetReservationById(id);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetReservationById_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetReservationByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("DB crash"));
            
            var actionResult = await _sut.GetReservationById(Guid.NewGuid());
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetReservationByShowingIdAsync ────────────────────────────────

        [Fact]
        public async Task GetReservationByShowingId_WhenReservationsExist_ReturnsOk()
        {
            var reservations = new List<Reservation> { new Reservation(), new Reservation() };
            _repositoryMock
                .Setup(r => r.GetReservationByShowingAsync(1))
                .ReturnsAsync(reservations);
            
            var actionResult = await _sut.GetReservationByShowingIdAsync(1);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(reservations, ok.Value);
        }

        [Fact]
        public async Task GetReservationByShowingId_WhenListIsEmpty_Returns404()
        {
            _repositoryMock
                .Setup(r => r.GetReservationByShowingAsync(99))
                .ReturnsAsync(new List<Reservation>());

            var actionResult = await _sut.GetReservationByShowingIdAsync(99);

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetReservationByShowingId_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.GetReservationByShowingAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            var actionResult = await _sut.GetReservationByShowingIdAsync(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── AddReservationById (Suggest) ──────────────────────────────────

        [Fact]
        public async Task Suggest_WhenServiceSucceeds_ReturnsOkWithSuggestResponse()
        {
            var request  = new SuggestRequest(ShowingId: 1, NormalCount: 2, WheelchairCount: 0);
            var response = new SuggestResponse(Guid.NewGuid(), new List<SeatInfo>(), Found: true);
            _serviceMock
                .Setup(s => s.SuggestAsync(request))
                .ReturnsAsync(response);

            var actionResult = await _sut.AddReservationById(request);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Suggest_WhenServiceThrows_Returns500()
        {
            _serviceMock
                .Setup(s => s.SuggestAsync(It.IsAny<SuggestRequest>()))
                .ThrowsAsync(new Exception("Service error"));
            
            var actionResult = await _sut.AddReservationById(
                new SuggestRequest(ShowingId: 1, NormalCount: 2, WheelchairCount: 0));
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── ConfirmReservationById ────────────────────────────────────────

        [Fact]
        public async Task ConfirmReservation_WhenFound_ReturnsOkWithUpdatedReservation()
        {
            var suggestionId = Guid.NewGuid();
            var confirmed    = new Reservation { Id = suggestionId };
            _repositoryMock
                .Setup(r => r.UpdateReservationStatusAsync(suggestionId, "Confirmed"))
                .ReturnsAsync(confirmed);

            var actionResult = await _sut.ConfirmReservationById(new ConfirmRequest(suggestionId));

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(confirmed, ok.Value);
        }

        [Fact]
        public async Task ConfirmReservation_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.UpdateReservationStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));
            
            var actionResult = await _sut.ConfirmReservationById(
                new ConfirmRequest(Guid.NewGuid()));
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task ConfirmReservation_PassesCorrectStatusToRepository()
        {
            var suggestionId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.UpdateReservationStatusAsync(suggestionId, "Confirmed"))
                .ReturnsAsync(new Reservation());

            await _sut.ConfirmReservationById(new ConfirmRequest(suggestionId));

            _repositoryMock.Verify(
                r => r.UpdateReservationStatusAsync(suggestionId, "Confirmed"),
                Times.Once);
        }

        // ── CancelReservationById ─────────────────────────────────────────

        [Fact]
        public async Task CancelReservation_WhenFound_ReturnsOkWithCancelledReservation()
        {
            var reservationId = Guid.NewGuid();
            var cancelled     = new Reservation { Id = reservationId };
            _repositoryMock
                .Setup(r => r.UpdateReservationStatusAsync(reservationId, "Cancelled"))
                .ReturnsAsync(cancelled);

            var actionResult = await _sut.CancelReservationById(new CancelRequest(reservationId));

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(cancelled, ok.Value);
        }

        [Fact]
        public async Task CancelReservation_WhenRepositoryThrows_Returns500()
        {
            _repositoryMock
                .Setup(r => r.UpdateReservationStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("DB error"));

            var actionResult = await _sut.CancelReservationById(
                new CancelRequest(Guid.NewGuid()));

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task CancelReservation_PassesCorrectStatusToRepository()
        {
            var reservationId = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.UpdateReservationStatusAsync(reservationId, "Cancelled"))
                .ReturnsAsync(new Reservation());

            await _sut.CancelReservationById(new CancelRequest(reservationId));
            
            _repositoryMock.Verify(
                r => r.UpdateReservationStatusAsync(reservationId, "Cancelled"),
                Times.Once);
        }
    }
}
