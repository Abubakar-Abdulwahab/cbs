using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class FourCoreBVNValidationResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public FourCoreBVNValidationResponseData Data { get; set; }
    }

    public class FourCoreBVNValidationResponseData
    {
        [JsonProperty("bvn")]
        public FourCoreBVNValidationResponseBVN BVN { get; set; }
    }

    public class FourCoreBVNValidationResponseBVN
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("middle_name")]
        public string MiddleName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("alt_number")]
        public string AltNumber { get; set; }

        [JsonProperty("bvn")]
        public string BVN { get; set; }
    }
}