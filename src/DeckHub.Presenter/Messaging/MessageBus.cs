using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeckHub.Presenter.Models;
using DeckHub.Presenter.Options;

namespace DeckHub.Presenter.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly ILogger<MessageBus> _logger;
        private readonly QueueClient _startClient;
        private readonly QueueClient _slideClient;

        public MessageBus(IOptions<MessagingOptions> options, ILogger<MessageBus> logger)
        {
            _logger = logger;
            if (string.IsNullOrWhiteSpace(options.Value.ServiceBusConnectionString))
            {
                _logger.LogWarning("No ServiceBusConnectionString configured.");
                return;
            }
            _startClient = new QueueClient(options.Value.ServiceBusConnectionString, "shows/start");
            _slideClient = new QueueClient(options.Value.ServiceBusConnectionString, "shows/slide");

        }

        public async Task StartShow(StartShow show)
        {
            if (_startClient == null)
            {
                _logger.LogWarning("No ServiceBus client, message not sent.");
                return;
            }

            try
            {
                var data = MessagePackSerializer.Serialize(show);
                var message = new Message(data);
                await _startClient.SendAsync(message).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error sending ServiceBus message: {ex.Message}");
            }
        }

        public async Task SlideAvailable(string place, string presenter, string slug, int slideNumber)
        {
            if (_slideClient == null)
            {
                _logger.LogWarning("No ServiceBus client, message not sent.");
                return;
            }

            var slide = new SlideAvailable
            {
                Place = place,
                Presenter = presenter,
                Slug = slug,
                Number = slideNumber
            };

            try
            {
                var data = MessagePackSerializer.Serialize(slide);
                var message = new Message(data);
                await _slideClient.SendAsync(message).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error sending ServiceBus message: {ex.Message}");
            }

        }
    }
}