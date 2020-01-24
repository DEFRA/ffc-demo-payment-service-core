using System;

namespace FFCDemoPaymentService.Messaging
{
    public class BrokerUrl
    {
        private string host;
        private int port;
        private string username;
        private string password;
        private bool useSsl;

        public BrokerUrl(string host, int port, string username, string password, bool useSsl = false)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
            this.useSsl = useSsl;
        }

        public override string ToString()
        {
            return string.Format("amqp{0}://{1}:{2}@{3}:{4}", useSsl ? "s" : string.Empty, username, password, host, port);
        }
    }
}
