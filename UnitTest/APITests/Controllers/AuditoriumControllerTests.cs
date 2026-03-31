using API.Controllers;
using API.Domain.Common;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Models;
using Xunit;

namespace UnitTest.APITests.Controllers
{
    public class AuditoriumControllerTests
    {
        private readonly Mock<IAuditoriumService> _serviceMock;
        private readonly AuditoriumController     _sut;

        public AuditoriumControllerTests()
        {
            _serviceMock = new Mock<IAuditoriumService>();
            _sut = new AuditoriumController(_serviceMock.Object);
        }

        // ── GetAll ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_WhenServiceSucceeds_ReturnsOkWithAuditoriums()
        {
            ICollection<Auditorium> auditoriums = new List<Auditorium>
            {
                new Auditorium { Id = 1, Name = "Screen 1" },
                new Auditorium { Id = 2, Name = "Screen 2" }
            };
            _serviceMock
                .Setup(s => s.GetAuditoriumsAsync())
                .ReturnsAsync(ResultOf<ICollection<Auditorium>>.Success(auditoriums));
            
            var actionResult = await _sut.GetAll();
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(auditoriums, ok.Value);
        }

        [Fact]
        public async Task GetAll_WhenServiceFails_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetAuditoriumsAsync())
                .ReturnsAsync(ResultOf<ICollection<Auditorium>>.Failure("DB error"));

            var actionResult = await _sut.GetAll();
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetAll_WhenServiceThrows_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetAuditoriumsAsync())
                .ThrowsAsync(new Exception("Unexpected"));

            var actionResult = await _sut.GetAll();
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── DeleteById ────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteById_WhenAuditoriumExists_ReturnsOk()
        {
            var auditorium = new Auditorium { Id = 1, Name = "Screen 1" };
            _serviceMock
                .Setup(s => s.DeleteAuditoriumByIdAsync(1))
                .ReturnsAsync(ResultOf<Auditorium>.Success(auditorium));
            
            var actionResult = await _sut.DeleteById(1);
            
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task DeleteById_WhenAuditoriumNotFound_Returns404()
        {
            _serviceMock
                .Setup(s => s.DeleteAuditoriumByIdAsync(99))
                .ReturnsAsync(ResultOf<Auditorium>.Failure("Auditorium not found"));
            
            var actionResult = await _sut.DeleteById(99);
            
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task DeleteById_WhenServiceThrows_Returns500()
        {
            _serviceMock
                .Setup(s => s.DeleteAuditoriumByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            var actionResult = await _sut.DeleteById(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── GetAuditoriumById ─────────────────────────────────────────────

        [Fact]
        public async Task GetAuditoriumById_WhenFound_ReturnsOkWithAuditorium()
        {
            var auditorium = new Auditorium { Id = 1, Name = "Screen 1" };
            _serviceMock
                .Setup(s => s.GetAuditoriumAsync(1))
                .ReturnsAsync(ResultOf<Auditorium>.Success(auditorium));
            
            var actionResult = await _sut.GetAuditoriumById(1);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(auditorium, ok.Value);
        }

        [Fact]
        public async Task GetAuditoriumById_WhenNotFound_Returns404()
        {
            _serviceMock
                .Setup(s => s.GetAuditoriumAsync(99))
                .ReturnsAsync(ResultOf<Auditorium>.Failure("Auditorium not found"));
            
            var actionResult = await _sut.GetAuditoriumById(99);
            
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetAuditoriumById_WhenServiceFailsWithOtherError_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetAuditoriumAsync(1))
                .ReturnsAsync(ResultOf<Auditorium>.Failure("Unexpected DB error"));
            
            var actionResult = await _sut.GetAuditoriumById(1);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task GetAuditoriumById_WhenServiceThrows_Returns500()
        {
            _serviceMock
                .Setup(s => s.GetAuditoriumAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB crash"));

            var actionResult = await _sut.GetAuditoriumById(1);

            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── AddAuditoriumById ─────────────────────────────────────────────

        [Fact]
        public async Task AddAuditoriumById_WhenSuccessful_ReturnsOkWithNewAuditorium()
        {
            var created = new Auditorium { Id = 3, Name = "Screen 3" };
            var rows    = new List<RowConfig> { new RowConfig(Seats: 10, Wheelchair: 2) };
            _serviceMock
                .Setup(s => s.AddAuditoriumAsync(It.IsAny<CreateAuditoriumRequest>()))
                .ReturnsAsync(created);
            
            var actionResult = await _sut.AddAuditoriumById("Screen 3", rows);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(created, ok.Value);
        }

        [Fact]
        public async Task AddAuditoriumById_WhenServiceThrows_Returns500()
        {
            _serviceMock
                .Setup(s => s.AddAuditoriumAsync(It.IsAny<CreateAuditoriumRequest>()))
                .ThrowsAsync(new Exception("Constraint violation"));
            
            var actionResult = await _sut.AddAuditoriumById("Bad", new List<RowConfig>());
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }
    }
}
