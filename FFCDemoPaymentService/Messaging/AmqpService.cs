using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Listener;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("starting AMQP service");
            Address address = new Address("amqp://artemis:artemis@ffc-demo-payment-artemis-queue:5672");
            ContainerHost host = new Amqp.Listener.ContainerHost(address);
            host.Open();
            Console.WriteLine("Container host is listening on {0}:{1}", address.Host, address.Port);
            int prefetchLimit = 5;
            var paymentProcessor = new PaymentMessageProcessor(prefetchLimit);
            host.RegisterMessageProcessor("payment", paymentProcessor);
            var scheduleProcessor = new ScheduleMessageProcessor(prefetchLimit);
            host.RegisterMessageProcessor("schedule", scheduleProcessor);

            return Task.CompletedTask;
        }
    }

    public class ScheduleMessageProcessor : IMessageProcessor
    {
        public ScheduleMessageProcessor(int credit = 1)
        {
            Credit = credit;
        }

        public void Process(MessageContext messageContext)
        {
            var message = messageContext.Message;
            Console.WriteLine("schedule message body: {0}", message.Body);
            messageContext.Complete();
        }

        public int Credit { get; }
    }

    public class PaymentMessageProcessor : IMessageProcessor
    {
        public PaymentMessageProcessor(int credit = 1)
        {
            Credit = credit;
        }

        public void Process(MessageContext messageContext)
        {
            var message = messageContext.Message;
            Console.WriteLine("payment message body: {0}", message.Body);
            messageContext.Complete();
        }

        public int Credit { get; }
    }
}