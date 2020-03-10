using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Tests.Mocks;
using System;
using NUnit.Framework;
using Moq;

namespace FFCDemoPaymentService.Tests.Messaging.Actions
{
    [TestFixture]
    public class PaymentActionTests
    {
        MockServiceScopeFactory mockServiceScopeFactory;
        PaymentAction paymentAction;

        [SetUp]
        public void SetUp()
        {
            mockServiceScopeFactory = new MockServiceScopeFactory();
            paymentAction = new PaymentAction(mockServiceScopeFactory.ServiceScopeFactory.Object);
        }

        [Test]
        public void Test_DeserializeMessage_Deserializes_Valid_Payment()
        {
            var message = "{'claimId':'MINE123','value':500.47}";
            var result = paymentAction.DeserializeMessage(message);
            Assert.AreEqual("MINE123", result.ClaimId);
            Assert.AreEqual(500.47, result.Value);
        }

        [Test]
        public void Test_DeserializeMessage_Rejects_Missing_ClaimId()
        {
            var message = "{'value':500.47}";
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => paymentAction.DeserializeMessage(message));
        }

        [Test]
        public void Test_DeserializeMessage_Rejects_Missing_Value()
        {
            var message = "{'claimId':'MINE123'}";
            Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => paymentAction.DeserializeMessage(message));
        }

        [Test]
        public void Test_DeserializeMessage_Does_Not_Throw_If_Additional_Properties()
        {
            var message = "{'claimId':'MINE123','value':500.47, 'moreInfo':'extra details'}";
            var result = paymentAction.DeserializeMessage(message);
            Assert.AreEqual("MINE123", result.ClaimId);
            Assert.AreEqual(500.47, result.Value);
        }
    }
}
