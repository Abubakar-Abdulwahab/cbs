using System;

namespace Parkway.Scheduler.Interface.Schedulers
{
    public class TriggerDurationHelperModel
    {
        public TriggerDurationType TriggerDurationType { get; set; }
        
        /// <summary>
        /// Duration rounds
        /// </summary>
        public int EndsAfterRounds { get; set; }

        /// <summary>
        /// Ends on this date
        /// </summary>
        public DateTime? EndsDate { get; set; }
    }

    public enum TriggerDurationType
    {
        None = 0,
        NeverEnds = 1,
        EndsAfter = 2,
        EndsOn = 3,
    }
}