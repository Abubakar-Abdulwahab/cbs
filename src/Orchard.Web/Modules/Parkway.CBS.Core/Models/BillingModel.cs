using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Models
{
    public class BillingModel : CBSModel
    {
        /// <summary>
        /// List of schedules
        /// </summary>
        public virtual ICollection<BillingSchedule> BillingSchedules { get; set; }


        /// <summary>
        /// MDA this billing model belongs to
        /// </summary>
        public virtual MDA Mda { get; set; }
      

        /// <summary>
        /// billing amount
        /// </summary>
        public virtual decimal Amount { get; set; }

        public virtual decimal Surcharge { get; set; }

        /// <summary>
        /// Assessment string
        /// </summary>
        public virtual string Assessment { get; set; }



        /// <summary>
        /// Fixed or Variable
        /// <see cref="Enums.BillingType"/>
        /// </summary>
        public virtual int BillingType { get; set; }

        /// <summary>
        /// frequency obj
        /// </summary>
        public virtual string BillingFrequency { get; set; }

        /// <summary>
        /// Duration Model
        /// </summary>
        public virtual string Duration { get; set; }

        /// <summary>
        /// Assessment Due Date model
        /// </summary>
        public virtual string DueDate { get; set; }

        /// <summary>
        /// Discount model
        /// </summary>
        public virtual string Discounts { get; set; }

        /// <summary>
        /// Penalties model
        /// </summary>
        public virtual string Penalties { get; set; }

        /// <summary>
        /// Demand notice model
        /// </summary>
        public virtual string DemandNotice { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }
       
        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }

        /// <summary>
        /// updated by the job
        /// </summary>
        public virtual DateTime? NextBillingDate { get; set; }

        /// <summary>
        /// Updated by the job
        /// </summary>
        public virtual bool StillRunning { get; set; }

        public virtual string DirectAssessmentModel { get; set; }

        /// <summary>
        /// Json string binds to dynamic, contains SelectedTemplate and SelectedImplementation
        /// </summary>
        public virtual string FileUploadModel { get; set; }

        /// <summary>
        /// Get billing type enum
        /// </summary>
        /// <returns>BillingType</returns>
        public BillingType GetBillingType()
        {
            return ((BillingType)this.BillingType);
        }
    }
}