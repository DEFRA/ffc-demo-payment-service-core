using System;
using System.Collections.Generic;
using System.Threading;
using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using NUnit.Framework;

namespace FFCDemoPaymentService.Tests.AMQP
{
    [TestFixture]
    public class AmqpReceiverTest
    {
        TimeSpan Timeout = TimeSpan.FromMilliseconds(5000);

        [Test]
        public void Test_Can_Connect()
        {
            Address address = new Address("amqp://artemis:artemis@127.0.0.1:5672");
            ContainerHost host = new Amqp.Listener.ContainerHost(address);
            host.Open();
            Console.WriteLine("Container host is listening on {0}:{1}", address.Host, address.Port);
            int prefetchLimit = 5;
            var processor = new PaymentMessageProcessor();
            host.RegisterMessageProcessor("payment", processor);
            var connection = new Connection(address);
            var session = new Session(connection);
            string name = "payment";
            var sender = new SenderLink(session, "send-link", name);

            int count = 5;
            for (int i = 0; i < count; i++)
            {
                var message = new Message("msg" + i);
                message.Properties = new Properties() {GroupId = name};
                sender.Send(message, Timeout);
            }

            sender.Close();
            session.Close();
            connection.Close();
            Assert.AreEqual(1, 1);

            Assert.AreEqual(count, processor.Messages.Count);
        }
    }

    public class PaymentMessageProcessor : IMessageProcessor
    {
        public PaymentMessageProcessor()
            : this(20, new List<Message>())
        {
        }

        public PaymentMessageProcessor(int credit, List<Message> messages)
        {
            this.Credit = credit;
            this.Messages = messages;
        }

        public void Process(MessageContext messageContext)
        {
            if (this.Messages != null)
            {
                var message = messageContext.Message;
                Console.WriteLine("message body: {0}", message.Body);
                this.Messages.Add(messageContext.Message);
            }

            messageContext.Complete();
        }

        public int Credit { get; }


        public List<Message> Messages { get; }
    }
}