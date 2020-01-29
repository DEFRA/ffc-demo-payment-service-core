using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Messaging
{
    public class SqsReceiver
    {
        public async Task StartPolling(string endpoint, string queueUrl, Action<string> messageAction, string keyId, string key, bool createQueue, string QueueName)
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(keyId, key);

            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = endpoint;

            var amazonSQSClient = new AmazonSQSClient(awsCredentials, amazonSQSConfig);

            if (createQueue)
            {
                CreateQueueRequest createQueueRequest = new CreateQueueRequest();
                createQueueRequest.QueueName = QueueName;

                CreateQueueResponse createQueueResponse = await amazonSQSClient.CreateQueueAsync(createQueueRequest);
            }

            // receive a message
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = queueUrl;

            // while (true)
            // {
                ReceiveMessageResponse receiveMessageResponse = await amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count > 0)
                {
                    var receiptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;

                    try
                    {
                        messageAction(receiveMessageResponse.Messages[0].Body);

                        // delete message
                        DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();

                        deleteMessageRequest.QueueUrl = queueUrl;
                        deleteMessageRequest.ReceiptHandle = receiptHandle;

                        DeleteMessageResponse response = await amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unable to process message", ex);
                    }
                }

            // }
        }
    }
}
