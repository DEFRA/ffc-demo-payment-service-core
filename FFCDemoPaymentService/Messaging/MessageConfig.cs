using Microsoft.Azure.ServiceBus.Primitives;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        private TokenProvider tokenProvider;
        public bool UseTokenProvider { get; set; }
        public string ScheduleQueueName { get; set; }
        public string PaymentQueueName { get; set; }
        public string MessageQueueHost { get; set; }
        public string MessageQueuePreFetch { get; set; }
        public string MessageQueueUser { get; set; }
        public string MessageQueuePassword { get; set; }
        public string MessageQueueEndPoint
        {
            get
            {
                return $"sb://{MessageQueueHost}/";
            }
        }
        public string ConnectionString
        {
            get
            {
                return $"Endpoint={MessageQueueEndPoint};SharedAccessKeyName={MessageQueueUser};SharedAccessKey={MessageQueuePassword}";
            }
        }
        public TokenProvider TokenProvider {
            get
            {
                return tokenProvider ??= TokenProvider.CreateManagedIdentityTokenProvider();
            }
        }
    }
}
