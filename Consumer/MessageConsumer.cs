using BuildingBlocks;

using MassTransit;

using System;
using System.Threading.Tasks;

namespace Consumer
{
    public class MessageConsumer : IConsumer<Message>
    {
        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Message: {context.Message.Value}");

            return Task.CompletedTask;
        }
    }
}
