using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Messaging
{
    public class SqsConnection : IConnection
    {
        private MessageConfig messageConfig;
        public SqsConnection(MessageConfig messageConfig)
        {
            this.messageConfig = messageConfig;
        }

        public async Task Listen()
        {
            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = messageConfig.ScheduleQueueEndpoint;

            var amazonSQSClient = new AmazonSQSClient(amazonSQSConfig);

            // create queue
            CreateQueueRequest createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = messageConfig.ScheduleQueueName;

            CreateQueueResponse createQueueResponse = await amazonSQSClient.CreateQueueAsync(createQueueRequest);

            // send a message
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = messageConfig.ScheduleQueueUrl; 
            sendMessageRequest.MessageBody = "Hello";

            SendMessageResponse sendMessageResponse = await amazonSQSClient.SendMessageAsync(sendMessageRequest);

            // receive a message
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = messageConfig.ScheduleQueueUrl;

            ReceiveMessageResponse receiveMessageResponse = await amazonSQSClient.ReceiveMessageAsync(receiveMessageRequest);

            // iterate all messages in queue
            for (int i = 0; i < receiveMessageResponse.Messages.Count; i++)
            {
                if (receiveMessageResponse.Messages[i].Body == "Hello")
                {
                    var receiptHandle = receiveMessageResponse.Messages[i].ReceiptHandle;

                    // delete message
                    DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();

                    deleteMessageRequest.QueueUrl = messageConfig.ScheduleQueueUrl;
                    deleteMessageRequest.ReceiptHandle = receiptHandle;

                    DeleteMessageResponse response = await amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);
                }
            }            
        }
    }
}
