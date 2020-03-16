
namespace FFCDemoPaymentService.Messaging
{
    public class SqsConfig
    {
        public SqsConfig(string endpoint, string queueName, string queueUrl, bool createQueue)
        {
            Endpoint = endpoint;
            QueueName = queueName;
            QueueUrl = queueUrl;
            CreateQueue = createQueue;
        }

        public string Endpoint { get; set; }
        public string QueueName { get; set; }
        public string QueueUrl { get; set; }
        public bool CreateQueue { get; set; }
    }
}
