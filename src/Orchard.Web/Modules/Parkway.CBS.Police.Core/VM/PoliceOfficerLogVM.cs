using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceOfficerLogVM
    {
        public Int64 Id { get; set; }

        public string Name { get; set; }

        public Int64 RankId { get; set; }

        public string RankName { get; set; }

        public int CommandId { get; set; }

        public string CommandName { get; set; }

        public string IdentificationNumber { get; set; }

        public string RankCode { get; set; }
    }
}