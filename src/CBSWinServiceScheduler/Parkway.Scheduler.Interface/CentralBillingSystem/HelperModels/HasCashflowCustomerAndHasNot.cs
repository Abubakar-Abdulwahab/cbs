using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.CentralBillingSystem.HelperModels
{
    /// <summary>
    /// Group of <see cref="RefDataAndCashflowDetails"/> consisting of a list of records that have cashflow customer records and those that have none
    /// </summary>
    internal class HasCashflowCustomerAndHasNot
    {
        public ConcurrentStack<RefDataAndCashflowDetails> ItemsWithCashflowDetails { get; set; }
        public ConcurrentStack<RefDataAndCashflowDetails> ItemsWithoutCashflowDetails { get; set; }
    }
}
