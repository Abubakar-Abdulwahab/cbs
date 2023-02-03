using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceRequestFlowDefinitionLevel : CBSModel
    {
        public virtual PSServiceRequestFlowDefinition Definition { get; set; }

        /// <summary>
        /// this is the position of the of this items. It indicates the order of flow
        /// </summary>
        public virtual int Position { get; set; }

        public virtual string PositionName { get; set; }

        public virtual string PositionDescription { get; set; }

        /// <summary>
        /// this indicates the direction of this flow definition
        /// it allows us know how to treat the request that has this direction
        /// <see cref="Enums.RequestDirection"/>
        /// </summary>
        public virtual int WorkFlowActionValue { get; set; }

        public virtual string ApprovalButtonName { get; set; }

        public virtual string PartialName { get; set; }

    }
}