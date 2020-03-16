
namespace FFCDemoPaymentService.Messaging.Mapping
{
    public class ScheduleMap : IMap
    {
        readonly MessageConfig messageConfig;

        public ScheduleMap(MessageConfig messageConfig)
        {
            this.messageConfig = messageConfig;
        }

        public SqsConfig MapToSqsConfig()
        {
            return new SqsConfig(messageConfig.ScheduleQueueEndpoint,                
                messageConfig.ScheduleQueueName,
                messageConfig.ScheduleQueueUrl,
                messageConfig.DevAccessKeyId, 
                messageConfig.DevAccessKey,
                messageConfig.CreateScheduleQueue);
        }
    }
}
