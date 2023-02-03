using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.CollectionReport.SearchFilters
{

    public class PaymentProviderCR : ICollectionReportFilters
    {
        public string FilterName => typeof(PaymentProviderCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if (searchParams.PaymentProviderId > 0)
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.PaymentProvider == searchParams.PaymentProviderId));
            }
        }
    }

    public class PaymentChannelCR : ICollectionReportFilters
    {
        public string FilterName => typeof(PaymentChannelCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if (searchParams.SelectedPaymentChannel != Models.Enums.PaymentChannel.None)
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.Channel == (int)searchParams.SelectedPaymentChannel));
                //criteria.Add(Restrictions.Eq("Channel", (int)searchParams.SelectedPaymentMethod));
            }
        }
    }


    public class PaymentReferenceCR : ICollectionReportFilters
    {
        public string FilterName => typeof(PaymentReferenceCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.PaymentRef))
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.PaymentReference == searchParams.PaymentRef.Trim()));
                //criteria.Add(Restrictions.Eq("PaymentReference", searchParams.PaymentRef.Trim()));
            }
        }
    }


    public class InvoiceNumberCR : ICollectionReportFilters
    {
        public string FilterName => typeof(InvoiceNumberCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.InvoiceNumber == searchParams.InvoiceNumber.Trim()));
                //criteria.Add(Restrictions.Eq("InvoiceNumber", searchParams.InvoiceNumber.Trim()));
            }
        }
    }


    public class ReceiptNumberCR : ICollectionReportFilters
    {
        public string FilterName => typeof(ReceiptNumberCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ReceiptNumber))
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.ReceiptNumber == searchParams.ReceiptNumber.Trim()));
                //criteria.Add(Restrictions.Eq("ReceiptNumber", searchParams.ReceiptNumber.Trim()));
            }
        }
    }


    public class BankCodeCR : ICollectionReportFilters
    {
        public string FilterName => typeof(BankCodeCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.SelectedBankCode) && (searchParams.SelectedBankCode != "0"))
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.BankCode == searchParams.SelectedBankCode.Trim()));
                //criteria.Add(Restrictions.Eq("BankCode", searchParams.SelectedBankCode.Trim()));
            }
        }
    }


    public class SelectedMDACR : ICollectionReportFilters
    {
        public string FilterName => typeof(SelectedMDACR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if(searchParams.MDAId > 0)
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.MDA.Id == searchParams.MDAId));
                //criteria.Add(Restrictions.Eq("TransactionLogMDA.Slug", searchParams.SelectedMDA.ToString()));

            }
        }
    }


    public class SelectedRevenueHeadCR : ICollectionReportFilters
    {
        public string FilterName => typeof(SelectedRevenueHeadCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, CollectionSearchParams searchParams)
        {
            if(searchParams.RevenueHeadId > 0)
            {
                criteria.Add(Restrictions.Where<TransactionLog>(x => x.RevenueHead.Id == searchParams.RevenueHeadId));
                //criteria.Add(Restrictions.Eq("TransactionLogRevenueHead.Id", searchParams.RevenueHeadId));
            }
        }
    }
}