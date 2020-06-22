using System;
using System.Collections.Generic;
using System.Text;
using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using NUnit.Framework;

namespace FFCDemoPaymentService.Tests.AMQP
{
    [TestFixture]
    public class AmqpReceiverTest
    {
        [Test]
        [Ignore("WIP")]
        public void Test_Can_Send()
        {
            Address address = new Address("amqp://artemis:artemis@127.0.0.1:5672");

            Console.WriteLine("Establishing a connection...");
            Connection connection = new Connection(address);

            Console.WriteLine("Creating a session...");
            Session session = new Session(connection);

            Console.WriteLine("Creating a sender link...");

            string name = "payment";
            SenderLink sender = new SenderLink(session, "sessionful-sender-link", name);

            int count = 5;
            Console.WriteLine("Sending {0} messages...", count);
            for (int i = 0; i < count; i++)
            {
                Message message = new Message();
                message.Properties = new Properties() {GroupId = name};
                message.BodySection = new Amqp.Framing.Data() {Binary = Encoding.UTF8.GetBytes("msg" + i)};
                sender.Send(message);
            }

            Console.WriteLine("Finished sending. Shutting down...");
            Console.WriteLine("");

            sender.Close();
            session.Close();
            connection.Close();
        }

        [Test]
        [Ignore("WIP")]
        public void Test_Can_Receive()
        {
            Address address = new Address("amqp://artemis:artemis@127.0.0.1:5672");

            Console.WriteLine("Establishing a connection...");
            Connection connection = new Connection(address);

            Console.WriteLine("Creating a session...");
            Session session = new Session(connection);

            string name = "payment";
            Console.WriteLine("Accepting a message session: payment");
            // Map filters = new Map();
            // filters.Add(new Symbol("com.microsoft:session-filter"), sessionId);

            ReceiverLink receiver = new ReceiverLink(
                session,
                "sessionful-receiver-link",
                new Source() {Address = name},
                null);

            MessageCallback onMessage = (link, message) =>
            {
                Console.WriteLine("Received message");
                link.Accept(message);
            };
            receiver.Start(5, onMessage);

            // int count = 5;
            // for (int i = 0; i < count; i++)
            // {
            //     Message message = receiver.Receive();
            //     if (message == null)
            //     {
            //         break;
            //     }
            //
            //     if (i == 0)
            //     {
            //         Console.WriteLine("Received message from session '{0}'", message.Properties.GroupId);
            //     }
            //
            //     receiver.Accept(message);
            // }

            Console.WriteLine("Finished receiving. Shutting down...");
            Console.WriteLine("");

            receiver.Close();
            session.Close();
            connection.Close();
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