using NUnit.Framework;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Mapping;

namespace FFCDemoPaymentService.Tests.Messaging.Mapping
{
    [TestFixture]
    public class ScheduleMapTests
    {
        MessageConfig messageConfig;
        ScheduleMap scheduleMap;

        [SetUp]
        public void SetUp()
        {
            messageConfig = new MessageConfig
            {
                ScheduleQueueName = "queue",
                ScheduleQueueEndpoint = "endpoint",
                ScheduleQueueUrl = "url",
                CreateScheduleQueue = true
            };

            scheduleMap = new ScheduleMap(messageConfig);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_Endpoint()
        {
             var result = scheduleMap.MapToSqsConfig().Endpoint;

            Assert.AreEqual("endpoint", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_QueueName()
        {
            var result = scheduleMap.MapToSqsConfig().QueueName;

            Assert.AreEqual("queue", result);
        }

        [Test]
        public void Test_MapToSqsConfig_Maps_QueueUrl()
        {
            var result = scheduleMap.MapToSqsConfig().QueueUrl;

            Assert.AreEqual("url", result);
        }
        
        [Test]
        public void Test_MapToSqsConfig_Maps_CreateQueue()
        {
            var result = scheduleMap.MapToSqsConfig().CreateQueue;

            Assert.IsTrue(result);
        }
    }
}
