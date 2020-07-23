using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.ServiceBus.Primitives;

namespace FFCDemoPaymentService.Messaging
{
    public class Receiver<T>
    {
        private readonly IMessageAction<T> action;
        private readonly int credit;
        private IQueueClient queueClient;

        public Receiver(TokenProvider tokenProvider, string queueEndPoint, string queueName, IMessageAction<T> messageAction, int credit = 1)
        {
            action = messageAction;
            this.credit = credit;
            CreateReceiver(tokenProvider, queueEndPoint, queueName);
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public async Task CloseAsync()
        {
            await queueClient.CloseAsync();
        }

        private void CreateReceiver(TokenProvider tokenProvider, string queueEndPoint, string queueName)
        {
            Console.WriteLine($"Creating {queueName} receiver at {queueEndPoint}");
            queueClient = new QueueClient(queueEndPoint, queueName, tokenProvider);
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
