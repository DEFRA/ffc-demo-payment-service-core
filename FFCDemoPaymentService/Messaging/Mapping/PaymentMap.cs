
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
                messageConfig.PaymentQueueRegion,
                messageConfig.PaymentQueueName,
                messageConfig.PaymentQueueUrl,
                messageConfig.PaymentAccessKeyId, 
                messageConfig.PaymentAccessKey,
                messageConfig.CreatePaymentQueue);
        }
    }
}
