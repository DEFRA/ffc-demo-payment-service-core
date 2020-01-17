using System;

namespace FFCDemoPaymentService.Messaging
{
    public class BrokerUrl
    {
        private string host;
        private int port;
        private string username;
        private string password;

        public BrokerUrl(string host, int port, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public override string ToString()
        {
            return string.Format("amqp://{0}:{1}@{2}:{3}", username, password, host, port);
        }
    }
}