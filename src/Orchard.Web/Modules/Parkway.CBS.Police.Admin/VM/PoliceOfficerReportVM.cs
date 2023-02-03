using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.VM
{
    public class PoliceOfficerReportVM
    {
        public List<PoliceOfficerVM> Reports { get; set; }

        public int TotalActiveOfficersRecord { get; set; }

        public string OfficerName { get; set; }

        public List<CommandVM> Commands { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public List<PoliceRankingVM> Ranks { get; set; }

        public dynamic Pager { get; set; }

        public string SelectedCommand { get; set; }

        public int CommandId { get; set; }

        public string SelectedCommandName { get; set; }

        public string SelectedCommandCode { get; set; }

        public int SelectedRank { get; set; }

        public int SelectedState { get; set; }

        public int SelectedLGA { get; set; }

        public string IdNumber { get; set; }

        public string IppisNumber { get; set; }

        public string TenantName { get; set; }

        public string LogoURL { get; set; }

        public string searchParametersToken { get; set; }
    }
}