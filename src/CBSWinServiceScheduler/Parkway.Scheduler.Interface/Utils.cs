using Newtonsoft.Json;
using Parkway.Scheduler.Interface.CentralBillingSystem.HelperModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface
{
    internal class Utils
    {

        internal static string ComplexDump<O>(O objValue)
        {
            return JsonConvert.SerializeObject(objValue);
        }


        /// <summary>
        /// Group list of <see cref="RefDataAndCashflowDetails"/> into items that have cashflow records and those that havve none
        /// </summary>
        /// <param name="refDataTaxEntityiesJoiner"></param>
        /// <returns>HasCashflowCustomerAndHasNot</returns>
        internal static HasCashflowCustomerAndHasNot SegmentThoseThatHaveCashflowRecordsAndThoseThatHaveNone(IList<RefDataAndCashflowDetails> refDataTaxEntityiesJoiner)
        {
            ConcurrentStack<RefDataAndCashflowDetails> hasCashFlow = new ConcurrentStack<RefDataAndCashflowDetails>();
            ConcurrentStack<RefDataAndCashflowDetails> hasCashFlowNot = new ConcurrentStack<RefDataAndCashflowDetails>();

            Parallel.ForEach(refDataTaxEntityiesJoiner, (item) =>
            {
                if (item.CashflowCustomerId != 0) { hasCashFlow.Push(item); }
                else { hasCashFlowNot.Push(item); }
            });
            return new HasCashflowCustomerAndHasNot { ItemsWithCashflowDetails = hasCashFlow, ItemsWithoutCashflowDetails = hasCashFlowNot };
        }


        /// <summary>
        /// Group unique ref data item and duplicates
        /// </summary>
        /// <param name="itemsWithoutCashflowDetails"></param>
        /// <returns>ProcessResponseModel</returns>
        internal static ProcessResponseModel SegmentEntitiesWithoutCashflowRecordsIntoUniqueItemAndDuplicates(ConcurrentStack<RefDataAndCashflowDetails> itemsWithoutCashflowDetails)
        {
            try
            {
                ConcurrentDictionary<string, RefDataAndCashflowDetails> distinctItems = new ConcurrentDictionary<string, RefDataAndCashflowDetails>();
                ConcurrentStack<RefDataAndCashflowDetails> duplicates = new ConcurrentStack<RefDataAndCashflowDetails>();

                Parallel.ForEach(itemsWithoutCashflowDetails, (entity) =>
                {
                    if (!distinctItems.TryAdd(entity.TaxIdentificationNumber, entity))
                    {
                        duplicates.Push(entity);
                    }
                });
                return new ProcessResponseModel { MethodReturnObject = new RefDataDistinctGroupModel { DistinctItems = distinctItems, Duplicates = duplicates } };
            }
            catch (Exception exception)
            {
                return new ProcessResponseModel { HasErrors = true, ErrorMessage = string.Format("Error getting distinct ref data records. Exception: {0} Exceptio Trace: {1}", exception.Message, exception.StackTrace) };
            }
        }
    }
}
