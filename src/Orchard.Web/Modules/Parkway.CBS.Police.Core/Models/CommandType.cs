using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class CommandType : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool IsVisible { get; set; }
    }
}