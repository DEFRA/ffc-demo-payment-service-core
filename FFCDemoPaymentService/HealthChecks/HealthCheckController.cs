using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FFCDemoPaymentService.Config;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace FFCDemoPaymentService.HealthChecks
{
    [Route("[controller]")]
    [Controller]
    public class HealthCheckController : ControllerBase
    {

        private readonly MessageConfig _sbConfig;
        private readonly CacheConfig _cacheConfig;
        private readonly StorageConfig _storageConfig;
        private readonly AppConfig _appConfig;

        public HealthCheckController(CacheConfig cacheConfig, StorageConfig storageConfig, AppConfig appConfig, MessageConfig messageConfig)
        {
            _cacheConfig = cacheConfig;
            _storageConfig = storageConfig;
            _appConfig = appConfig;
            _sbConfig = messageConfig;
        }

        [Route("redis")]
        [HttpGet]
        public async Task<HealthCheckResult> CheckRedis()
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
                return HealthCheckResult.Unhealthy("Unable to connect to Redis.");
            }

        }

        [HttpGet]
        [Route("storage")]
        public async Task<HealthCheckResult> checkStorage()
        {

            try
            {
                var _blobServiceClient = new BlobServiceClient(_storageConfig.ConnectionString);
                await _blobServiceClient.CreateBlobContainerAsync(new Random().Next().ToString());

                return HealthCheckResult.Healthy("Storage Connection Successfull:");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to Storage.");
            }

        }

        [HttpGet]
        [Route("appconfig")]
        public async Task<HealthCheckResult> checkAppConfig()
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
                return HealthCheckResult.Unhealthy("Unable to connect to App Config.");
            }

        }

        [HttpGet]
        [Route("servicebus")]
        public async Task<HealthCheckResult> checkServiceBus()
        {
            try
            {
                await using var client = _sbConfig.UseCredentialChain ?
                new ServiceBusClient(_sbConfig.MessageQueueHost, new DefaultAzureCredential()) :
                new ServiceBusClient(_sbConfig.ConnectionString);

                ServiceBusSender sender = client.CreateSender(_sbConfig.ScheduleTopicName);

                string messageBody = JsonConvert.SerializeObject(new Random().Next());
                ServiceBusMessage message = new ServiceBusMessage(messageBody);
                await sender.SendMessageAsync(message);

                ServiceBusReceiver receiver = client.CreateReceiver(_sbConfig.ScheduleTopicName, _sbConfig.ScheduleSubscriptionName);
                ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
                var body = Encoding.UTF8.GetString(receivedMessage.Body);
                var paymentRecieved = JsonConvert.DeserializeObject<String>(body);
                return HealthCheckResult.Healthy("Service Bus Connection Successfull:" + paymentRecieved);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Unable to connect to Service Bus.");
            }

        }
    }
}
