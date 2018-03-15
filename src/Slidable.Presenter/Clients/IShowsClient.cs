using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Slidable.Presenter.Models;

namespace Slidable.Presenter.Clients
{
    public interface IShowsClient
    {
        Task<Show> Get(string presenter, string slug, CancellationToken ct = default);
        Task<Show> GetLatest(string presenter, CancellationToken ct = default);
        Task<IList<Show>> FindByTag(string tag, CancellationToken ct = default);
        Task<Show> Start(StartShow startShow, CancellationToken ct = default);
        Task ShowSlide(string presenter, string slug, int number, CancellationToken ct);
    }
}