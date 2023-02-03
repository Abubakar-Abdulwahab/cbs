using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class FormSetupEditModel
    {
        public IEnumerable<FormControlsRevenueHeadVM> RevenueHeadControlsForIndividual { get; set; }
        public IEnumerable<FormControlsRevenueHeadVM> RevenueHeadControlsForCorporate { get; set; }

        public IEnumerable<FormControlRevenueHeadMetaDataExtended> ControlsPerEntity { get; set; }

        public string RevenueHeadName { get; set; }
        public string Slug { get; set; }
        public int Id { get; set; }
    }

    public class FormControlsRevenueHeadVM : FormControl
    {
        public Boolean Required { get; set; }
        public bool Editable { get; set; }
    }
}