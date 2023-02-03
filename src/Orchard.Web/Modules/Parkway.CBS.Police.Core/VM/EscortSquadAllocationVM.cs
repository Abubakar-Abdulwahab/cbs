using Parkway.CBS.Police.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class EscortSquadAllocationVM
    {
        public Int64 Id { get; set; }

        public CommandVM Command { get; set; }

        public int NumberOfOfficers { get; set; }

        public string StatusDescription { get; set; }

        public Int64 EscortSquadAllocationGroupId { get; set; }
    }
}