
namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        public string ScheduleQueueName { get; set; }        
        public string ScheduleQueueEndpoint { get; set; }
        public string ScheduleQueueUrl { get; set; }
        public string ScheduleQueueRegion { get; set; }
        public string ScheduleAccessKeyId { get; set; }
        public string ScheduleAccessKey { get; set; }
        public bool CreateScheduleQueue { get; set; }
        public string PaymentQueueName { get; set; }
        public string PaymentQueueEndpoint { get; set; }
        public string PaymentQueueUrl { get; set; }
        public string PaymentQueueRegion { get; set; }
        public string PaymentAccessKeyId { get; set; }
        public string PaymentAccessKey { get; set; }
        public bool CreatePaymentQueue { get; set; }
    }
}
