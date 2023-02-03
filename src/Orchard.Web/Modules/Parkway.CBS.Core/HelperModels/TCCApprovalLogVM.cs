using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCApprovalLogVM
    {
        public int Status { get; set; }

        public string ApprovalTime { get; set; }

        public string ApprovingAdminUserName { get; set; }

        public string Comment { get; set; }
    }
}