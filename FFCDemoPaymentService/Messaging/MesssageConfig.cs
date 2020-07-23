
namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
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
    }
}
