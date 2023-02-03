using CBSPay.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    // for lack of a better name
    public interface IConstantAPIModelService
    {
        void SaveTaxPayersTypeList();
        void SaveRevenueSubStreamList();
        void SaveRevenueStreamList();
        void SaveEconomicActivities(int taxPayerType);
        IEnumerable<TaxPayerType> FetchTaxPayersTypeList();
        IEnumerable<RevenueSubStream> FetchRevenueSubStreamList(); 
        IEnumerable<RevenueStream> FetchRevenueStreamList();
        IEnumerable<EconomicActivities> FetchEconomicActivitiesList(string taxPayerType);

    }
}
