using System.Text.Json;

namespace FFCDemoPaymentService.Models
{
    public class Claim
    {
        [JsonProperty(PropertyName = "claimId", Required = Required.Always)]
        public string ClaimId { get; set; }
    }
}
