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

        public void PublishProductReserveEvent(Guid productId, int quantity)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "product.reserve", durable: true, exclusive: false, autoDelete: false);

            var evt = new ProductReserveEvent { ProductId = productId, Quantity = quantity };
            var message = JsonSerializer.Serialize(evt);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "product.reserve", basicProperties: null, body: body);
        }
    }
}
