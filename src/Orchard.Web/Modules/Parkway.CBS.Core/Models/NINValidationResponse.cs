using Newtonsoft.Json;
using System;

namespace Parkway.CBS.Core.Models
{
    public class NINValidationResponse : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        [JsonProperty("batchid")]
        public virtual string BatchId { get; set; }

        [JsonProperty("birthcountry")]
        public virtual string BirthCountry { get; set; }

        [JsonProperty("birthdate")]
        public virtual string BirthDate { get; set; }

        [JsonProperty("birthlga")]
        public virtual string BirthLga { get; set; }

        [JsonProperty("birthstate")]
        public virtual string BirthState { get; set; }

        [JsonProperty("cardstatus")]
        public virtual string CardStatus { get; set; }

        [JsonProperty("centralID")]
        public virtual string CentralID { get; set; }

        [JsonProperty("documentno")]
        public virtual string DocumentNo { get; set; }

        [JsonProperty("educationallevel")]
        public virtual string EducationalLevel { get; set; }

        [JsonProperty("email")]
        public virtual string Email { get; set; }

        [JsonProperty("emplymentstatus")]
        public virtual string EmploymentStatus { get; set; }

        [JsonProperty("firstname")]
        public virtual string FirstName { get; set; }

        [JsonProperty("gender")]
        public virtual string Gender { get; set; }

        [JsonProperty("heigth")]
        public virtual string Height { get; set; }

        [JsonProperty("maritalstatus")]
        public virtual string MaritalStatus { get; set; }

        [JsonProperty("middlename")]
        public virtual string MiddleName { get; set; }

        [JsonProperty("nin")]
        public virtual string NIN { get; set; }

        [JsonProperty("nok_address1")]
        public virtual string NextOfKinAddress1 { get; set; }

        [JsonProperty("nok_address2")]
        public virtual string NextOfKinAddress2 { get; set; }

        [JsonProperty("nok_firstname")]
        public virtual string NextOfKinFirstName { get; set; }

        [JsonProperty("nok_lga")]
        public virtual string NextOfKinLGA { get; set; }

        [JsonProperty("nok_middlename")]
        public virtual string NextOfKinMiddleName { get; set; }

        [JsonProperty("nok_state")]
        public virtual string NextOfKinState { get; set; }

        [JsonProperty("nok_surname")]
        public virtual string NextOfKinSurname { get; set; }

        [JsonProperty("nok_town")]
        public virtual string NextOfKinTown { get; set; }

        [JsonProperty("nspokenlang")]
        public virtual string NativeSpokenLang { get; set; }

        [JsonProperty("photo")]
        public virtual string Photo { get; set; }

        [JsonProperty("profession")]
        public virtual string Profession { get; set; }

        [JsonProperty("religion")]
        public virtual string Religion { get; set; }

        [JsonProperty("residence_AdressLine1")]
        public virtual string ResidenceAdressLine1 { get; set; }

        [JsonProperty("residence_Town")]
        public virtual string ResidenceTown { get; set; }

        [JsonProperty("residence_lga")]
        public virtual string ResidenceLGA { get; set; }

        [JsonProperty("residence_state")]
        public virtual string ResidenceState { get; set; }

        [JsonProperty("residencestatus")]
        public virtual string ResidenceStatus { get; set; }

        [JsonProperty("self_origin_lga")]
        public virtual string SelfOriginLGA { get; set; }

        [JsonProperty("self_origin_place")]
        public virtual string SelfOriginPlace { get; set; }

        [JsonProperty("self_origin_state")]
        public virtual string SelfOriginState { get; set; }

        [JsonProperty("signature")]
        public virtual string Signature { get; set; }

        [JsonProperty("surname")]
        public virtual string Surname { get; set; }

        [JsonProperty("telephoneno")]
        public virtual string TelephoneNo { get; set; }

        [JsonProperty("title")]
        public virtual string Title { get; set; }

        [JsonProperty("trackingId")]
        public virtual string TrackingId { get; set; }
        public virtual string ResponseDump { get; set; }
    }
}