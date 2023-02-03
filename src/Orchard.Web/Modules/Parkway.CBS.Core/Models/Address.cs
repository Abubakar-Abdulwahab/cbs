namespace Parkway.CBS.Core.Models
{
    public class Address : CBSBaseModel
    {
        public virtual long Id { get; set; }
        public virtual string HouseNumber { get; set; }
        public virtual string StreetName { get; set; }
        public virtual string District { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Town { get; set; }
        public virtual string Country { get; set; }
        public virtual string PMB { get; set; }
        public virtual string CO { get; set; }
        public virtual string AdditionalInformation { get; set; }
        public virtual Applicant Applicant { get; set; }
    }
}