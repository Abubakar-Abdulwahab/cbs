using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.PAYEReceiptUtilizationReport.SearchFilters
{
    public class ReceiptNumberFilter : IPAYEReceiptUtilizationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ReceiptNumber))
            {
                criteria.Add(Restrictions.Where<PAYEReceipt>(x => x.Receipt.ReceiptNumber == searchParams.ReceiptNumber.Trim()));
            }
        }
    }

    public class TaxEntityIdFilter : IPAYEReceiptUtilizationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams)
        {
            if (searchParams.TaxEntityId > 0)
            {
                criteria.Add(Restrictions.Eq($"{ nameof(PAYEReceipt.Receipt.Invoice.TaxPayer)}.{nameof(PAYEReceipt.Receipt.Invoice.TaxPayer.Id)}", searchParams.TaxEntityId));
            }
        }
    }

    public class StatusFilter : IPAYEReceiptUtilizationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams)
        {
            if (searchParams.Status != Models.Enums.PAYEReceiptUtilizationStatus.None)
            {
                criteria.Add(Restrictions.Where<PAYEReceipt>(x => x.UtilizationStatusId == (int)searchParams.Status));
            }
        }
    }

    public class PayerIDFilter : IPAYEReceiptUtilizationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEReceiptSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.PayerId))
            {
                criteria.Add(Restrictions.Where<PAYEReceipt>(x => x.Receipt.Invoice.TaxPayer.PayerId == searchParams.PayerId.Trim()));
            }
        }
    }


}