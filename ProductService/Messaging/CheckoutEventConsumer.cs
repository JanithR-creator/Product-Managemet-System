using Common.Events;
using ProductService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ProductService.Messaging
{
    public class CheckoutEventConsumer : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IConnection connection;
        private readonly IModel channel;

        public CheckoutEventConsumer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                Port = 5672
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            //declare queues
            channel.QueueDeclare(queue: "product.reserve", durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                Task.Run(async () =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var evt = JsonSerializer.Deserialize<ProductCommonEventDto>(message);

                    if (evt != null)
                    {
                        using var scope = serviceProvider.CreateScope();
                        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                        foreach (var product in evt.ProductQuantities)
                        {
                            var productId = product.Key;
                            var quantity = product.Value;

                            var success = await productService.ReserveProductStockAsync(productId, quantity);

                            if (!string.IsNullOrEmpty(ea.BasicProperties?.ReplyTo))
                            {
                                var replyProps = channel.CreateBasicProperties();
                                replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                                var responseMessage = Encoding.UTF8.GetBytes(success.ToString());

                                channel.BasicPublish(
                                    exchange: "",
                                    routingKey: ea.BasicProperties.ReplyTo,
                                    basicProperties: replyProps,
                                    body: responseMessage);
                            }

                            if (success)
                            {
                                Console.WriteLine($"[✓] Reserved product {productId} (Qty {quantity})");
                            }
                            else
                            {
                                Console.WriteLine($"[X] Insufficient stock for product {productId}");
                            }
                        }
                    }
                });
            };
            channel.BasicConsume(queue: "product.reserve", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel.Close();
            connection.Close();
            base.Dispose();
        }
    }
}
