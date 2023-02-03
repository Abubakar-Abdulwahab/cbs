using Newtonsoft.Json;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class BillingFrequencyModel
    {
        public FrequencyType FrequencyType { get; set; }
        //Fixed frequency
        public FixedBillingModel FixedBill { get; set; }
        //Variable frequency
        public VariableBillingModel VariableBill { get; set; }
        //duration
        public DurationModel Duration { get; set; }
        
    }

    public class DurationModel
    {
        public DurationType DurationType { get; set; }
        public int EndsAfterRounds { get; set; }
        public string EndsAtDate { get; set; } //dd/MM/YYYY 
        public DateTime? EndsDate { get; set; } //dd/MM/YYYY 

    }    

    public class FixedBillingModel
    {
        public DateTime StartDateAndTime { get; set; }

        public string CRONExpression { get; set; }

        /// <summary>
        /// dd/MM/YYYY
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        public string StartFrom { get; set; }

        /// <summary>
        /// HH:mm
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        public string StartTime { get; set; }

        public YearlyBillingModel YearlyBill { get; set; }

        public MonthlyBillingModel MonthlyBill { get; set; }

        public WeeklyBillingModel WeeklyBill { get; set; }

        public DailyBillingModel DailyBill { get; set; }

        [JsonIgnore]
        public DateTime NextBillingDate { get; internal set; }
    }

    public class VariableBillingModel
    {
        public int Interval { get; set; }
    }

    public class YearlyBillingModel
    {
        public int NumberOfYears { get; set; }
        public MonthlyBillingModel MonthlyBill { get; set; }
    }

    public class MonthlyBillingModel
    {
        public List<Months> Months { get; set; }
        public WeekDays WeekDay { get; set; }
        public Days SelectedDay { get; set; }
    }

    public class WeeklyBillingModel
    {
        public List<Days> Days { get; set; }
    }

    public class DailyBillingModel
    {
        public int Interval { get; set; }
    }
}