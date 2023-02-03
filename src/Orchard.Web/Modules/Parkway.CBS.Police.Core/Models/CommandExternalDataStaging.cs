using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class CommandExternalDataStaging : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual int CommandCategoryId { get; set; }

        public virtual string LGACode { get; set; }

        public virtual string StateCode { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual int AddedBy { get; set; }

        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual int LastUpdatedBy { get; set; }

        public virtual string Address { get; set; }

        public virtual int CommandTypeId { get; set; }

        public virtual int CallLogForExternalSystemId { get; set; }

        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }

        public virtual string ParentCode { get; set; }

        public virtual string ZonalCode { get; set; }
    }
}