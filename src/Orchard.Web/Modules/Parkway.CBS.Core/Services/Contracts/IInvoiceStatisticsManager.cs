using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IInvoiceStatisticsManager<Stats> : IDependency, IBaseManager<Stats>
    {
        Stats GetStats(RevenueHead revenueHead, DateTime dueDate, TaxEntityCategory category);

        /// <summary>
        /// Seed the convcats
        /// </summary>
        void DoConcat();
        void DeleteAll();
        void Populate();
        void Populate2();
    }
}
