using Microsoft.Azure.ServiceBus.Primitives;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        private TokenProvider tokenProvider;
        public bool UseTokenProvider { get; set; }
        public string ScheduleTopicName { get; set; }
        public string PaymentTopicName { get; set; }
        public string ScheduleSubscriptionName { get; set; }
        public string PaymentSubscriptionName { get; set; }
        public string MessageQueueHost { get; set; }
        public string MessageQueuePreFetch { get; set; }
        public string MessageQueueUser { get; set; }
        public string MessageQueuePassword { get; set; }
        public string MessageQueueEndPoint
        {
            get => $"sb://{MessageQueueHost}/";
        }
        public string ConnectionString
        {
            get => $"Endpoint={MessageQueueEndPoint};SharedAccessKeyName={MessageQueueUser};SharedAccessKey={MessageQueuePassword}";
        }
        public TokenProvider TokenProvider {
            get => tokenProvider ??= TokenProvider.CreateManagedIdentityTokenProvider();
        }
    }
}
