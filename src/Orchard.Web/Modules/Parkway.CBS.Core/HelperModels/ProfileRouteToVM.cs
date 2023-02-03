using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RouteToVM
    {
        public InvoiceGenerationStage Stage { get; set; }

        public string RouteName { get; set; }
    }    
}