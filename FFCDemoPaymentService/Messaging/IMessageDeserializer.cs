
namespace FFCDemoPaymentService.Messaging
{
    public interface IMessageDeserializer<out T>
    {
        T DeserializeMessage(string message);
    }
}
