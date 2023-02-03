using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class CommandCategory : CBSBaseModel
    {
        public virtual CommandCategory ParentCommandCategory { get; set; }

        public virtual int ParentCategory { get; set; }

        public virtual int LeftIndex { get; set; }

        public virtual int RightIndex { get; set; }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSCommandCategoryLevel"/>
        /// </summary>
        public virtual int CategoryLevel { get; set; }
    }
}