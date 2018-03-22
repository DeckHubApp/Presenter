using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Slidable.Presenter.Authentication;
using Slidable.Presenter.Clients;
using Slidable.Presenter.Models;

namespace Slidable.Presenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShowsClient _shows;
        private readonly RedisPublisher _redis;
        private readonly IApiKeyProvider _apiKeyProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IShowsClient shows, RedisPublisher redis, IApiKeyProvider apiKeyProvider, ILogger<HomeController> logger)
        {
            _shows = shows;
            _redis = redis;
            _apiKeyProvider = apiKeyProvider;
            _logger = logger;
        }

        [HttpGet("{handle}/{slug}")]
        [Authorize]
        public IActionResult PresenterView(string handle, string slug, CancellationToken ct)
        {
            if (!IsCurrentUser(handle))
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost("{handle}/start")]
        public async Task<IActionResult> Start(string handle, [FromBody] StartShow startShow, CancellationToken ct)
        {
            if (!ValidateApiKey(handle))
            {
                return NotFound();
            }
            startShow.Presenter = handle;
            await _shows.Start(startShow, ct).ConfigureAwait(false);
            return StatusCode(201);
        }

        [HttpPut("{handle}/{slug}/{number}")]
        public async Task<IActionResult> ShowSlide(string handle, string slug, int number, CancellationToken ct)
        {
            if (!ValidateApiKey(handle))
            {
                return NotFound();
            }
            await _shows.ShowSlide(handle, slug, number, ct);
            _redis.PublishSlideAvailable(handle, slug, number);
            return Accepted();
        }

        private bool IsCurrentUser(string handle)
        {
            var user = User.FindFirst(SlidableClaimTypes.Handle)?.Value;
            return handle.Equals(user, StringComparison.OrdinalIgnoreCase);
        }

        private bool ValidateApiKey(string handle)
        {
            var apiKey = Request.Headers["API-Key"];

            if (_apiKeyProvider.CheckBase64(handle, apiKey)) return true;

            _logger.LogWarning(EventIds.PresenterInvalidApiKey, "Invalid API key.");
            return false;
        }
    }
}
