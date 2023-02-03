using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceRequestFlowDefinition : CBSModel
    {
        public virtual string DefinitionName { get; set; }

        public virtual string DefinitionDescription { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// this indicates the type of this flow definition
        /// <see cref="Enums.DefinitionType"/>
        /// </summary>
        public virtual int DefinitionType { get; set; }

        public virtual IEnumerable<PSServiceRequestFlowDefinitionLevel> LevelDefinitions { get; set; }
    }
}