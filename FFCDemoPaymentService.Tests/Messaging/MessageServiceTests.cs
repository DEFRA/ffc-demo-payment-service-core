using System;
using NUnit.Framework;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Tests
{
    [TestFixture]
    public class MessageServiceTests
    {
        MessageConfig messageConfig;
        //MessageService messageService;

        [SetUp]
        public void SetUp()
        {
            messageConfig = new MessageConfig
            {
                ScheduleQueueEndpoint = "scheduleEndpoint",
                ScheduleQueueName = "schedule",
                ScheduleQueueUrl = "scheduleUrl",
                ScheduleQueueRegion = "scheduleRegion",
                ScheduleAccessKeyId = "scheduleId",
                ScheduleAccessKey = "scheduleKey",
                CreateScheduleQueue = true,
                PaymentQueueEndpoint = "paymentEndpoint",
                PaymentQueueName = "payment",
                PaymentQueueUrl = "paymentUrl",
                PaymentQueueRegion = "paymentRegion",
                PaymentAccessKeyId = "paymentId",
                PaymentAccessKey = "paymentKey",
                CreatePaymentQueue = false
            };

            //messageService = new MessageService(messageConfig);
        }

        // [Test]
        // public void Test_ExecuteAsync_Starts_Polling()
        // {

        // }

        
    }
}
