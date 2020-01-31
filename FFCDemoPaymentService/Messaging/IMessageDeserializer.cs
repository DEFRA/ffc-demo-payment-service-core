using System;

namespace FFCDemoPaymentService.Messaging
{
    public interface IMessageDeserializer<T>
    {
        T DeserializeMessage(string message);
    }
}
