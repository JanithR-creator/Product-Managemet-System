using Common.Events;
using ProductService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ProductService.Messaging
{
    public class ProductEventConsumer : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IConnection connection;
        private readonly IModel channel;

        public ProductEventConsumer(IServiceProvider serviceProvider)
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
            channel.QueueDeclare(queue: "product.restore", durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(channel);
            var restoreConsumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                Task.Run(async () =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var evt = JsonSerializer.Deserialize<ProductReserveEvent>(message);

                    if (evt != null)
                    {
                        using var scope = serviceProvider.CreateScope();
                        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                        var success = await productService.ReserveProductStockAsync(evt);

                        if (success)
                        {
                            Console.WriteLine($"[✓] Reserved product {evt.ProductId} (Qty {evt.Quantity})");
                        }
                        else
                        {
                            Console.WriteLine($"[X] Insufficient stock for product {evt.ProductId}");
                        }
                    }
                });
            };
            channel.BasicConsume(queue: "product.reserve", autoAck: true, consumer: consumer);

            restoreConsumer.Received += (model, ea) =>
            {
                Task.Run(async () =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var evt = JsonSerializer.Deserialize<ProductRestoreEvent>(message);

                    if (evt != null)
                    {
                        using var scope = serviceProvider.CreateScope();
                        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                        await productService.RestoreProductStockAsync(evt);

                        Console.WriteLine($"[✓] Restored stock for product {evt.ProductId} (Qty {evt.Quantity})");
                    }
                });
            };
            channel.BasicConsume(queue: "product.restore", autoAck: true, consumer: restoreConsumer);

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
