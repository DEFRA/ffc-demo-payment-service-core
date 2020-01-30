using System;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Mapping;

namespace FFCDemoPaymentService.Tests.Messaging
{
    [TestFixture]
    public class ScheduleMappingTests
    {
        MessageConfig messageConfig;
        ScheduleMapping scheduleMapping;

        [Setup]
        public void Setup()
        {
            messageConfig = new MessageConfig
            {
                ScheduleQueueName = "queue",
                ScheduleQueueEndpoint = "endpoint",
                ScheduleQueueUrl = "url",
                ScheduleQueueRegion = "region",
                ScheduleAccessKeyId = "keyId",
                SchedulingAccessKey = "key",
                CreateScheduleQueue = false
            };
        }

        [Test]
        public void Test_MapToSqs_Maps_Endpoint()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().Endpoint;

            Assert.AreEqual("endpoint", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_Region()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().Region;

            Assert.AreEqual("region", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_QueueName()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().QueueName;

            Assert.AreEqual("queue", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_QueueUrl()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().QueueUrl;

            Assert.AreEqual("url", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_AccessKeyId()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().AccessKeyId;

            Assert.AreEqual("keyId", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_AccessKey()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().AccessKey;

            Assert.AreEqual("key", result);
        }

        [Test]
        public void Test_MapToSqs_Maps_CreateQueue()
        {
            scheduleMapping = new ScheduleMapping(messageConfig);

            var result = scheduleMapping.MapToSqs().CreateQueue;

            Assert.IsTrue(result);
        }
    }
}
