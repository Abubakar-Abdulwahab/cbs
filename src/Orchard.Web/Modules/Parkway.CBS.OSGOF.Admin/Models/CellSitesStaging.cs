using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Models
{
    public class CellSitesStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual int SNOnFile { get; set; }

        public virtual string SNOnFileFileValue { get; set; }

        public virtual string OperatorSiteId { get; set; }

        //public virtual string OSGOFId { get; set; }

        public virtual int YearOfDeployment { get; set; }

        public virtual string YearOfDeploymentFileValue { get; set; }

        public virtual decimal HeightOfTower { get; set; }

        public virtual string HeightOfTowerFileValue { get; set; }

        public virtual string MastType { get; set; }

        public virtual string Long { get; set; }

        public virtual string Lat { get; set; }

        public virtual string SiteAddress { get; set; }

        public virtual string Region { get; set; }

        public virtual StateModel State { get; set; }

        public virtual string StateFileValue { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual string LGAFileValue { get; set; }

        public virtual CellSitesScheduleStaging Schedule { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }
    }
}