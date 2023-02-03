using CBSPay.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Interfaces
{
    public interface IAdminService
    {
        decimal? GetTodayPOAAmount();
        decimal? GetTodayBillSettlementAmount();
        decimal? GetTodaysTotalTransaction();
        IEnumerable<WeeklyPaymentTransaction> GetPaymentTransactionDetailsForOneWeek();
    }
}
