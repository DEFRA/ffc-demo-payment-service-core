using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCDemoPaymentService.Models
{
    public class Claim
    {
        [JsonPropertyName( "claimId")]
        [JsonRequired]
        public string ClaimId { get; set; }
    }
}
