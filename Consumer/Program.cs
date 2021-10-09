using BuildingBlocks;

using MassTransit;

using RabbitMQ.Client;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    public class Program
    {
        public static async Task Main()
        {
            string routingKey = await Task.Run(() =>
            {
                Console.WriteLine("Queue ID: ");
                Console.Write(">> ");

                return Console.ReadLine();
            });

            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Send<Message>(x => x.UseRoutingKeyFormatter(ctx => ctx.Message.RoutingKey.ToString()));
                cfg.Publish<Message>(x => x.ExchangeType = ExchangeType.Direct);

                cfg.ReceiveEndpoint(routingKey, re =>
                {
                    re.Bind<Message>(c => c.RoutingKey = routingKey);

                    re.Consumer<MessageConsumer>();
                });
            });

            CancellationTokenSource source = new(TimeSpan.FromSeconds(10));

            _ = await busControl.StartAsync(source.Token);

            try
            {
                Console.WriteLine("Press enter to exit");

                _ = await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
