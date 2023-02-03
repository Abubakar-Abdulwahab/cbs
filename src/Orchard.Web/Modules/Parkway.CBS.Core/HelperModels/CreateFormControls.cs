using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateFormControls
    {
        public int RevenueHeadId { get; set; }
        public List<FormHelperModel> FormHelperModels { get; set; }
        public string UserEmail { get; set; }
    }

    public class CreateFormControlsResponse 
    {
        public List<FormControlResponse> Responses { get; set; }
    }

    public class FormControlResponse
    {
        public int ControlId { get; set; }
        public string TechnicalName { get; set; }
        public int TaxCategoryIdentifier { get; set; }
    }
}