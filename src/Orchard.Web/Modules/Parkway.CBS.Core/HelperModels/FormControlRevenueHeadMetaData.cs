using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class FormControlRevenueHeadMetaData
    {

        public int TaxEntityCategoryId { get; set; }
        public string TaxEntityCategoryName { get; set; }
        public List<int> FormControlIds { get; set; }
    }

    public class FormControlRevenueHeadMetaDataExtended : FormControlRevenueHeadMetaData
    {
        public IEnumerable<FormControlViewModel> FormControls { get; set; }
    }
}