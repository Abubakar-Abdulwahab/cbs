using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class FormSetupViewModel
    {
        public IEnumerable<FormControl> DatabaseControlsForCorprate { get; set; }
        public IEnumerable<FormControl> DatabaseControlsForIndvidual { get; set; }

        public IEnumerable<FormControlRevenueHeadMetaDataExtended> ControlsPerEntity { get; set; }
        public FormControl Form { get; set; }
        public string RevenueHeadName { get; set; }
        public bool Checked { get; set; }
        public ControlTypes ControlType { get; set; }
    }
}