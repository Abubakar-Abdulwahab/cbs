using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class CellSitesVM
    {
        public dynamic Pager { get; set; }

        public int TotalNumberOfCellSites { get; set; }

        public List<CellSitesDetailsVM> CellSites { get; set; }

        public int PageSize { get; set; }

        public long OperatorId { get; set; }

        public bool DoWork { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string Message { get; set; }

    }

}