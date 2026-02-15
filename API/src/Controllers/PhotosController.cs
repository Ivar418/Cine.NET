using Microsoft.AspNetCore.Mvc;
using WebApi_PocV1.DTOs.Requests;
using WebApi_PocV1.DTOs.Responses;
using WebApi_PocV1.Storage;

namespace WebApi_PocV1.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoStorage _storage;

        public PhotosController(IPhotoStorage storage)
        {
            _storage = storage;
        }

        [HttpPost]
        [Route("new")]
        [RequestSizeLimit(25_000_000)] // 25 MB (also configure server limits)
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string creatorId,
            [FromForm] string? folder = null, [FromForm] Boolean overwrite = false,
            CancellationToken ct = default)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            // Basic content-type check (do not rely on this alone)
            var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg", "image/png", "image/webp", "image/gif"
            };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest(new { error = "Unsupported image type." });

            // Limit to specific folders
            var allowedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "movie", "profile" };
            if (!allowedFolders.Contains(folder) || string.IsNullOrWhiteSpace(folder))
                return BadRequest(new { error = "Unsupported folder." });
            // Optional: limit by extension too
            var ext = Path.GetExtension(file.FileName);
            var allowedExt = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (string.IsNullOrWhiteSpace(ext) || !allowedExt.Contains(ext))
                return BadRequest(new { error = "Unsupported file extension." });

            // Save
            //TODO move to a service layer
            //TODO check if creatorId actually is an existing movie or user
            await using var stream = file.OpenReadStream();
            var result = await _storage.SaveAsync(
                stream: stream,
                contentType: file.ContentType,
                originalFileName: file.FileName,
                folder: folder,
                creatorId: creatorId,
                ct: ct
            );

            // result could include: id, url, key/path, size, etc.
            return Created(result.Url, result);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPhoto([FromBody] PhotoRequest request)
        {
            if (request.CreatorId is null)
                return BadRequest(new { error = "Missing creatorId." });
            if (request.PhotoType is null)
                return BadRequest(new { error = "Missing photoType." });
 
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _storage.GetByCreatorAndType(request.CreatorId, request.PhotoType);
            if (result is null)
                return NotFound();

            var response = new PhotoResponse
            {
                Id = result.Id,
                Url = result.Url,
                Size = result.Size,
                ContentType = result.ContentType,
                creatorId = result.creatorId
            };

            return Ok(response);
        }
    }
}