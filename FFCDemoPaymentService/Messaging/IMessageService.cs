using System;

namespace FFCDemoPaymentService.Messaging
{
    public interface IMessageService
    {
        void Listen();
        void CreateConnectionToQueue();
    }    
}