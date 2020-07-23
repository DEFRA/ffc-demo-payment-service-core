using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Scheduling;
using FFCDemoPaymentService.Tests.Unit.Mocks;
using System;
using NUnit.Framework;
using Moq;

namespace FFCDemoPaymentService.Tests.Unit.Messaging.Actions
{
    [TestFixture]
    public class ScheduleActionTests
    {
        Mock<IScheduleService> scheduleService;
        MockServiceScopeFactory mockServiceScopeFactory;
        ScheduleAction scheduleAction;

        [SetUp]
        public void SetUp()
        {
            scheduleService = new Mock<IScheduleService>();

            mockServiceScopeFactory = new MockServiceScopeFactory();
            mockServiceScopeFactory.ServiceProvider.Setup(x => x.GetService(typeof(IScheduleService))).Returns(scheduleService.Object);

            scheduleAction = new ScheduleAction(mockServiceScopeFactory.ServiceScopeFactory.Object);
        }

        [Test]
        public void Test_ReceiveMessage_Calls_ScheduleService()
        {
            var message = "{'claimId':'MINE123'}";
            scheduleAction.ReceiveMessage(message);
            scheduleService.Verify(x => x.CreateSchedule(It.Is<string>(s => s == "MINE123"), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void Test_DeserializeMessage_Deserializes_Valid_Schedule()
        {
            var message = "{'claimId':'MINE123'}";
            var result = scheduleAction.DeserializeMessage(message);
            Assert.AreEqual("MINE123", result.ClaimId);
        }

        [Test]
        public void Test_DeserializeMessage_Rejects_Missing_ClaimId()
        {
            var message = "{'value':500.47}";
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => scheduleAction.DeserializeMessage(message));
        }

        [Test]
        public void Test_DeserializeMessage_Does_Not_Throw_If_Additional_Properties()
        {
            var message = "{'claimId':'MINE123','value':500.47}";
            var result = scheduleAction.DeserializeMessage(message);
            Assert.AreEqual("MINE123", result.ClaimId);
        }
    }
}
