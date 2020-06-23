using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Amqp;
using Amqp.Framing;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using NUnit.Framework;

namespace FFCDemoPaymentService.Tests.AMQP
{
    [TestFixture]
    public partial class AmqpReceiverTest
    {
        [Test]
        public void Test_Can_Receive()
        {
            WaitForMessageServer();

            Address address = new Address("amqp://artemis:artemis@ffc-demo-payment-artemis-queue:5672");

            Connection connection = new Connection(address);
            Session session = new Session(connection);

            var testAction = new TestAction();
            string queueName = "payment";
            var receiver = new AmqpReceiver<Payment>(session, queueName, testAction);

            Assert.AreEqual(0, testAction.messages.Count);
            SenderLink sender = new SenderLink(session, "test-sender", queueName);

            Message message = new Message();
            message.Properties = new Properties() { GroupId = queueName };
            message.BodySection = new Amqp.Framing.Data() { Binary = Encoding.UTF8.GetBytes("test message") };
            sender.Send(message);
  

            sender.Close();
            receiver.Close();
            session.Close();
            connection.Close();

            Assert.AreEqual(1, testAction.messages.Count);
        }

        private static void WaitForMessageServer()
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://ffc-demo-payment-artemis-queue:8161");
            RetryHelper.RetryOnException(5, TimeSpan.FromSeconds(10), () => request.GetResponse());
        }

        class TestAction : IMessageAction<Payment>
        {
            public List<string> messages;

            public TestAction()
            {
                this.messages = new List<string>();
            }

            public void ReceiveMessage(string message)
            {
                this.messages.Add(message);
            }
        }
    }
}