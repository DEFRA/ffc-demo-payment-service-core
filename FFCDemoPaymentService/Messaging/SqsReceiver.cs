using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.Runtime;
using System.Threading.Tasks;
using Amazon.Runtime.CredentialManagement;

namespace FFCDemoPaymentService.Messaging
{
    public class SqsReceiver : IReceiver
    {
        readonly SqsConfig sqsConfig;
        BasicAWSCredentials credentials;
        AmazonSQSConfig amazonSQSConfig;
        AmazonSQSClient amazonSQSClient;
        readonly Action<string> messageAction;

        public SqsReceiver(SqsConfig sqsConfig, Action<string> messageAction)
        {
            this.sqsConfig = sqsConfig;
            this.messageAction = messageAction;
        }

        public void StartPolling()
        {
            SetCredentials();
            SetConfiguration();
            SetClient();

            if (sqsConfig.CreateQueue)
            {
                Task.Run(() => CreateQueue()).Wait();
            }

            Start();
        }

        private void SetCredentials()
        {
            credentials = new BasicAWSCredentials(sqsConfig.AccessKeyId, sqsConfig.AccessKey);
        }

        private void SetConfiguration()
        {
            amazonSQSConfig = new AmazonSQSConfig()
            {
                ServiceURL = sqsConfig.Endpoint
            };

            if (!sqsConfig.CreateQueue)
            {
                amazonSQSConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(sqsConfig.Region);
            }
        }

        private void SetClient()
        {
            amazonSQSClient = new AmazonSQSClient(amazonSQSConfig);
        }

        private async Task CreateQueue()
        {
            Console.WriteLine("Creating queue: {0}", sqsConfig.QueueName);
            try
            {
                CreateQueueRequest createQueueRequest = new CreateQueueRequest(sqsConfig.QueueName);

                await amazonSQSClient.CreateQueueAsync(createQueueRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to create queue: {0}", ex);
            }
        }

        private void Start()
        {
            Console.WriteLine("Ready to receive message from {0}", sqsConfig.QueueName);
            Task.Run(() => ListenForMessages());
        }

        private async Task ListenForMessages()
        {
            while (true)
            {
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(sqsConfig.QueueUrl)
                {
                    WaitTimeSeconds = 5
                };

                ReceiveMessageResponse receiveMessageResponse = await amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count > 0)
                {
                    await ReceiveMessages(receiveMessageResponse);
                }
            }
        }

        private async Task ReceiveMessages(ReceiveMessageResponse receiveMessageResponse)
        {
            try
            {
                Console.WriteLine("Received message: {0}", receiveMessageResponse.Messages[0].Body);
                var receiptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;
                messageAction(receiveMessageResponse.Messages[0].Body);
                await DeleteMessage(receiptHandle);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to process message: {0}", ex);
                throw;
            }
        }

        private async Task DeleteMessage(string receiptHandle)
        {
            DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest()
            {
                QueueUrl = sqsConfig.QueueUrl,
                ReceiptHandle = receiptHandle
            };

            try
            {
                await amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to delete message: {0}", ex);
                throw;
            }
        }
    }
}
