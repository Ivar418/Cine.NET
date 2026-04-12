using API.Controllers;
using API.Domain.Common;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedLibrary.DTOs.Requests;
using SharedLibrary.DTOs.Responses;
using Xunit;

namespace UnitTest.APITests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService>    _orderServiceMock;
        private readonly Mock<IOrderPdfService> _orderPdfServiceMock;
        private readonly OrdersController       _sut;

        public OrdersControllerTests()
        {
            _orderServiceMock    = new Mock<IOrderService>();
            _orderPdfServiceMock = new Mock<IOrderPdfService>();
            _sut = new OrdersController(_orderServiceMock.Object, _orderPdfServiceMock.Object);
        }
        
        private static CreateOrderRequest ValidOrderRequest() => new CreateOrderRequest
        {
            OrderType     = "Online",
            PaymentMethod = "Card",
            Tickets = new List<TicketRequest>
            {
                new TicketRequest
                {
                    ShowingId       = 1,
                    ShowDateTimeUtc = DateTimeOffset.UtcNow.AddDays(1),
                    SeatNumber      = "A1",
                    TicketType      = "Normal",
                    Price           = 12.50m
                }
            }
        };

        private static CreateOrderResponse SampleOrderResponse(int orderId = 1) =>
            new CreateOrderResponse
            {
                OrderId       = orderId,
                OrderCode     = "ABC123",
                OrderType     = "Online",
                PaymentStatus = "Pending",
                PaymentMethod = "Card",
                TotalAmount   = 12.50m,
                CreatedAtUtc  = DateTime.UtcNow,
                Tickets       = new List<CreatedOrderTicketResponse>()
            };

        // ── Create ────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_WhenServiceSucceeds_ReturnsOkWithResponse()
        {
            var request  = ValidOrderRequest();
            var response = SampleOrderResponse();
            _orderServiceMock
                .Setup(s => s.CreateAsync(request))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Success(response));
            
            var actionResult = await _sut.Create(request);
            
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Create_WhenServiceFails_ReturnsBadRequestWithError()
        {
            var request = ValidOrderRequest();
            _orderServiceMock
                .Setup(s => s.CreateAsync(request))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure("At least one ticket is required."));

            var actionResult = await _sut.Create(request);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("At least one ticket is required.", bad.Value);
        }

        [Theory]
        [InlineData("OrderType is required.")]
        [InlineData("PaymentMethod is required.")]
        [InlineData("Each ticket must have a valid ShowingId.")]
        [InlineData("Ticket price cannot be negative.")]
        public async Task Create_WhenServiceReturnsValidationError_ReturnsBadRequestWithThatMessage(string errorMessage)
        {
            var request = ValidOrderRequest();
            _orderServiceMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateOrderRequest>()))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure(errorMessage));
            
            var actionResult = await _sut.Create(request);

            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal(errorMessage, bad.Value);
        }

        // ── GetById ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenOrderExists_ReturnsOkWithOrder()
        {
            var response = SampleOrderResponse(orderId: 5);
            _orderServiceMock
                .Setup(s => s.GetByIdAsync(5))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Success(response));

            var actionResult = await _sut.GetById(5);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task GetById_WhenOrderNotFound_ReturnsNotFoundWithError()
        {
            _orderServiceMock
                .Setup(s => s.GetByIdAsync(99))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure("Order with id 99 was not found."));

            var actionResult = await _sut.GetById(99);
            
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("Order with id 99 was not found.", notFound.Value);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(1000)]
        public async Task GetById_WithVariousIds_ForwardsCorrectIdToService(int orderId)
        {
            _orderServiceMock
                .Setup(s => s.GetByIdAsync(orderId))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Success(SampleOrderResponse(orderId)));
            
            await _sut.GetById(orderId);
            
            _orderServiceMock.Verify(s => s.GetByIdAsync(orderId), Times.Once);
        }

        // ── ConfirmPayment ────────────────────────────────────────────────

        [Fact]
        public async Task ConfirmPayment_WhenSucceeds_ReturnsOkWithUpdatedOrder()
        {
            var response = SampleOrderResponse();
            response.PaymentStatus = "Paid";
            _orderServiceMock
                .Setup(s => s.ConfirmPaymentAsync(1))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Success(response));

            var actionResult = await _sut.ConfirmPayment(1);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task ConfirmPayment_WhenServiceFails_ReturnsBadRequest()
        {
            _orderServiceMock
                .Setup(s => s.ConfirmPaymentAsync(99))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure("Order with id 99 was not found."));
            
            var actionResult = await _sut.ConfirmPayment(99);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Order with id 99 was not found.", bad.Value);
        }

        [Fact]
        public async Task ConfirmPayment_WhenOrderIdIsZero_ReturnsBadRequest()
        {
            _orderServiceMock
                .Setup(s => s.ConfirmPaymentAsync(0))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0."));
            
            var actionResult = await _sut.ConfirmPayment(0);

            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        // ── GetReservationPdf ─────────────────────────────────────────────

        [Fact]
        public async Task GetReservationPdf_WhenSucceeds_ReturnsFileWithCorrectName()
        {
            var pdfBytes = new byte[] { 1, 2, 3 };
            _orderPdfServiceMock
                .Setup(s => s.GenerateReservationPdfAsync(7))
                .ReturnsAsync(ResultOf<byte[]>.Success(pdfBytes));
            
            var actionResult = await _sut.GetReservationPdf(7);
            
            var file = Assert.IsType<FileContentResult>(actionResult);
            Assert.Equal("application/pdf", file.ContentType);
            Assert.Equal("reservering-7.pdf", file.FileDownloadName);
            Assert.Equal(pdfBytes, file.FileContents);
        }

        [Fact]
        public async Task GetReservationPdf_WhenServiceFails_ReturnsBadRequest()
        {
            _orderPdfServiceMock
                .Setup(s => s.GenerateReservationPdfAsync(99))
                .ReturnsAsync(ResultOf<byte[]>.Failure("Order not found"));

            var actionResult = await _sut.GetReservationPdf(99);

            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        // ── GetTicketsPdf ─────────────────────────────────────────────────

        [Fact]
        public async Task GetTicketsPdf_WhenSucceeds_ReturnsFileWithCorrectName()
        {
            var pdfBytes = new byte[] { 4, 5, 6 };
            _orderPdfServiceMock
                .Setup(s => s.GeneratePaidTicketsPdfAsync(3))
                .ReturnsAsync(ResultOf<byte[]>.Success(pdfBytes));

            var actionResult = await _sut.GetTicketsPdf(3);

            var file = Assert.IsType<FileContentResult>(actionResult);
            Assert.Equal("application/pdf", file.ContentType);
            Assert.Equal("tickets-3.pdf", file.FileDownloadName);
            Assert.Equal(pdfBytes, file.FileContents);
        }

        [Fact]
        public async Task GetTicketsPdf_WhenServiceFails_ReturnsBadRequest()
        {
            _orderPdfServiceMock
                .Setup(s => s.GeneratePaidTicketsPdfAsync(99))
                .ReturnsAsync(ResultOf<byte[]>.Failure("Tickets not found"));

            var actionResult = await _sut.GetTicketsPdf(99);

            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task GetTicketsPdf_WhenOrderIdVaries_FileNameContainsOrderId()
        {
            _orderPdfServiceMock
                .Setup(s => s.GeneratePaidTicketsPdfAsync(42))
                .ReturnsAsync(ResultOf<byte[]>.Success(new byte[] { 0 }));

            var actionResult = await _sut.GetTicketsPdf(42);

            var file = Assert.IsType<FileContentResult>(actionResult);
            Assert.Contains("42", file.FileDownloadName);
        }

        // ── ResetToPending ────────────────────────────────────────────────

        [Fact]
        public async Task ResetToPending_WhenSucceeds_ReturnsOkWithOrder()
        {
            var response = SampleOrderResponse();
            response.PaymentStatus = "Pending";
            _orderServiceMock
                .Setup(s => s.ResetToPendingAsync(1))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Success(response));
            
            var actionResult = await _sut.ResetToPending(1);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task ResetToPending_WhenOrderNotFound_ReturnsBadRequest()
        {
            _orderServiceMock
                .Setup(s => s.ResetToPendingAsync(99))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure("Order with id 99 was not found."));

            var actionResult = await _sut.ResetToPending(99);
            
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("Order with id 99 was not found.", bad.Value);
        }

        [Fact]
        public async Task ResetToPending_WhenOrderIdIsZero_ReturnsBadRequest()
        {
            _orderServiceMock
                .Setup(s => s.ResetToPendingAsync(0))
                .ReturnsAsync(ResultOf<CreateOrderResponse>.Failure("OrderId must be greater than 0."));
            
            var actionResult = await _sut.ResetToPending(0);

            Assert.IsType<BadRequestObjectResult>(actionResult);
        }
    }
}
