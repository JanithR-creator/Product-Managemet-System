using CartService.Services;
using Common.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CartService.Messaging
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

            channel.QueueDeclare(queue: "checkout.publish", durable: true, exclusive: false, autoDelete: false);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var checkoutConsumer = new EventingBasicConsumer(channel);

            checkoutConsumer.Received += (model, ea) =>
            {
                Task.Run(async () =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var evt = JsonSerializer.Deserialize<CheckoutEventDto>(message);

                    if (evt != null)
                    {
                        using var scope = serviceProvider.CreateScope();
                        var checkoutService = scope.ServiceProvider.GetRequiredService<ICartService>();

                        var success = await checkoutService.Checkout(evt);

                        if (success)
                        {
                            Console.WriteLine($"[✓] Checkout {evt.UserId}");
                        }
                        else
                        {
                            Console.WriteLine($"[X] Checkout Cancelled {evt.UserId}");
                        }
                    }
                });
            };
            channel.BasicConsume(queue: "checkout.publish", autoAck: true, consumer: checkoutConsumer);

            return Task.CompletedTask;
        }
    }
}
