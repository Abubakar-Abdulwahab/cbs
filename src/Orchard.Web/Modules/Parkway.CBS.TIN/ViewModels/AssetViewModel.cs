using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.TIN.ViewModels
{
    public class AssetViewModel
    {
        public string TypeOfAsset { get; set; }
        public int MarketValue { get; set; }
        public DateTime OwnershipDate { get; set; }
        public string LocationOfAsset { get; set; }
    }
}