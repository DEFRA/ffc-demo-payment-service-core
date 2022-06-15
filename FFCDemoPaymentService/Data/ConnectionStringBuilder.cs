using System.Data.Common;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Core;
using System;

namespace FFCDemoPaymentService.Data
{
    public class PostgresConnectionStringBuilder
    {
        private readonly DbConnectionStringBuilder stringBuilder;
        private string postgresUser;
        private string postgresPassword;
        private string postgresDb;
        private string postgresHost;
        private string postgresPort;
        public bool UseCredentialChain { get; set; }

        public PostgresConnectionStringBuilder()
        {
            stringBuilder = new DbConnectionStringBuilder();
        }

        public async Task<string> GetConnectionString()
        {
            if (UseCredentialChain)
            {
                var credential = new DefaultAzureCredential();
                var accessToken = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://ossrdbms-aad.database.windows.net" }));
                stringBuilder.Add("password", accessToken.Token);
                stringBuilder.Add("sslmode", "Require");
            }
            Console.WriteLine(stringBuilder.ConnectionString);
            return stringBuilder.ConnectionString;
        }

        public string PostgresUser
        {
            get => postgresUser;
            set
            {
                postgresUser = value;
                stringBuilder.Add("username", postgresUser);
            }
        }
        public string PostgresPassword
        {
            get => postgresPassword;
            set
            {
                postgresPassword = value;
                stringBuilder.Add("password", postgresPassword);
            }
        }
        public string PostgresDb
        {
            get => postgresDb;
            set
            {
                postgresDb = value;
                stringBuilder.Add("database", postgresDb);
            }
        }
        public string PostgresHost
        {
            get => postgresHost;
            set
            {
                postgresHost = value;
                stringBuilder.Add("host", postgresHost);
            }
        }
        public string PostgresPort
        {
            get => postgresPort;
            set
            {
                postgresPort = value;
                stringBuilder.Add("port", postgresPort);
            }
        }
    }
}
