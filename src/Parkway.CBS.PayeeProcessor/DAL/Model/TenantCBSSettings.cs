
namespace Parkway.CBS.PayeeProcessor.DAL.Model
{
    public class TenantCBSSettings : CBSBaseModel
    {
        public virtual int Id { get; set; }
        public virtual string Identifier { get; set; }
        public virtual string StateName { get; set; }
        public virtual int StateId { get; set; }
        public virtual int CountryId { get; set; }
        public virtual string CountryName { get; set; }
        public virtual int AddedBy_Id { get; set; }
        public virtual int LastUpdatedBy_Id { get; set; }
    }
}
