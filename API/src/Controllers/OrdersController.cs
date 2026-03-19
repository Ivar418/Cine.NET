using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs.Requests;

namespace API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderPdfService _orderPdfService;

        public OrdersController(IOrderService orderService, IOrderPdfService orderPdfService)
        {
            _orderService = orderService;
            _orderPdfService = orderPdfService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            var result = await _orderService.CreateAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var result = await _orderService.GetByIdAsync(orderId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("{orderId:int}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var result = await _orderService.ConfirmPaymentAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{orderId:int}/reservation-pdf")]
        public async Task<IActionResult> GetReservationPdf(int orderId)
        {
            var result = await _orderPdfService.GenerateReservationPdfAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return File(result.Value!, "application/pdf", $"reservering-{orderId}.pdf");
        }

        [HttpGet("{orderId:int}/tickets-pdf")]
        public async Task<IActionResult> GetTicketsPdf(int orderId)
        {
            var result = await _orderPdfService.GeneratePaidTicketsPdfAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return File(result.Value!, "application/pdf", $"tickets-{orderId}.pdf");
        }

        [HttpPost("{orderId:int}/reset-to-pending")]
        public async Task<IActionResult> ResetToPending(int orderId)
        {
            var result = await _orderService.ResetToPendingAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
