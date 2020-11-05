using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Telemetry;
using Microsoft.Azure.ServiceBus;

namespace FFCDemoPaymentService.Messaging
{
    public class Receiver<T>
    {
        private readonly IMessageAction<T> action;
        private readonly int credit;
        private IQueueClient queueClient;
        private readonly ITelemetryProvider telemetryProvider;

        public Receiver(MessageConfig messageConfig, string queueName, IMessageAction<T> messageAction,
                        ITelemetryProvider telemetryProvider, int credit = 1)
        {
            action = messageAction;
            this.credit = credit;
            CreateReceiver(messageConfig, queueName);
            RegisterOnMessageHandlerAndReceiveMessages();
            this.telemetryProvider = telemetryProvider;
        }

        public async Task CloseAsync()
        {
            await queueClient.CloseAsync();
        }

        private void CreateReceiver(MessageConfig messageConfig, string queueName)
        {
            Console.WriteLine($"Creating {queueName} receiver at {messageConfig.MessageQueueEndPoint}");

            if (messageConfig.UseTokenProvider)
            {
                Console.WriteLine($"Using Token Provider");
                queueClient = new QueueClient(messageConfig.MessageQueueEndPoint, queueName, messageConfig.TokenProvider);
            }
            else
            {
                Console.WriteLine("Using Connection String");
                queueClient = new QueueClient(messageConfig.ConnectionString, queueName);
            }
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
            telemetryProvider.TrackTrace("Trace Receiver");
            var messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine("Received message");
            Console.WriteLine(messageBody);
            action.ReceiveMessage(messageBody);
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            telemetryProvider.TrackException(new Exception($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}."));
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
