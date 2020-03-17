
namespace FFCDemoPaymentService.Messaging.Mapping
{
    public class PaymentMap : IMap
    {
        readonly MessageConfig messageConfig;

        public PaymentMap(MessageConfig messageConfig)
        {
            this.messageConfig = messageConfig;
        }

        public SqsConfig MapToSqsConfig()
        {
            return new SqsConfig(messageConfig.PaymentQueueEndpoint,
                messageConfig.PaymentQueueName,
                messageConfig.PaymentQueueUrl,
                messageConfig.DevAccessKeyId, 
                messageConfig.DevAccessKey,
                messageConfig.CreatePaymentQueue);
        }
    }
}
