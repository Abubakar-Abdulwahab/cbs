using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.Schedulers.Quartz
{
    public class JobDataKs
    {

        /// <summary>
        /// Billing Type
        /// </summary>
        public const string BILLING_TYPE = "BillingType";

        /// <summary>
        /// Fixed
        /// </summary>
        public const string FIXED_BILLING_TYPE = "Fixed";

        /// <summary>
        /// DurationType
        /// </summary>
        public const string DURATION_TYPE = "DurationType";

        /// <summary>
        /// EndsAfter
        /// </summary>
        public const string ENDS_AFTER = "EndsAfter";

        /// <summary>
        /// FixedRounds
        /// </summary>
        public const string FIXED_ROUNDS = "FixedRounds";

        /// <summary>
        /// Rounds
        /// </summary>
        public const string ROUNDS = "Rounds";

        /// <summary>
        /// EndsOn
        /// </summary>
        public const string ENDS_ON = "EndsOn";

        /// <summary>
        /// NeverEnds
        /// </summary>
        public const string NEVER_ENDS = "NeverEnds";

        /// <summary>
        /// Variable billing type
        /// </summary>
        public const string VARIABLE_BILLING_TYPE = "Variable";
    }
}
