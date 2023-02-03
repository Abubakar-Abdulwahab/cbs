using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{
    public class CellSitesScheduleVM
    {
        public string PayerId { get; set; }

        public long Id { get; set; }

        public int TotalNoOfRowsProcessed { get; set; }

        /// <summary>
        /// indicates whether a schedule has been treated, ie processed and approved 
        /// </summary>
        public bool ScheduleHasAlreadyBeenTreated { get; set; }
    }

    public class CellSitesScheduleStagingVM
    {
        public Int64 StagingId { get; set; }

        /// <summary>
        /// Tax Entity profile
        /// </summary>
        public Int64 ProfileId { get; set; }

        /// <summary>
        /// Id of the admin user that uploaded the file, if applicable
        /// </summary>
        public int AdminUserId { get; set; }

        /// <summary>
        /// Id of the CBSUser that uploaded the file, if applicable
        /// </summary>
        public Int64 OperatorId { get; set; }

        /// <summary>
        /// indicates if it was an admin side upload
        /// </summary>
        public bool AddedByAdmin { get; set; }

        /// <summary>
        /// indicates whether a schedule has been treated, ie processed and approved 
        /// </summary>
        public bool ScheduleHasAlreadyBeenTreated { get; set; }

        public string PayerId { get; set; }

        /// <summary>
        /// operator name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Base identifier for OSGOF site Id
        /// </summary>
        public string OSGOFSiteIdPrefix { get; set; }


        public int OperatorCategoryId { get; set; }
    }
}