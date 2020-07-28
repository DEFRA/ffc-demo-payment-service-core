using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace FFCDemoPaymentService.Data
{
    public class ConnectionStringBuilder
    {
        private readonly AzureServiceTokenProvider azureServiceTokenProvider;
        private readonly DbConnectionStringBuilder builder;

        public ConnectionStringBuilder(string connectionString)
        {
          azureServiceTokenProvider = new AzureServiceTokenProvider();
          builder = new DbConnectionStringBuilder
          {
            ConnectionString = connectionString
          };
        }

        public async Task<string> GetConnectionString()
        {
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://ossrdbms-aad.database.windows.net");
            builder.Add("password", accessToken);
            builder.Add("Persist Security Info", "true");
            return builder.ConnectionString;
        }
    }
}
