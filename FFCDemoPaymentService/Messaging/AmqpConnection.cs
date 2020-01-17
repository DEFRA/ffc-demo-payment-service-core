using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Newtonsoft.Json;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpConnection : IConnection
    {
        protected ConnectionFactory connectionFactory;
        protected Address address;
        protected Connection connection;
        protected Session session;
        protected ReceiverLink receiverLink;

        public async Task CreateConnectionToQueue(string brokerUrl, string queue)
        {
            if (connectionFactory == null)
            {
                ConfigureConnectionFactory();
            }
            address = new Address(brokerUrl);
            connection = await connectionFactory.CreateAsync(address);
            session = new Session(connection);
            receiverLink = new ReceiverLink(session, "receiver", queue);
        }

        private void ConfigureConnectionFactory()
        {
            connectionFactory = new ConnectionFactory();
            connectionFactory.AMQP.ContainerId = "Payment-Service-Core-Container";
        }

        public void CloseConnection()
        {
            if(receiverLink != null)
            {
                receiverLink.Close();
            }
            if(session != null)
            {
                session.Close();
            }
            if(connection != null)
            {
                connection.Close();
            }
        }

        public ReceiverLink GetReceiver()
        {
           return receiverLink;
        }
    }
}