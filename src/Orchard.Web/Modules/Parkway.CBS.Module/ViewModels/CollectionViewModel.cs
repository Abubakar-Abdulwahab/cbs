using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Payee;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class CollectionViewModel
    {
        
        public string TINorRIN { get; set; }
        //enum of use types
        public TaxPayerType TaxPayerType { get; set; }

        public List<TaxEntityCategory> TaxCategories { get; set; }
        //list of mdas
        public IEnumerable<SelectListItem> Mdas { get; set; }

        public IEnumerable<RevenueHead> RevenueHeads { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public DirectAssessmentCollection DirectAssessmentCollection { get; set; }

        public bool IsCBSUser { get; set; }

        public CBSUser CBSUser { get; set; }

        public ICollection<DirectAssessmentPayeeLine> DirectAssessmentPayeeLines { get; set; }

        public Dictionary<string, string> StateLGAs { get; internal set; }
    }
}