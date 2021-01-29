using Azure.Identity;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        private DefaultAzureCredential credential;
        public bool UseCredentialChain { get; set; }
        public string ScheduleTopicName { get; set; }
        public string PaymentTopicName { get; set; }
        public string ScheduleSubscriptionName { get; set; }
        public string PaymentSubscriptionName { get; set; }
        public string MessageQueueHost { get; set; }
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
        public DefaultAzureCredential Credential {
            get => credential ??= new DefaultAzureCredential();
        }
    }
}
