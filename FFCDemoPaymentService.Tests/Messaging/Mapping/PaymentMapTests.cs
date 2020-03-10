using NUnit.Framework;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Mapping;

namespace FFCDemoPaymentService.Tests.Messaging.Mapping
{
    [TestFixture]
    public class PaymentMapTests
    {
        MessageConfig messageConfig;
        PaymentMap paymentMap;

        [SetUp]
        public void SetUp()
        {
            messageConfig = new MessageConfig
            {
                PaymentQueueName = "queue",
                PaymentQueueEndpoint = "endpoint",
                PaymentQueueUrl = "url",
                PaymentQueueRegion = "region",
                PaymentAccessKeyId = "keyId",
                PaymentAccessKey = "key",
                CreatePaymentQueue = true
            };

            paymentMap = new PaymentMap(messageConfig);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_Endpoint()
        { 
            var result = paymentMap.MapToSqsConfig().Endpoint;

            Assert.AreEqual("endpoint", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_Region()
        {
            var result = paymentMap.MapToSqsConfig().Region;

            Assert.AreEqual("region", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_QueueName()
        {
            var result = paymentMap.MapToSqsConfig().QueueName;

            Assert.AreEqual("queue", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_QueueUrl()
        {
            var result = paymentMap.MapToSqsConfig().QueueUrl;

            Assert.AreEqual("url", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_AccessKeyId()
        {
            var result = paymentMap.MapToSqsConfig().AccessKeyId;

            Assert.AreEqual("keyId", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_AccessKey()
        {
            var result = paymentMap.MapToSqsConfig().AccessKey;

            Assert.AreEqual("key", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_CreateQueue()
        {
            var result = paymentMap.MapToSqsConfig().CreateQueue;

            Assert.IsTrue(result);
        }
    }
}
