using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.ViewModels
{
    public class WeeklyPaymentTransaction
    {
        public DayOfWeek DayOfWeek { get; set; }
        public decimal POAAmount { get; set; }
        public decimal BillSettlementAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public IEnumerable<WeeklyPaymentTransaction> weeklyPaymentTransactions { get; set; }
        public decimal todaysPOAAmount { get; set; }
        public decimal todaysBillSettlementAmount { get; set; }
        public decimal todaysTotalAmount { get; set; }
    }

    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}
