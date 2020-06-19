
namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        public string ScheduleQueueName { get; set; }
        public string ScheduleQueueEndpoint { get; set; }
        public string ScheduleQueueUrl
        {
            get
            {
                return GetQueueUrl(ScheduleQueueEndpoint, ScheduleQueueName, CreateScheduleQueue);
            }
        }
        public bool CreateScheduleQueue { get; set; }
        public string PaymentQueueName { get; set; }
        public string PaymentQueueEndpoint { get; set; }
        public string PaymentQueueUrl
        {
            get
            {
                return GetQueueUrl(PaymentQueueEndpoint, PaymentQueueName, CreatePaymentQueue);
            }
        }
        public bool CreatePaymentQueue { get; set; }
        public string DevAccessKeyId { get; set; }
        public string DevAccessKey { get; set; }
        public string MessageQueueHost { get; set; }
        public string MessageQueuePort { get; set; }
        public string MessageQueueUser { get; set; }
        public string MessageQueuePassword { get; set; }

        private string GetQueueUrl(string endpoint, string queueName, bool isDevelopment = false)
        {
            return isDevelopment ? string.Format("{0}/queue/{1}", endpoint, queueName) :
                    string.Format("{0}/{1}", endpoint, queueName);
        }
    }
}
