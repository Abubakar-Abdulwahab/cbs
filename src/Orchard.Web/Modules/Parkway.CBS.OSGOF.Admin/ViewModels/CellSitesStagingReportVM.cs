using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class CellSitesStagingReportVM
    {
        public dynamic Pager { get; set; }

        public string PayerId { get; set; }

        public bool Error { get; set; }

        public string ErrorMessage { get; set; }

        public string Message { get; set; }

        public Int64 TotalNumberOfRecords { get; set; }

        public Int64 TotalNumberOfInvalid { get; set; }

        public Int64 TotalNumberOfValidRecords { get; set; }

        public List<CellSitesStagingVM> CellSites { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public int PageSize { get; set; }

        public bool DoWork { get; set; }

        public string ScheduleRef { get; set; }
    }

    public class CellSitesStagingVM
    {
        public string SNOnFile { get; set; }

        public string OperatorSiteId { get; set; }

        public string YearOfDeployment { get; set; }

        public string HeightOfTower { get; set; }

        public string MastType { get; set; }

        public string Long { get; set; }

        public string Lat { get; set; }

        public string SiteAddress { get; set; }

        public string Region { get; set; }

        public string State { get; set; }

        public string LGA { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessages { get; set; }
    }
}