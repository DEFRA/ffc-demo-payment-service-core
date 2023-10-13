using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCDemoPaymentService.Models
{
    public class Claim
    {
        [JsonPropertyName( "claimId")]
        public string ClaimId { get; set; }
    }
}
