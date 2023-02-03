using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceFormFields : CBSModel
    {
        public virtual PSService Service { get; set; }

        public virtual FormControl FormControl { get; set; }

        public virtual bool IsActive { get; set; }
    }
}