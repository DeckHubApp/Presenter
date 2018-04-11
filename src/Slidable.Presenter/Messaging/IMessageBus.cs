using System.Threading.Tasks;
using Slidable.Presenter.Models;

namespace Slidable.Presenter.Messaging
{
    public interface IMessageBus
    {
        Task StartShow(StartShow show);
        Task SlideAvailable(string place, string presenter, string slug, int slideNumber);
    }
}