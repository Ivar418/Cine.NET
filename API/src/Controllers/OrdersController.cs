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

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the order list on success,
        /// or a <c>400 Bad Request</c> response when retrieval fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request">The order payload containing reservation, customer, and ticket details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created order on success,
        /// or a <c>400 Bad Request</c> response when validation or creation fails.
        /// </returns>
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

        /// <summary>
        /// Retrieves a single order by its identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the order when found,
        /// or <c>404 Not Found</c> when the order does not exist.
        /// </returns>
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

        /// <summary>
        /// Confirms payment for an order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated order/payment state,
        /// or a <c>400 Bad Request</c> response when confirmation fails.
        /// </returns>
        [HttpPost("{orderId:int}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var result = await _orderService.ConfirmPaymentAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        /// <summary>
        /// Generates and downloads the reservation PDF for an order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// A PDF file response on success, or a <c>400 Bad Request</c> response when generation fails.
        /// </returns>
        [HttpGet("{orderId:int}/reservation-pdf")]
        public async Task<IActionResult> GetReservationPdf(int orderId)
        {
            var result = await _orderPdfService.GenerateReservationPdfAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return File(result.Value!, "application/pdf", $"reservering-{orderId}.pdf");
        }

        /// <summary>
        /// Generates and downloads the paid tickets PDF for an order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// A PDF file response on success, or a <c>400 Bad Request</c> response when generation fails.
        /// </returns>
        [HttpGet("{orderId:int}/tickets-pdf")]
        public async Task<IActionResult> GetTicketsPdf(int orderId)
        {
            var result = await _orderPdfService.GeneratePaidTicketsPdfAsync(orderId);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return File(result.Value!, "application/pdf", $"tickets-{orderId}.pdf");
        }

        /// <summary>
        /// Resets an order state back to pending.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated order on success,
        /// or a <c>400 Bad Request</c> response when the reset operation fails.
        /// </returns>
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
