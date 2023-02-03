using System.Collections.Concurrent;

namespace Parkway.CBS.Core.HelperModels
{
    /// <summary>
    /// Group of <see cref="RefDataAndCashflowDetails"/> consisting of a list of records that have cashflow customer records and those that have none
    /// </summary>
    public class HasCashflowCustomerAndHasNot
    {
        public ConcurrentStack<RefDataAndCashflowDetails> ItemsWithCashflowDetails { get; set; }
        public ConcurrentStack<RefDataAndCashflowDetails> ItemsWithoutCashflowDetails { get; set; }
    }
}