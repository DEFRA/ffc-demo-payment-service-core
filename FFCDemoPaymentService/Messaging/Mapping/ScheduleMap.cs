using System;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Messaging.Mapping
{
    public class ScheduleMap : IMap
    {
        MessageConfig messageConfig;

        public ScheduleMap(MessageConfig messageConfig)
        {
            this.messageConfig = messageConfig;
        }

        public SqsConfig MapToSqsConfig()
        {
            return new SqsConfig(messageConfig.ScheduleQueueEndpoint,
                messageConfig.ScheduleQueueRegion,
                messageConfig.ScheduleQueueName,
                messageConfig.ScheduleQueueUrl,
                messageConfig.ScheduleAccessKeyId, 
                messageConfig.ScheduleAccessKey,
                messageConfig.CreateScheduleQueue);
        }
    }
}
