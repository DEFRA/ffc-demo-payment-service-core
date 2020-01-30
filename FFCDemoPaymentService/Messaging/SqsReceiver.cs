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
        public async Task StartPolling(string endpoint, string queueUrl, Action<string> messageAction, string keyId, string key, bool createQueue, string queueName, string region)
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(keyId, key);

            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = endpoint;


            var amazonSQSClient = new AmazonSQSClient(awsCredentials, amazonSQSConfig);

            if (createQueue)
            {
                Console.WriteLine("Creating queue {0}", queueName);
                try
                {
                    CreateQueueRequest createQueueRequest = new CreateQueueRequest();
                    createQueueRequest.QueueName = queueName;

                    CreateQueueResponse createQueueResponse = await amazonSQSClient.CreateQueueAsync(createQueueRequest);
                }
                catch
                {
                    Console.WriteLine("Can't create queue");
                }
            }

            Console.WriteLine("Ready to receive message from {0}", queueName);

            while (true)
            {
                // receive a message
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = queueUrl;
                receiveMessageRequest.WaitTimeSeconds = 5;
                ReceiveMessageResponse receiveMessageResponse = await amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count > 0)
                {
                    try
                    {
                        var receiptHandle = receiveMessageResponse.Messages[0].ReceiptHandle;

                        Console.WriteLine("Received message");
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
            }
        }
    }
}
