using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace FFCDemoPaymentService.Data
{
    public class PostgresConnectionStringBuilder
    {
        private readonly AzureServiceTokenProvider tokenProvider;
        private readonly DbConnectionStringBuilder stringBuilder;
        private string postgresUser;
        private string postgresPassword;
        private string postgresDb;
        private string postgresHost;
        private string postgresPort;
        public bool UseTokenProvider { get; set; }

        public PostgresConnectionStringBuilder()
        {
            tokenProvider = new AzureServiceTokenProvider();
            stringBuilder = new DbConnectionStringBuilder();
        }

        public async Task<string> GetConnectionString()
        {
            System.Console.WriteLine("CONNECTION STRING 1:");
            System.Console.WriteLine(stringBuilder.ConnectionString);

            if (UseTokenProvider) {
                string accessToken = await tokenProvider.GetAccessTokenAsync("https://ossrdbms-aad.database.windows.net");
                stringBuilder.Add("password", accessToken);
            }

            System.Console.WriteLine("CONNECTION STRING 2:");
            System.Console.WriteLine(stringBuilder.ConnectionString);
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
