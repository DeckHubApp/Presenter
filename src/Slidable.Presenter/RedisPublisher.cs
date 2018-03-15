using MessagePack;
using Slidable.Presenter.Models;
using StackExchange.Redis;

namespace Slidable.Presenter
{
    public class RedisPublisher
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisPublisher(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public void PublishSlideAvailable(string presenter, string slug, int number)
        {
            var m = new SlideAvailable
            {
                Presenter = presenter,
                Slug = slug,
                Number = number
            };

            _redis.GetSubscriber().Publish("slidable:slideAvailable", MessagePackSerializer.Serialize(m));
        }
    }
}