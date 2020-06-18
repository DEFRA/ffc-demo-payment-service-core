using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Address address = new Address("amqp://artemis:artemis@ffc-demo-payment-artemis-queue:5672");

            Console.WriteLine("Establishing a connection...");
            Connection connection = new Connection(address);

            Console.WriteLine("Creating a session...");
            Session session = new Session(connection);

            string name = "payment";
            Console.WriteLine("Accepting a message session: payment");
            // Map filters = new Map();
            // filters.Add(new Symbol("com.microsoft:session-filter"), sessionId);

            ReceiverLink receiver = new ReceiverLink(
                session,
                "sessionful-receiver-link",
                new Source() { Address = name },
                null);

            MessageCallback onMessage = (link, message) =>
            {
                Console.WriteLine("Received message");
                link.Accept(message);
            };
            receiver.Start(5, onMessage);

            return Task.CompletedTask;
        }
    }

}