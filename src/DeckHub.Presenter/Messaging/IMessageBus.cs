using System.Threading.Tasks;
using DeckHub.Presenter.Models;

namespace DeckHub.Presenter.Messaging
{
    public interface IMessageBus
    {
        Task StartShow(StartShow show);
        Task SlideAvailable(string place, string presenter, string slug, int slideNumber);
    }
}