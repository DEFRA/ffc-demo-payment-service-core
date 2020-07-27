using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace FFCDemoPaymentService.Data
{
    public class ConnectionStringBuilder
    {
        private AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

        private DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

        public ConnectionStringBuilder()
        {
            // builder.Add("Password")
            // System.Console.WriteLine($"CONNECTION STRING: {builder.ConnectionString}");
        }

        public async Task TestTokenRetrieve()
        {
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://ossrdbms-aad.database.windows.net");
            System.Console.WriteLine($"TOKEN TEST: {accessToken}");
        }

    }
}
