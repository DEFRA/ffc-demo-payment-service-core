using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;

namespace FFCDemoPaymentService.Messaging
{
    public interface IConnection    
    {
        Task CreateConnectionToQueue(string brokerUrl, string queue);

        void CloseConnection();

        ReceiverLink GetReceiver();               
    }
}

