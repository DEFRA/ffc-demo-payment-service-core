using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Telemetry;
using Azure.Messaging.ServiceBus;

namespace FFCDemoPaymentService.Messaging
{
    public class Receiver<T>
    {
        private readonly IMessageAction<T> action;
        private readonly int credit;
        private ISubscriptionClient subscriptionClient;
        private readonly ITelemetryProvider telemetryProvider;

        public Receiver(MessageConfig messageConfig, string topicName, string subscriptionName, IMessageAction<T> messageAction,
                        ITelemetryProvider telemetryProvider, int credit = 1)
        {
            action = messageAction;
            this.credit = credit;
            CreateReceiver(messageConfig, topicName, subscriptionName);
            RegisterOnMessageHandlerAndReceiveMessages();
            this.telemetryProvider = telemetryProvider;
        }

        public async Task CloseAsync()
        {
            await subscriptionClient.CloseAsync();
        }

        private void CreateReceiver(MessageConfig messageConfig, string topicName, string subscriptionName)
        {
            Console.WriteLine($"Creating {subscriptionName} receiver at {messageConfig.MessageQueueEndPoint}");

            if (messageConfig.UseTokenProvider)
            {
                Console.WriteLine($"Using token provider");
                subscriptionClient = new SubscriptionClient(messageConfig.MessageQueueEndPoint, topicName, subscriptionName, messageConfig.TokenProvider);
            }
            else
            {
                Console.WriteLine("Using connection string");
                subscriptionClient = new ServiceBusClient(messageConfig.ConnectionString);
            }
            var receiver = subscriptionClient.cr
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = credit,
                AutoComplete = false,
            };
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            telemetryProvider.TrackTrace("Trace Receiver");
            var messageBody = Encoding.UTF8.GetString(message.Body);
            try
            {
                action.ReceiveMessage(messageBody);
                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to process message {ex}");
                await subscriptionClient.AbandonAsync(message.SystemProperties.LockToken);
            }
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
