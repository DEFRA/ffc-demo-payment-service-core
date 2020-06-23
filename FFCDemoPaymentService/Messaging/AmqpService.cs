using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpService : BackgroundService
    {
        private readonly IMessageAction<Schedule> scheduleAction;
        private readonly IMessageAction<Payment> paymentAction;
        private readonly string paymentQueue;
        private readonly string scheduleQueue;
        private readonly Address address;

        private AmqpReceiver<Payment> paymentReceiver;
        private AmqpReceiver<Schedule> scheduleReceiver;
        private Session session;
        private Connection connection;
        private readonly int credits;

        public AmqpService(
            MessageConfig messageConfig,
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction)
        {
            this.paymentQueue = messageConfig.PaymentQueueName;
            this.scheduleQueue = messageConfig.ScheduleQueueName;
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            this.credits = Int32.Parse(messageConfig.MessageQueuePreFetch);


            this.address = new Address(
                messageConfig.MessageQueueHost,
                Int32.Parse(messageConfig.MessageQueuePort),
                messageConfig.MessageQueueUser,
                messageConfig.MessageQueuePassword,
                "/",
                messageConfig.MessageQueuePort == "5672" ? "AMQP" : "AMQPS"
                );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("opening connection...");
            connection = new Connection(address);
            Console.WriteLine("creating session...");
            session = new Session(connection);
            paymentReceiver = new AmqpReceiver<Payment>(session, paymentQueue, paymentAction, credits);
            scheduleReceiver = new AmqpReceiver<Schedule>(session, scheduleQueue, scheduleAction, credits);

            return Task.CompletedTask;
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