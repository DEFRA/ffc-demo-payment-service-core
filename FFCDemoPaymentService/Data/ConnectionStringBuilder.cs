using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace FFCDemoPaymentService.Data
{
    public class ConnectionStringBuilder
    {
        private readonly AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

        private readonly DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

        public ConnectionStringBuilder(string connectionString)
        {
            builder.ConnectionString = connectionString;
        }

        public async Task GetConnectionString()
        {
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://ossrdbms-aad.database.windows.net");
            builder.Add("Password", accessToken);
            System.Console.WriteLine($"CONNECTION STRING: {builder.ConnectionString}");
        }
    }
}
