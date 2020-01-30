using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.Runtime;

using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Messaging
{
    public class SqsReceiver
    {
        SqsConfig sqsConfig;
        BasicAWSCredentials credentials;
        AmazonSQSConfig amazonSQSConfig;
        AmazonSQSClient amazonSQSClient;
        Action<string> messageAction;

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
            amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = sqsConfig.Endpoint;
            if (!sqsConfig.CreateQueue)
            {
                amazonSQSConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(sqsConfig.Region);
            }
        }

        private void SetClient()
        {
            amazonSQSClient = new AmazonSQSClient(credentials, amazonSQSConfig);
        }

        private async Task CreateQueue()
        {
            Console.WriteLine("Creating queue {0}", sqsConfig.QueueName);
            try
            {
                CreateQueueRequest createQueueRequest = new CreateQueueRequest();
                createQueueRequest.QueueName = sqsConfig.QueueName;

                CreateQueueResponse createQueueResponse = await amazonSQSClient.CreateQueueAsync(createQueueRequest);
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
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = sqsConfig.QueueUrl;
                receiveMessageRequest.WaitTimeSeconds = 5;
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
                Console.WriteLine("Unable to process message", ex);
            }
        }

        private async Task DeleteMessage(string receiptHandle)
        {
            DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();
            deleteMessageRequest.QueueUrl = sqsConfig.QueueUrl;
            deleteMessageRequest.ReceiptHandle = receiptHandle;

            DeleteMessageResponse response = await amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);
        }
    }
}
