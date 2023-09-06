namespace FFCDemoPaymentService.Config
{
    public class StorageConfig
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }  
        public string ConnectionString
        {
            get => $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey};EndpointSuffix=core.windows.net";
        }
    }
}
