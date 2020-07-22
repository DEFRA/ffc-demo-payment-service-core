using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.ServiceBus.InteropExtensions;

namespace FFCDemoPaymentService.Messaging
{
    public class Receiver<T>
    {
        private readonly IMessageAction<T> action;
        private readonly int credit;
        private IQueueClient queueClient;

        public Receiver(string connectionString, string queueName, IMessageAction<T> messageAction, string endPoint, int credit = 1)
        {
            action = messageAction;
            this.credit = credit;
            CreateReceiver(connectionString, queueName, endPoint);
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public async Task CloseAsync()
        {
            await queueClient.CloseAsync();
        }

        private void CreateReceiver(string connectionString, string queueName, string endPoint)
        {
            Console.WriteLine($"Creating {queueName} receiver");
            // queueClient = new QueueClient(connectionString, queueName);
            var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
            queueClient = new QueueClient(endPoint, queueName, tokenProvider);
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = credit,
                AutoComplete = false,
            };
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var messageBody = JsonConvert.SerializeObject(message.GetBody<object>());
            Console.WriteLine("Received message");
            Console.WriteLine(messageBody);
            action.ReceiveMessage(messageBody);
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
