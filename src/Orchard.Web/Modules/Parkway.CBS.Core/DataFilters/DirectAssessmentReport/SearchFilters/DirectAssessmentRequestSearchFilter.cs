using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.DirectAssessmentReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.DirectAssessmentReport.SearchFilters
{
    public class DirectAssessmentTINFilter : IDirectAssessmentRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, DirectAssessmentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.TIN))
            {
                criteria.Add(Restrictions.Eq("TaxPayer.PayerId", searchParams.TIN.Trim()));
            }
        }
    }

    public class DirectAssessmentTypeFilter : IDirectAssessmentRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, DirectAssessmentSearchParams searchParams)
        {
            if (searchParams.DirectAssessmentType != 0 )
            {
                criteria.Add(Restrictions.Where<PAYEDirectAssessmentRecord>(x => x.AssessmentType.Id == searchParams.DirectAssessmentType));
            }
        }
    }

    public class DirectAssessmentInvoiceNumberFilter : IDirectAssessmentRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, DirectAssessmentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                criteria.Add(Restrictions.Where<PAYEDirectAssessmentRecord>(x => x.Invoice.InvoiceNumber == searchParams.InvoiceNumber.Trim()));
            }
        }
    }

    public class DirectAssessmentInvoiceStatusFilter : IDirectAssessmentRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, DirectAssessmentSearchParams searchParams)
        {
            if (searchParams.InvoiceStatus != Models.Enums.InvoiceStatus.All)
            {
                criteria.Add(Restrictions.Where<PAYEDirectAssessmentRecord>(x => x.Invoice.Status == (int)searchParams.InvoiceStatus));
            }
        }
    }
}