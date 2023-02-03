using Newtonsoft.Json;

namespace Parkway.CBS.Core.HelperModels
{
    public class NINValidationImplResponseModel
    {
        [JsonProperty("batchid")]
        public string BatchId { get; set; }

        [JsonProperty("birthcountry")]
        public string BirthCountry { get; set; }

        [JsonProperty("birthdate")]
        public string BirthDate { get; set; }

        [JsonProperty("birthlga")]
        public string BirthLga { get; set; }

        [JsonProperty("birthstate")]
        public string BirthState { get; set; }

        [JsonProperty("cardstatus")]
        public string CardStatus { get; set; }

        [JsonProperty("centralID")]
        public string CentralID { get; set; }

        [JsonProperty("documentno")]
        public string DocumentNo { get; set; }

        [JsonProperty("educationallevel")]
        public string EducationalLevel { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("emplymentstatus")]
        public string EmploymentStatus { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("heigth")]
        public string Height { get; set; }

        [JsonProperty("maritalstatus")]
        public string MaritalStatus { get; set; }

        [JsonProperty("middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("nin")]
        public string NIN { get; set; }

        [JsonProperty("nok_address1")]
        public string NextOfKinAddress1 { get; set; }

        [JsonProperty("nok_address2")]
        public string NextOfKinAddress2 { get; set; }

        [JsonProperty("nok_firstname")]
        public string NextOfKinFirstName { get; set; }

        [JsonProperty("nok_lga")]
        public string NextOfKinLGA { get; set; }

        [JsonProperty("nok_middlename")]
        public string NextOfKinMiddleName { get; set; }

        [JsonProperty("nok_state")]
        public string NextOfKinState { get; set; }

        [JsonProperty("nok_surname")]
        public string NextOfKinSurname { get; set; }

        [JsonProperty("nok_town")]
        public string NextOfKinTown { get; set; }

        [JsonProperty("nspokenlang")]
        public string NativeSpokenLang { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }

        [JsonProperty("profession")]
        public string Profession { get; set; }

        [JsonProperty("religion")]
        public string Religion { get; set; }

        [JsonProperty("residence_AdressLine1")]
        public string ResidenceAdressLine1 { get; set; }

        [JsonProperty("residence_Town")]
        public string ResidenceTown { get; set; }

        [JsonProperty("residence_lga")]
        public string ResidenceLGA { get; set; }

        [JsonProperty("residence_state")]
        public string ResidenceState { get; set; }

        [JsonProperty("residencestatus")]
        public string ResidenceStatus { get; set; }

        [JsonProperty("self_origin_lga")]
        public string SelfOriginLGA { get; set; }

        [JsonProperty("self_origin_place")]
        public string SelfOriginPlace { get; set; }

        [JsonProperty("self_origin_state")]
        public string SelfOriginState { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("telephoneno")]
        public string TelephoneNo { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("trackingId")]
        public string TrackingId { get; set; }
    }
}