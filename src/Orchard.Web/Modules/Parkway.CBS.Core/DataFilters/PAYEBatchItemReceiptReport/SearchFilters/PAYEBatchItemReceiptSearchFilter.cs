using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchItemReceiptReport.SearchFilters
{
    public class ReceiptNumberFilter : IPAYEBatchItemReceiptSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ReceiptNumber))
            {
                criteria.Add(Restrictions.Where<PAYEBatchItemReceipt>(x => x.ReceiptNumber == searchParams.ReceiptNumber.Trim()));
            }
        }
    }

    public class TaxEntityIdFilter : IPAYEBatchItemReceiptSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams)
        {
            if (searchParams.TaxEntityId > 0)
            {
                if (searchParams.IsEmployer)
                {
                    criteria.CreateAlias("PYR.PAYEBatchItem.PAYEBatchRecord", "BatchRecord");
                    criteria.Add(Restrictions.Eq("BatchRecord.TaxEntity.Id", searchParams.TaxEntityId));
                }
                else
                {
                    criteria.Add(Restrictions.Eq("TaxEntity.Id", searchParams.TaxEntityId));
                }
            }
        }
    }

}