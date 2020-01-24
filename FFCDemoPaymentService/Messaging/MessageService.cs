using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Amqp;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        private IConnection connection;
        private MessageConfig messageConfig;
        private IServiceScopeFactory serviceScopeFactory;

        public MessageService(IConnection connection, MessageConfig messageConfig, IServiceScopeFactory serviceScopeFactory)
        {
            this.connection = connection;
            this.messageConfig = messageConfig;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Listen();
            return Task.CompletedTask;
        }

        public void Listen()
        {
            CreateConnectionToQueue();
            var receiver = connection.GetReceiver();
            receiver.Start(20, (link, message) =>
            {
                try
                {
                    ReceiveMessage(message);
                    link.Accept(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Message Rejected: {0}", ex);
                    link.Reject(message);
                }
            });
        }

        public void CreateConnectionToQueue()
        {
            Task.Run(() =>
                connection.CreateConnectionToQueue(new BrokerUrl(messageConfig.Host, messageConfig.Port, 
                    messageConfig.PaymentUserName, messageConfig.PaymentPassword, messageConfig.UseSsl).ToString(),
                messageConfig.PaymentQueue))
            .Wait();
        }

        private void ReceiveMessage(Message message)
        {
            Console.WriteLine(message.Body);
        }
    }
}
