using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.AssessmentReport.SearchFilters
{
    public class MDAIR : IInvoiceAssessmentSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, InvoiceAssessmentSearchParams searchParams)
        {
            if(searchParams.MDAId != 0)
            {
                criteria.Add(Restrictions.Where<InvoiceItems>(x => x.Mda.Id == searchParams.MDAId));
                //criteria.Add(Restrictions.Eq("InvoiceItems.Mda.Id", searchParams.MDAId));
            }
        }
    }

    public class RevenueHeadIR : IInvoiceAssessmentSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, InvoiceAssessmentSearchParams searchParams)
        {
            if (searchParams.RevenueHeadId != 0)
            {
                criteria.Add(Restrictions.Where<InvoiceItems>(x => x.RevenueHead.Id == searchParams.RevenueHeadId));
                //criteria.Add(Restrictions.Eq("InvoiceItems.RevenueHead.Id", searchParams.RevenueHeadId));
            }
        }
    }

    public class TINIR : IInvoiceAssessmentSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, InvoiceAssessmentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.TIN))
            {
                criteria.Add(Restrictions.Where<InvoiceItems>(x => x.TaxEntity.TaxPayerIdentificationNumber == searchParams.TIN));
                //criteria.Add(Restrictions.Eq("InvoiceItems.TaxEntity.TaxPayerIdentificationNumber", searchParams.TIN));
            }
        }
    }

    public class InvoiceStatusIR : IInvoiceAssessmentSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, InvoiceAssessmentSearchParams searchParams)
        {
            if (searchParams.Options.PaymentStatus != Models.Enums.InvoiceStatus.All)
            {
                criteria.Add(Restrictions.Where<InvoiceItems>(x => x.Invoice.Status == (int)searchParams.Options.PaymentStatus));
                //criteria.Add(Restrictions.Eq("InvoiceItems.Invoice.Status", (int)searchParams.Options.PaymentStatus));
            }
        }
    }

    public class InvoiceNumberIR : IInvoiceAssessmentSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, InvoiceAssessmentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                //criteria.Add(Restrictions.Eq("InvoiceItems.InvoiceNumber", searchParams.InvoiceNumber));
                criteria.Add(Restrictions.Where<InvoiceItems>(x => x.InvoiceNumber == searchParams.InvoiceNumber));
            }
        }
    }

    public class CategoryIR : IInvoiceAssessmentSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, InvoiceAssessmentSearchParams searchParams)
        {
            if (searchParams.Category != 0)
            {
                criteria.Add(Restrictions.Where<InvoiceItems>(x => x.TaxEntityCategory.Id == searchParams.Category));
            }
        }
    }
}