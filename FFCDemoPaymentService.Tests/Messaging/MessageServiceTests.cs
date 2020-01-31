using System;
using NUnit.Framework;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using Moq;

namespace FFCDemoPaymentService.Tests
{
    [TestFixture]
    public class MessageServiceTests
    {
        Mock<IMessageAction<Schedule>> scheduleAction;
        Mock<IMessageAction<Payment>> paymentAction;
        MessageConfig messageConfig;
        Mock<IReceiver> scheduleReceiver;
        Mock<IReceiver> paymentReceiver;
        MessageService messageService;

        [SetUp]
        public void SetUp()
        {
            scheduleAction = new Mock<IMessageAction<Schedule>>();
            paymentAction = new Mock<IMessageAction<Payment>>();
            messageConfig = new MessageConfig();
            scheduleReceiver = new Mock<IReceiver>();
            paymentReceiver = new Mock<IReceiver>();

            messageService = new MessageService(scheduleAction.Object, paymentAction.Object, messageConfig, scheduleReceiver.Object, paymentReceiver.Object);
        }

        [Test]
        public void Test_StartPolling_Starts_Polling_Schedule()
        {
            messageService.StartPolling();
            scheduleReceiver.Verify(x => x.StartPolling(), Times.Once);
        }

        [Test]
        public void Test_StartPolling_Starts_Polling_Payments()
        {
            messageService.StartPolling();
            paymentReceiver.Verify(x => x.StartPolling(), Times.Once);
        }
    }
}
