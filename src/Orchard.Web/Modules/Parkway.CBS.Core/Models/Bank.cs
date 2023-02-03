namespace Parkway.CBS.Core.Models
{
    public class Bank : CBSBaseModel
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; }

        public virtual string Code { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}