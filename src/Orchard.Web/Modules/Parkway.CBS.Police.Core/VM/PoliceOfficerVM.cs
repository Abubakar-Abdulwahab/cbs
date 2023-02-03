using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceOfficerVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Int64 RankId { get; set; }

        public string RankName { get; set; }

        public string RankCode { get; set; }

        public string IdNumber { get; set; }
        
        public string IppisNumber { get; set; }

        public int CommandId { get; set; }

        public int CommandCategoryId { get; set; }

        public int SubCommandId { get; set; }

        public string SubCommandCode { get; set; }

        public int SubSubCommandId { get; set; }

        public string CommandName { get; set; }

        public string CommandCode { get; set; }

        public string AccountNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public Int64 PoliceOfficerLogId { get; set; }
    }
}