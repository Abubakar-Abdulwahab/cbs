using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.FileUpload.OSGOFImplementation.Models
{
    public class OSGOFCellSitesExcelModel
    {
        public bool HasError { get; internal set; }

        public string ErrorMessages { get; internal set; }

        public CellSiteIntValue SN { get; set; }

        public CellSiteStringValue OperatorSiteId { get; internal set; }

        public CellSiteIntValue YearofDeployment { get; internal set; }

        public CellSiteDecimalValue HeightofTower { get; internal set; }

        public CellSiteStringValue Longitude { get; internal set; }

        public CellSiteStringValue TowerMastType { get; internal set; }

        public CellSiteStringValue Latitude { get; internal set; }

        public CellSiteStringValue SiteAddress { get; internal set; }

        public CellSiteStringValue Region { get; internal set; }

        public CellSiteStringValue State { get; internal set; }

        public CellSiteStringValue LGA { get; internal set; }


    }
}
