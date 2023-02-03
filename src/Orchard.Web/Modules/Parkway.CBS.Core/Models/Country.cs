namespace Parkway.CBS.Core.Models
{
    public class Country : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual bool IsActive { get; set; }
    }
}