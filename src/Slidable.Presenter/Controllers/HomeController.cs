using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Slidable.Presenter.Authentication;
using Slidable.Presenter.Models;

namespace Slidable.Presenter.Controllers
{
    [Route("")]
    [Authorize]
    public class PresentController : Controller
    {
        [HttpGet("{handle}/{slug}")]
        public IActionResult PresenterView(string handle, string slug, CancellationToken ct)
        {
            var user = User.FindFirst(ShtikClaimTypes.Handle)?.Value;
            if (!handle.Equals(user, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
//            var (show, questions) = await MultiTask.WhenAll(_shows.Get(user, slug, ct), _questions.GetQuestionsForShow(user, slug, ct));
//            if (show == null)
//            {
//                return NotFound();
//            }
            var viewModel = new PresenterViewModel
            {
            };
            return View(viewModel);
        }

        [HttpPost("{handle}/start")]
        public async Task<IActionResult> Start(string handle, [FromBody] StartShow startShow, CancellationToken ct)
        {
            startShow.Presenter = handle;
            var show = await _shows.Start(startShow, ct).ConfigureAwait(false);
            return CreatedAtAction("Show", "Live", new {presenter = startShow.Presenter, slug = startShow.Slug}, show);
        }

        [HttpPut("{handle}/{slug}/{number}")]
        public async Task<IActionResult> ShowSlide(string handle, string slug, int number, CancellationToken ct)
        {
            byte[] content;
            using (var stream = new MemoryStream(65536))
            {
                await Request.Body.CopyToAsync(stream, ct);
                content = stream.ToArray();
            }
            var (ok, uri) = await MultiTask.WhenAll(
                _shows.ShowSlide(handle, slug, number, ct),
                _slides.Upload(handle, slug, number, Request.ContentType, content, ct)
            ).ConfigureAwait(false);
            // TODO: Post onto Redis message bus
//            await _hubContext.SendSlideAvailable(handle, slug, number).ConfigureAwait(false);
            if (!ok) return NotFound();
            return Accepted(uri);
        }
    }
}
