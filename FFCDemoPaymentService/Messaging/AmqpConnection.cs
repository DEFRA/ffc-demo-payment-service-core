using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Newtonsoft.Json;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpConnection : IConnection
    {
        
        public Task CreateConnectionToQueue(string brokerUrl, string queue)
        {
            throw new NotImplementedException();
        }

        public void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public ReceiverLink GetReceiver()
        {
            throw new NotImplementedException();
        }

        

    }
}