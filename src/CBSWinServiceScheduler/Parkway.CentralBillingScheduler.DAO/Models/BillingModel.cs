using System.Collections.Generic;

namespace Parkway.CentralBillingScheduler.DAO.Models
{
    public class BillingModel : CBSModel
    {
        /// <summary>
        /// List of schedules
        /// </summary>
        //public virtual ICollection<BillingSchedule> BillingSchedules { get; set; }

        /// <summary>
        /// billing amount
        /// </summary>
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Assessment string
        /// </summary>
        public virtual string Assessment { get; set; }

        /// <summary>
        /// Fixed or Variable
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

    }
}