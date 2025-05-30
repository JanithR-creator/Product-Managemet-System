using Common.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CartService.Messaging
{
    public class CartEventPublisher
    {
        private readonly IConfiguration config;

        public CartEventPublisher(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<bool> PublishProductReserveEventAsync(ProductCommonEventDto @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the request queue and response queue
            channel.QueueDeclare(queue: "product.reserve", durable: true, exclusive: false, autoDelete: false);
            var replyQueueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            var tcs = new TaskCompletionSource<bool>();
            var correlationId = Guid.NewGuid().ToString();

            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties?.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    bool success = bool.Parse(response);
                    tcs.SetResult(success);
                }
            };

            channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = correlationId;

            channel.BasicPublish(exchange: "", routingKey: "product.reserve", basicProperties: props, body: body);

            // Wait for response from consumer
            return await tcs.Task;
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
