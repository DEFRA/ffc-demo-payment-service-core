using System;

namespace FFCDemoPaymentService.Messaging
{
    public class SqsConnection
    {
        private MessageConfig messageConfig;
        public SqsConnection(MessageConfig messageConfig)
        {
            this.messageConfig = messageConfig;
        }

        public void Listen()
        {
            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = messageConfig.ScheduleQueueEndpoint;

            amazonSQSClient = new AmazonSQSClient(amazonSQSConfig);

            // create queue
            CreateQueueRequest createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = messageConfig.ScheduleQueueName;
            createQueueRequest.DefaultVisibilityTimeout = 10;

            CreateQueueResponse createQueueResponse = amazonSQSClient.CreateQueue(createQueueRequest);

            // send a message
            sendMessageRequest sendMessageRequest = new sendMessageRequest();
            sendMessageRequest.QueueUrl = myQueueURL; sendMessageRequest.MessageBody = "Hello";

            SendMessageResponse sendMessageResponse = amazonSQSClient.SendMessage(sendMessageRequest);

            // receive a message
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = messageConfig.ScheduleQueueUrl;

            ReceiveMessageResponse receiveMessageResponse = amazonSQSClient.ReceiveMessage(receiveMessageRequest);

            // iterate all messages in queue
            if (result.Message.Count != 0)
            {
                for (int i = 0; i < result.Message.Count; i++)
                {
                    if (result.Message[i].Body == "Hello")
                    {
                        receiptHandle = result.Message[i].ReceiptHandle;

                        // delete message
                        DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();

                        deleteMessageRequest.QueueUrl = messageConfig.ScheduleQueueUrl;
                        deleteMessageRequest.ReceiptHandle = recieptHandle;

                        DeleteMessageResponse response = amazonSQSClient.DeleteMessage(deleteMessageRequest);
                    }
                }
            }
        }
    }
}
