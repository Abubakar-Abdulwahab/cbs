using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSServiceRequestFlowDefinitionLevelDTO
    {
        public int Id { get; set; }

        public int Position { get; set; }

        public string PositionName { get; set; }

        public string  PositionDescription { get; set; }

        public RequestDirection RequestDirectionValue { get; set; }

        public string ApprovalButtonName { get; set; }

        public string DefinitionName { get; set; }

        public int DefinitionId { get; set; }

        public int ServiceId { get; set; }
    }
}