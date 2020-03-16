
namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        public string ScheduleQueueName { get; set; }
        public string ScheduleQueueEndpoint { get; set; }
        public string ScheduleQueueUrl { get; set; }
        public bool CreateScheduleQueue { get; set; }
        public string PaymentQueueName { get; set; }
        public string PaymentQueueEndpoint { get; set; }
        public string PaymentQueueUrl { get; set; }
        public bool CreatePaymentQueue { get; set; }
        public string DevAccessKeyId { get; set; }
        public string DevAccessKey { get; set; }
    }
}
