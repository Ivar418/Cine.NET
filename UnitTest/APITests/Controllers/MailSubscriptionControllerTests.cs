using API.Controllers;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Moq;
using SharedLibrary.DTOs.Requests;
using Xunit;

namespace UnitTest.APITests.Controllers
{
    public class MailSubscriptionControllerTests
    {
        private readonly Mock<ILocalMailService>      _mailServiceMock;
        private readonly MailSubscriptionController   _sut;

        public MailSubscriptionControllerTests()
        {
            _mailServiceMock = new Mock<ILocalMailService>();
            _sut = new MailSubscriptionController(_mailServiceMock.Object);
        }

        // ── Subscribe ─────────────────────────────────────────────────────

        [Fact]
        public async Task Subscribe_WhenEmailIsNew_AddsAndReturnsOk()
        {
            const string email = "user@example.com";
            _mailServiceMock.Setup(s => s.ExistsAsync(email)).ReturnsAsync(false);
            _mailServiceMock.Setup(s => s.AddAsync(email)).ReturnsAsync(true);
            
            var actionResult = await _sut.Subscribe(email);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal("Subscription successful.", ok.Value);
        }

        [Fact]
        public async Task Subscribe_WhenEmailIsNew_CallsAddAsync()
        {
            const string email = "user@example.com";
            _mailServiceMock.Setup(s => s.ExistsAsync(email)).ReturnsAsync(false);
            _mailServiceMock.Setup(s => s.AddAsync(email)).ReturnsAsync(true);
            
            await _sut.Subscribe(email);

            _mailServiceMock.Verify(s => s.AddAsync(email), Times.Once);
        }

        [Fact]
        public async Task Subscribe_WhenEmailAlreadyExists_ReturnsBadRequest()
        {
            const string email = "existing@example.com";
            _mailServiceMock.Setup(s => s.ExistsAsync(email)).ReturnsAsync(true);
            
            var actionResult = await _sut.Subscribe(email);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Email is already subscribed.", bad.Value);
        }

        [Fact]
        public async Task Subscribe_WhenEmailAlreadyExists_DoesNotCallAddAsync()
        {
            const string email = "existing@example.com";
            _mailServiceMock.Setup(s => s.ExistsAsync(email)).ReturnsAsync(true);

            await _sut.Subscribe(email);

            _mailServiceMock.Verify(s => s.AddAsync(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Subscribe_WhenEmailIsNullOrWhitespace_ReturnsBadRequest(string? email)
        {
            var actionResult = await _sut.Subscribe(email!);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Email is required.", bad.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Subscribe_WhenEmailIsNullOrWhitespace_DoesNotCallService(string? email)
        {
            await _sut.Subscribe(email!);
            
            _mailServiceMock.Verify(s => s.ExistsAsync(It.IsAny<string>()), Times.Never);
            _mailServiceMock.Verify(s => s.AddAsync(It.IsAny<string>()),    Times.Never);
        }

        [Fact]
        public async Task Subscribe_WhenServiceThrows_Returns500()
        {
            const string email = "user@example.com";
            _mailServiceMock.Setup(s => s.ExistsAsync(email)).ThrowsAsync(new Exception("DB crash"));

            var actionResult = await _sut.Subscribe(email);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── Unsubscribe ───────────────────────────────────────────────────

        [Fact]
        public async Task Unsubscribe_WhenEmailIsSubscribed_ReturnsOk()
        {
            const string email = "user@example.com";
            _mailServiceMock.Setup(s => s.RemoveAsync(email)).ReturnsAsync(true);
            
            var actionResult = await _sut.Unsubscribe(email);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal("Unsubscription successful.", ok.Value);
        }

        [Fact]
        public async Task Unsubscribe_WhenEmailIsNotSubscribed_ReturnsBadRequest()
        {
            const string email = "notsubscribed@example.com";
            _mailServiceMock.Setup(s => s.RemoveAsync(email)).ReturnsAsync(false);
            
            var actionResult = await _sut.Unsubscribe(email);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Email is not subscribed.", bad.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Unsubscribe_WhenEmailIsNullOrWhitespace_ReturnsBadRequest(string? email)
        {
            var actionResult = await _sut.Unsubscribe(email!);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Email is required.", bad.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task Unsubscribe_WhenEmailIsNullOrWhitespace_DoesNotCallService(string? email)
        {
            await _sut.Unsubscribe(email!);
            
            _mailServiceMock.Verify(s => s.RemoveAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Unsubscribe_WhenServiceThrows_Returns500()
        {
            const string email = "user@example.com";
            _mailServiceMock.Setup(s => s.RemoveAsync(email)).ThrowsAsync(new Exception("DB crash"));
            
            var actionResult = await _sut.Unsubscribe(email);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }

        // ── SendEmailToSubscribers ────────────────────────────────────────

        [Fact]
        public async Task SendEmailToSubscribers_WhenSucceeds_ReturnsOk()
        {
            var request = new EmailRequest
            {
                Subject      = "Hello",
                FromName     = "Cinema",
                EmailContent = "Check out our new movies!"
            };
            _mailServiceMock
                .Setup(s => s.SendEmailToSubscribersAsync(
                    It.IsAny<TextPart>(),
                    request.FromName,
                    request.Subject))
                .ReturnsAsync(true);
            
            var actionResult = await _sut.SendEmailToSubscribers(request);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal("Emails sent successfully.", ok.Value);
        }

        [Fact]
        public async Task SendEmailToSubscribers_CallsServiceWithCorrectFromNameAndSubject()
        {
            var request = new EmailRequest
            {
                Subject      = "Big Sale",
                FromName     = "Admin",
                EmailContent = "Don't miss it!"
            };
            _mailServiceMock
                .Setup(s => s.SendEmailToSubscribersAsync(
                    It.IsAny<TextPart>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(true);
            
            await _sut.SendEmailToSubscribers(request);
            
            _mailServiceMock.Verify(
                s => s.SendEmailToSubscribersAsync(
                    It.IsAny<TextPart>(),
                    "Admin",
                    "Big Sale"),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailToSubscribers_BuildsTextPartFromEmailContent()
        {
            TextPart? capturedPart = null;
            var request = new EmailRequest
            {
                Subject      = "Test",
                FromName     = "Tester",
                EmailContent = "My email body"
            };
            _mailServiceMock
                .Setup(s => s.SendEmailToSubscribersAsync(
                    It.IsAny<TextPart>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Callback<TextPart, string, string>((part, _, _) => capturedPart = part)
                .ReturnsAsync(true);
            
            await _sut.SendEmailToSubscribers(request);
            
            Assert.NotNull(capturedPart);
            Assert.Equal("My email body", capturedPart!.Text);
        }

        [Fact]
        public async Task SendEmailToSubscribers_WhenServiceThrows_Returns500()
        {
            var request = new EmailRequest
            {
                Subject      = "Test",
                FromName     = "Tester",
                EmailContent = "Body"
            };
            _mailServiceMock
                .Setup(s => s.SendEmailToSubscribersAsync(
                    It.IsAny<TextPart>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("SMTP failure"));
            
            var actionResult = await _sut.SendEmailToSubscribers(request);
            
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(500, result.StatusCode);
        }
    }
}
