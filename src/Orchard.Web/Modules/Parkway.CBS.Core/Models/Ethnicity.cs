namespace Parkway.CBS.Core.Models
{
    public class Ethnicity : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
    }
}