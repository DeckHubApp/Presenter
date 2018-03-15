using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Slidable.Presenter.Models;
using Slidable.Presenter.Options;

namespace Slidable.Presenter.Clients
{
    public class ShowsClient : IShowsClient
    {
        private readonly HttpClient _http;

        public ShowsClient(IOptions<ServiceOptions> options)
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(options.Value.Shows.BaseUrl)
            };
        }

        public async Task<Show> Start(StartShow startShow, CancellationToken ct = default)
        {
            var show = new Show
            {
                Place = startShow.Place,
                Presenter = startShow.Presenter,
                Slug = startShow.Slug,
                Time = startShow.Time,
                HighestSlideShown = 0
            };
            var response = await _http.PostJsonAsync("/shows/start", show, ct: ct).ConfigureAwait(false);
            return await response.Deserialize<Show>();
        }

        public async Task ShowSlide(string presenter, string slug, int number, CancellationToken ct)
        {
            var content = new StringContent("");
            await _http.PutAsync($"/shows/{presenter}/{slug}?highestSlideShown={number}", content, ct);
        }

        public async Task<Show> GetLatest(string presenter, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/shows/find/by/{presenter}/latest", ct).ConfigureAwait(false);
            return await response.Deserialize<Show>();
        }

        public async Task<Show> Get(string presenter, string slug, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/shows/{presenter}/{slug}", ct).ConfigureAwait(false);
            return await response.Deserialize<Show>();
        }

        public async Task<IList<Show>> FindByTag(string tag, CancellationToken ct = default)
        {
            var response = await _http.GetAsync($"/shows/find-by-tag/{tag}", ct).ConfigureAwait(false);
            return await response.Deserialize<Show[]>();
        }
    }
}