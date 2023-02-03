using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Models
{
    public class CellSites : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual bool AddedByAdmin { get; set; }

        public virtual UserPartRecord AdminUser { get; set; }

        public virtual CBSUser OperatorUser { get; set; }

        public virtual TaxEntity TaxProfile { get; set; }

        public virtual OperatorCategory OperatorCategory { get; set; }

        public virtual string OperatorSiteId { get; set; }

        /// <summary>
        /// base prefix for OSGOF Id, feeds the computed column OSGOF Id the prefix for the unique value
        /// </summary>
        public virtual string OperatorSitePrefix { get; set; }

        /// <summary>
        /// readonly field
        /// </summary>
        public virtual string OSGOFId { get; set; }

        public virtual int YearOfDeployment { get; set; }

        public virtual decimal HeightOfTower { get; set; }

        public virtual string MastType { get; set; }

        public virtual string Long { get; set; }

        public virtual string Lat { get; set; }

        public virtual string SiteAddress { get; set; }

        public virtual string Region { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual bool Approved { get; set; }

        public virtual UserPartRecord ApprovedBy { get; set; }
    }
}