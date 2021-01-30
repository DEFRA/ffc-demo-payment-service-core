using System;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Telemetry;
using Azure.Messaging.ServiceBus;

namespace FFCDemoPaymentService.Messaging
{
    public class Receiver<T>
    {
        private readonly IMessageAction<T> action;
        private readonly MessageConfig messageConfig;
        private readonly ITelemetryProvider telemetryProvider;

        public Receiver(MessageConfig messageConfig, IMessageAction<T> messageAction, ITelemetryProvider telemetryProvider)
        {
            action = messageAction;
            this.messageConfig = messageConfig;
            this.telemetryProvider = telemetryProvider;
        }

        public async Task ReceiveMessagesAsync(string topicName, string subscriptionName)
        {
            await using var client = messageConfig.UseCredentialChain ?
                new ServiceBusClient(messageConfig.MessageQueueEndPoint, messageConfig.Credential) :
                new ServiceBusClient(messageConfig.ConnectionString);

            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 2
            };

            await using var processor = client.CreateProcessor(topicName, subscriptionName, options);
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();

        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            telemetryProvider.TrackTrace("Trace Receiver");
            string body = args.Message.Body.ToString();
            try
            {
                action.ReceiveMessage(body);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to process message {ex}");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
