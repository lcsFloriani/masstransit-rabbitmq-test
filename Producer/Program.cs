using BuildingBlocks;

using MassTransit;

using RabbitMQ.Client;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    public class Program
    {
        public static async Task Main()
        {
            string routingKey = "e5e0f90e-546d-4783-ab09-98c1ec69074f";
            CancellationTokenSource source = new(TimeSpan.FromSeconds(10));

            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Send<Message>(x => x.UseRoutingKeyFormatter(ctx => ctx.Message.RoutingKey.ToString()));

                cfg.Publish<Message>(x => x.ExchangeType = ExchangeType.Direct);
            });

            _ = await busControl.StartAsync(source.Token);

            try
            {
                while (true)
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Message: ");
                        Console.Write(">> ");

                        return Console.ReadLine();
                    });

                    await busControl.Publish<Message>(new
                    {
                        Value = value,
                        RoutingKey = new Guid(routingKey)
                    });
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
