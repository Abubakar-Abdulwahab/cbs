using Newtonsoft.Json;
using System;

namespace Parkway.CBS.Core.Models
{
    public class BVNValidationResponse : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        [JsonProperty("phoneNumber")]
        public virtual string PhoneNumber { get; set; }

        [JsonProperty("email")]
        public virtual string EmailAddress { get; set; }

        [JsonProperty("gender")]
        public virtual string Gender { get; set; }

        [JsonProperty("phoneNumber2")]
        public virtual string PhoneNumber2 { get; set; }

        [JsonProperty("levelOfAccount")]
        public virtual string LevelOfAccount { get; set; }

        [JsonProperty("lgaOfOrigin")]
        public virtual string LgaOfOrigin { get; set; }

        [JsonProperty("lgaOfResidence")]
        public virtual string LgaOfResidence { get; set; }

        [JsonProperty("maritalStatus")]
        public virtual string MaritalStatus { get; set; }

        [JsonProperty("nin")]
        public virtual string NIN { get; set; }

        [JsonProperty("nameOnCard")]
        public virtual string NameOnCard { get; set; }

        [JsonProperty("nationality")]
        public virtual string Nationality { get; set; }

        [JsonProperty("residentialAddress")]
        public virtual string ResidentialAddress { get; set; }

        [JsonProperty("stateOfOrigin")]
        public virtual string StateOfOrigin { get; set; }

        [JsonProperty("stateOfResidence")]
        public virtual string StateOfResidence { get; set; }

        [JsonProperty("title")]
        public virtual string Title { get; set; }

        [JsonProperty("base64Image")]
        public virtual string Base64Image { get; set; }

        [JsonProperty("responseCode")]
        public virtual string ResponseCode { get; set; }

        [JsonProperty("bvn")]
        public virtual string BVN { get; set; }

        [JsonProperty("firstName")]
        public virtual string FirstName { get; set; }

        [JsonProperty("middleName")]
        public virtual string MiddleName { get; set; }

        [JsonProperty("lastName")]
        public virtual string LastName { get; set; }

        [JsonProperty("dateOfBirth")]
        public virtual string DateOfBirth { get; set; }

        [JsonProperty("registrationDate")]
        public virtual string RegistrationDate { get; set; }

        [JsonProperty("enrollmentBank")]
        public virtual string EnrollmentBank { get; set; }

        [JsonProperty("enrollmentBranch")]
        public virtual string EnrollmentBranch { get; set; }

        [JsonProperty("watchListed")]
        public virtual string WatchListed { get; set; }

        [JsonProperty("requestId")]
        public virtual string RequestId { get; set; }

        [JsonProperty("responseDescription")]
        public virtual string ResponseDescription { get; set; }

        [JsonProperty("userId")]
        public virtual string UserId { get; set; }
        public virtual string ResponseDump { get; set; }
    }
}