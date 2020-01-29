using System;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace FFCDemoPaymentService.Messaging
{
    public class SqsSender
    {
        public async Task SendMessage(MessageConfig messageConfig)
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(messageConfig.ScheduleAccessKeyId, messageConfig.ScheduleAccessKey);

            AmazonSQSConfig amazonSQSConfig = new AmazonSQSConfig();
            amazonSQSConfig.ServiceURL = messageConfig.ScheduleQueueEndpoint;

            var amazonSQSClient = new AmazonSQSClient(awsCredentials, amazonSQSConfig);

            try
            {
                // create queue
                CreateQueueRequest createQueueRequest = new CreateQueueRequest();
                createQueueRequest.QueueName = messageConfig.ScheduleQueueName;

                CreateQueueResponse createQueueResponse = await amazonSQSClient.CreateQueueAsync(createQueueRequest);
            }
            catch
            {
                Console.WriteLine("Can't create queue");
            }
            // send a message
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = messageConfig.ScheduleQueueUrl;
            sendMessageRequest.MessageBody = "Hello";

            SendMessageResponse sendMessageResponse = await amazonSQSClient.SendMessageAsync(sendMessageRequest);
        }
    }
}
