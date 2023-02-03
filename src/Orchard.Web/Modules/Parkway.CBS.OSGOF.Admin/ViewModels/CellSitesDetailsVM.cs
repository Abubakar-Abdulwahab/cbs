using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;
using Parkway.CBS.OSGOF.Admin.Models;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class CellSitesDetailsVM
    {
        public Int64 Id { get; set; }

        public string OperatorSiteId { get; set; }

        public string OSGOFID { get; set; }

        public int YearOfDeployment { get; set; }

        public decimal HeightOfTower { get; set; }

        public string Long { get; set; }

        public string Lat { get; set; }

        //public TaxEntity Operator { get; set; }

        public TaxEntityProfileVM Operator { get; set; }

        public string SiteAddress { get; set; }

        public string Region { get; set; }

        public string State { get; set; }

        public string LGA { get; set; }

        public decimal Amount { get; set; }

        public string MastType { get; set; }

        //public CellSitesScheduleStaging Schedule { get; set; }
    }
}