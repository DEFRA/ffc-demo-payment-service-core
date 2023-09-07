using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Config;
using FFCDemoPaymentService.Messaging;
using StackExchange.Redis;
using Azure.Storage.Blobs;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace FFCDemoPaymentService.HealthChecks
{

    public class ReadinessCheck : IHealthCheck
    {
        private readonly ApplicationDbContext db;

        private readonly MessageConfig _sbConfig;
        private readonly CacheConfig _cacheConfig;
        private readonly StorageConfig _storageConfig;
        private readonly AppConfig _appConfig;

        public ReadinessCheck(ApplicationDbContext db, CacheConfig cacheConfig, StorageConfig storageConfig, AppConfig appConfig, MessageConfig messageConfig)
        {
            this.db = db;
            _cacheConfig = cacheConfig;
            _storageConfig = storageConfig;
            _appConfig = appConfig;
            _sbConfig = messageConfig;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            HealthCheckResult postgresdb = await CheckDatabase(db);
            HealthCheckResult redis = await CheckRedis();
            HealthCheckResult storage = await checkStorage();
            HealthCheckResult appConfig = await checkAppConfig();
            HealthCheckResult serviceBus = await checkServiceBus();

            if (postgresdb.Status.ToString() != "Healthy")
            {
                return HealthCheckResult.Unhealthy("Unhealthy Postgress DB", exception: postgresdb.Exception);
            }
            if (redis.Status.ToString() != "Healthy")
            {
                return HealthCheckResult.Unhealthy("Unhealthy Redis", exception: redis.Exception);
            }
            if (storage.Status.ToString() != "Healthy")
            {
                return HealthCheckResult.Unhealthy("Unhealthy Storage Account", exception: storage.Exception);
            }
            if (appConfig.Status.ToString() != "Healthy")
            {
                return HealthCheckResult.Unhealthy("Unhealthy App Config", exception: appConfig.Exception);
            }
            if (serviceBus.Status.ToString() != "Healthy")
            {
                return HealthCheckResult.Unhealthy("Unhealthy Service Bus", exception: serviceBus.Exception);
            }
            return HealthCheckResult.Healthy("A healthy result.");
        }

        private async Task<HealthCheckResult> CheckDatabase(ApplicationDbContext dbContext)
        {
            try
            {
                await dbContext.Database.CanConnectAsync();
                return HealthCheckResult.Healthy("DB Connection healthy");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to DB.", exception: ex);
            }
        }
        private async Task<HealthCheckResult> CheckRedis()
        {
            try
            {
                var _cache = ConnectionMultiplexer.Connect(_cacheConfig.ConnectionString).GetDatabase();
                var pingResult = await _cache.ExecuteAsync("PING");

                await _cache.StringSetAsync("testkey", new Random().Next().ToString());
                await _cache.StringGetAsync("testkey");

                return HealthCheckResult.Healthy("Redis Connection PING:" + ((RedisValue)pingResult));
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to Redis.", exception: ex);
            }

        }
        private async Task<HealthCheckResult> checkStorage()
        {
            // Upload and retrieve files or data from Azure Storage
            // ...
            try
            {
                var _blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);
                var client = _blobServiceClient.GetBlobContainerClient("test");
                if (!client.Exists())
                {
                    await _blobServiceClient.CreateBlobContainerAsync("test");
                }
                await client.CreateIfNotExistsAsync();
                return HealthCheckResult.Healthy("Storage Connection Successfull:");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to Storage.", exception: ex);
            }

        }

        private async Task<HealthCheckResult> checkAppConfig()
        {

            try
            {
                var _appConfigClient = new ConfigurationClient(_appConfig.ConnectionString);
                var setting = new ConfigurationSetting("test_key", new Random().Next().ToString());
                await _appConfigClient.SetConfigurationSettingAsync(setting);
                ConfigurationSetting retrievedSetting = await _appConfigClient.GetConfigurationSettingAsync("test_key");
                Console.WriteLine($"The value of the configuration setting is: {retrievedSetting.Value}");

                return HealthCheckResult.Healthy("App Config Connection Successfull:" + retrievedSetting.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to App Config.", exception: ex);
            }

        }
        private async Task<HealthCheckResult> checkServiceBus()
        {
            try
            {
                await using var client = _sbConfig.UseCredentialChain ?
                new ServiceBusClient(_sbConfig.MessageQueueHost, new DefaultAzureCredential()) :
                new ServiceBusClient(_sbConfig.ConnectionString);

                ServiceBusSender sender = client.CreateSender(_sbConfig.ScheduleTopicName);
                ServiceBusMessage message = new ServiceBusMessage(new Random().Next().ToString());
                await sender.SendMessageAsync(message);

                ServiceBusReceiver receiver = client.CreateReceiver(_sbConfig.ScheduleTopicName, _sbConfig.ScheduleSubscriptionName);
                ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
                string body = receivedMessage.Body.ToString();

                return HealthCheckResult.Healthy("Service Bus Connection Successfull:" + body);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to Service Bus.", exception: ex);
            }

        }
    }
}
