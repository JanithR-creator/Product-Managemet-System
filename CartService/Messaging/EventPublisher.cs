using Common.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CartService.Messaging
{
    public class EventPublisher
    {
        private readonly IConfiguration config;

        public EventPublisher(IConfiguration config)
        {
            this.config = config;
        }

        public void PublishProductReserveEvent(ProductCommonEventDto @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "product.reserve", durable: true, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "product.reserve", basicProperties: null, body: body);
        }

        public void PublishProductRestoreEvent(ProductCommonEventDto @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "product.restore", durable: true, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "product.restore", basicProperties: null, body: body);
        }

        public void PublishProductUpdateEvent(ProductCommonEventUpdateDto @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "product.update", durable: true, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "product.update", basicProperties: null, body: body);
        }

    }
}
