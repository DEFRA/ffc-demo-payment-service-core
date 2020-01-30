using System;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Mapping;

namespace FFCDemoPaymentService.Tests.Messaging
{
    [TestFixture]
    public class PaymentMappingTests
    {
        MessageConfig messageConfig;
        PaymentMapping paymentMapping;

        [Setup]
        public void Setup()
        {
            messageConfig = new MessageConfig
            {
                PaymentQueueName = "queue",
                PaymentQueueEndpoint = "endpoint",
                PaymentQueueUrl = "url",
                PaymentQueueRegion = "region",
                PaymentAccessKeyId = "keyId",
                SchedulingAccessKey = "key",
                CreatePaymentQueue = false
            };
        }

        [Test]
        public void Test_MapToSqs_Maps_Endpoint()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().Endpoint;

            Assert.AreEqual("endpoint", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_Region()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().Region;

            Assert.AreEqual("region", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_QueueName()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().QueueName;

            Assert.AreEqual("queue", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_QueueUrl()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().QueueUrl;

            Assert.AreEqual("url", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_AccessKeyId()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().AccessKeyId;

            Assert.AreEqual("keyId", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_AccessKey()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().AccessKey;

            Assert.AreEqual("key", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_CreateQueue()
        {
            paymentMapping = new PaymentMapping(messageConfig);

            var result = paymentMapping.MapToSqs().CreateQueue;

            Assert.IsTrue(result);
        }
    }
}
