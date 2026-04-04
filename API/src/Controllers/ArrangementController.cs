using Microsoft.AspNetCore.Mvc;
using API.Mappers;
using API.Services.Interfaces;
using SharedLibrary.Domain.Entities;
using SharedLibrary.DTOs.Requests;

namespace API.Controllers
{
    [ApiController]
    [Route("api/arrangements")]
    public class ArrangementsController : ControllerBase
    {
        private readonly IArrangementService _service;

        public ArrangementsController(IArrangementService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all arrangements.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of arrangements when successful,
        /// or an error response when the operation fails.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return result switch
            {
                { IsFailure: true } => StatusCode(500, new { error = "An error occurred" }),
                { IsSuccess: true } => Ok(ArrangementMapper.ToResponses(result.Value!)),
                _ => StatusCode(500, new { error = "Unexpected result" })
            };
        }

        /// <summary>
        /// Retrieves a single arrangement by its identifier.
        /// </summary>
        /// <param name="id">The arrangement identifier.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the arrangement when found,
        /// or <c>404 Not Found</c> when it does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result.IsFailure || result.Value == null)
                return NotFound();

            return Ok(ArrangementMapper.ToResponse(result.Value));
        }

        /// <summary>
        /// Creates a new arrangement.
        /// </summary>
        /// <param name="request">The arrangement data to create, including arrangement items.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created arrangement,
        /// or an error response when creation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArrangementRequest request)
        {
            var arrangement = new Arrangement
            {
                Name = request.Name,
                Price = request.Price,
                IsActive = request.IsActive,
                Items = request.Items.Select(i => new ArrangementItem
                {
                    Type = (ArrangementItemType)i.Type,
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList()
            };

            var result = await _service.CreateAsync(arrangement);

            if (result.IsFailure)
                return StatusCode(500, new { error = result.Error });

            return Ok(ArrangementMapper.ToResponse(result.Value!));
        }
    }
}