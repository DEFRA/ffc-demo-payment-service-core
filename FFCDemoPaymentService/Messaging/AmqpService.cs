using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpService : BackgroundService
    {
        readonly IMessageAction<Schedule> scheduleAction;
        readonly IMessageAction<Payment> paymentAction;

        private ReceiverLink paymentReceiver;
        private ReceiverLink scheduleReceiver;
        private Session session;
        private Connection connection;

        public AmqpService(
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction)
        {
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string paymentQueue = "payment";
            string scheduleQueue = "schedule";
            string amqpAddress = "amqp://artemis:artemis@ffc-demo-payment-artemis-queue:5672";
            Address address = new Address(amqpAddress);
            Console.WriteLine("Establishing a connection...");
            connection = new Connection(address);
            Console.WriteLine("Creating a session...");
            session = new Session(connection);

            paymentReceiver = CreateReceiver(paymentQueue, paymentAction);
            scheduleReceiver = CreateReceiver(scheduleQueue, scheduleAction);

            return Task.CompletedTask;
        }

        private ReceiverLink CreateReceiver<T>(string queueName, IMessageAction<T> messageAction)
        {
            Console.WriteLine($"Creating a {queueName} receiver");
            var receiver = new ReceiverLink(
                session,
                $"{queueName}Receiver",
                new Source() {Address = queueName},
                null);

            MessageCallback onMessage = (link, message) =>
            {
                // the below extension method will not resolve for some reason, so having to use ToString.
                // var messageBody = message.GetBody<string>();
                string messageBody = message.Body.ToString();
                Console.WriteLine("Received message");
                Console.WriteLine(messageBody);
                messageAction.ReceiveMessage(messageBody);
                link.Accept(message);
            };
            receiver.Start(5, onMessage);
            return receiver;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("closing paymentReceiver...");
            paymentReceiver.Close();
            Console.WriteLine("closing scheduleReceiver...");
            scheduleReceiver.Close();
            Console.WriteLine("closing session...");
            session.Close();
            Console.WriteLine("closing connection...");
            connection.Close();
            return Task.CompletedTask;
        }
    }
}