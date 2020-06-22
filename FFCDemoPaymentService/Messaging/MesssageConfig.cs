
namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        public string ScheduleQueueName { get; set; }
        public string PaymentQueueName { get; set; }
        public string MessageQueueHost { get; set; }
        public string MessageQueuePort { get; set; }
        public string MessageQueueUser { get; set; }
        public string MessageQueuePassword { get; set; }
    }
}
