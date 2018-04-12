using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Slidable.Presenter.Authentication;
using Slidable.Presenter.Messaging;
using Slidable.Presenter.Models;

namespace Slidable.Presenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageBus _messageBus;
        private readonly IApiKeyProvider _apiKeyProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IMessageBus messageBus, IApiKeyProvider apiKeyProvider, ILogger<HomeController> logger)
        {
            _messageBus = messageBus;
            _apiKeyProvider = apiKeyProvider;
            _logger = logger;
        }

        [HttpGet("{place}/{presenter}/{slug}")]
        [Authorize]
        public IActionResult PresenterView(string place, string presenter, string slug, CancellationToken ct)
        {
            if (!IsCurrentUser(presenter))
            {
                return NotFound();
            }

            var model = new PresenterViewModel
            {
                Place = place,
                Presenter = presenter,
                Slug = slug
            };
            return View(model);
        }

        [HttpPost("{presenter}/start")]
        public async Task<IActionResult> Start(string presenter, [FromBody] StartShow startShow, CancellationToken ct)
        {
            if (!ValidateApiKey(presenter))
            {
                return NotFound();
            }
            startShow.Presenter = presenter;
            await _messageBus.StartShow(startShow);
            return StatusCode(201);
        }

        [HttpPut("{place}/{presenter}/{slug}/{number}")]
        public async Task<IActionResult> ShowSlide(string place, string presenter, string slug, int number, CancellationToken ct)
        {
            if (!ValidateApiKey(presenter))
            {
                return NotFound();
            }

            await _messageBus.SlideAvailable(place, presenter, slug, number);
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
