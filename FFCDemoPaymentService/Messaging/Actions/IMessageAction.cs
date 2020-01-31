using System;

namespace FFCDemoPaymentService.Messaging.Actions
{
    public interface IMessageAction<T>
    {
        void ReceiveMessage(string message);
    }
}
