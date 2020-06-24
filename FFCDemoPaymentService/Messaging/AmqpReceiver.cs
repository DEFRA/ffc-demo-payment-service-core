using System;
using Amqp;
using Amqp.Framing;
using FFCDemoPaymentService.Messaging.Actions;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpReceiver<T>
    {
        private readonly IMessageAction<T> action;
        private ReceiverLink receiver;

        public AmqpReceiver(Session session, string queueName, IMessageAction<T> messageAction, int credit = 1)
        {
            this.action = messageAction;
            CreateReceiver(session, queueName);
            receiver.Start(credit, OnMessage);
        }

        public void Close()
        {
            receiver.Close();
        }

        private void CreateReceiver(Session session, string queueName)
        {
            Console.WriteLine($"Creating {queueName} receiver");
            this.receiver = new ReceiverLink(
                session,
                $"{queueName}Receiver",
                new Source() {Address = queueName},
                null);
        }


        private void OnMessage(IReceiverLink link, Message message)
        {
            string messageBody = message.Body.ToString();
            Console.WriteLine("Received message");
            Console.WriteLine(messageBody);
            action.ReceiveMessage(messageBody);
            link.Accept(message);
        }
    }
}
