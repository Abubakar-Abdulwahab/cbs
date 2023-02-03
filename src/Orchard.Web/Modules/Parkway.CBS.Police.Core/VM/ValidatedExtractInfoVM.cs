using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class ValidatedExtractInfoVM
    {
        public string CustomerName { get; set; }

        public int SelectedCategory { get; set; }

        public int SelectedSubCategory { get; set; }

        public string SelectedCategoryName { get; set; }

        public string SelectedSubCategoryName { get; set; }

        public string CommandStateName { get; set; }

        public string CommandLgaName { get; set; }

        public string CommandName { get; set; }

        public DateTime RequestDate { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string Reason { get; set; }
    }
}