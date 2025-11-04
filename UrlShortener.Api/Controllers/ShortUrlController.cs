using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly IConfiguration _config;

        public ShortUrlController(IShortUrlService shortUrlService, IConfiguration config)
        {
            _shortUrlService = shortUrlService;
            _config = config;
        }

        // ✅ POST: api/shorturl
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShortUrlRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
                return BadRequest(new { message = "OriginalUrl is required" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            var result = await _shortUrlService.CreateAsync(request.OriginalUrl, userId);

            // ✅ Base URL chính xác (tránh lỗi https://localhost:7176/api/api/)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                shortUrl = $"{baseUrl}/r/{result.Code}",
                originalUrl = result.OriginalUrl,
                createdAt = result.CreatedAt
            });
        }

        // ✅ GET: api/shorturl
        [HttpGet]
        public async Task<IActionResult> GetMyLinks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            var links = await _shortUrlService.GetByUserAsync(userId);
            return Ok(links);
        }
    }

    public class CreateShortUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
