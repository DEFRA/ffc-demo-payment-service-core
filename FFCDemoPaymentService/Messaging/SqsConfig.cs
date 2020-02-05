
namespace FFCDemoPaymentService.Messaging
{
    public class SqsConfig
    {
        public SqsConfig(string endpoint, string region, string queueName, string queueUrl, string accessKeyId, string accessKey, bool createQueue)
        {
            Endpoint = endpoint;
            Region = region;
            QueueName = queueName;
            QueueUrl = queueUrl;
            AccessKeyId = accessKeyId;
            AccessKey = accessKey;
            CreateQueue = createQueue;
        }

        public string Endpoint { get; set; }
        public string Region { get; set; }
        public string QueueName { get; set; }
        public string QueueUrl { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKey { get; set; }
        public bool CreateQueue { get; set; }
    }
}
