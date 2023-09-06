namespace FFCDemoPaymentService.Config
{
    public class AppConfig
    {
        public string Endpoint { get; set; }
        public string Id { get; set; }
        public string Secret { get; set; }
        public string ConnectionString
        {
            get => $"Endpoint=https://{Endpoint};Id={Id};Secret={Secret}";
        }
    }
}
