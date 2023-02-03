using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class ApprovalLogVM
    {
        public Int64 Id { get; set; }

        public int Status { get; set; }

        public string ApprovalTime { get; set; }

        public string ApprovingAdminUserName { get; set; }

        public string Comment { get; set; }
    }
}