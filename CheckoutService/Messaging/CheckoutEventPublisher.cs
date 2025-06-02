using Common.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CheckoutService.Messaging
{
    public class CheckoutEventPublisher
    {
        private readonly IConfiguration config;

        public CheckoutEventPublisher(IConfiguration config)
        {
            this.config = config;
        }
        public void PublishCheckoutEvent(CheckoutEventDto @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "checkout.publish", durable: true, exclusive: false, autoDelete: false);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "checkout.publish", basicProperties: null, body: body);
        }
    }
}
