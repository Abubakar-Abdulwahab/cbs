using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.VM
{
    public class EscortRequestDetailsVM : PSSRequestDetailsVM
    {
        public EscortRequestVM EscortInfo { get; set; }

        public List<PoliceRankingVM> PoliceRanks { get; set; }

        public ICollection<ProposedEscortOffficerVM> AddedOfficersSelection { get; set; }

        public ICollection<ProposedEscortOffficerVM> OfficersSelection { get; set; }
        
        public ICollection<ProposedEscortOffficerVM> RemovedOfficersSelection { get; set; }

        public int NumberOfOfficers { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get { return StateLGAs != null ? StateLGAs.Where(state => state.Id == TaxEntity.SelectedState).FirstOrDefault().LGAs.ToList() : null; } }

        public List<CommandVM> ListOfCommands { get; set; }

        public int FlowDefinitionId { get; set; }
        
        public List<EscortPartialVM> Partials { get; set; }

        public ICollection<DIGTacticalSquadVM> TacticalSquadsSelection { get; set; }

        public ICollection<DIGTacticalSquadVM> AddedTacticalSquads { get; set; }

        public ICollection<DIGTacticalSquadVM> RemovedTacticalSquads { get; set; }

        public ICollection<AIGFormationVM> AddedFormations { get; set; }

        public ICollection<AIGFormationVM> FormationsSelection { get; set; }
        
        public ICollection<AIGFormationVM> RemovedFormations { get; set; }

        public long AllocationGroupId { get; set; }

        public List<EscortApprovalViewPermissions> Permissions { get; set; }

        public IEnumerable<EscortProcessStageDefinitionDTO> RequestStages { get; set; }

        public int SelectedRequestStage { get; set; }

        public int CommandTypeId { get; set; }
    }
}