namespace Parkway.CBS.Police.Core.DTO
{
    public class PSServiceRequestFlowApproverDTO
    {
        public PSServiceRequestFlowDefinitionLevelDTO FlowDefinitionLevel { get; set; }

        public VM.PSSAdminUsersVM PSSAdminUser { get; set; }
    }
}