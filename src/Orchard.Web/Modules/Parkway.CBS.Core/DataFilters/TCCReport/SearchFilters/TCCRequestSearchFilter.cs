using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.TCCReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.TCCReport.SearchFilters
{

    public class ApplicationNumberFilter : ITCCRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TCCReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ApplicationNumber))
            {
                criteria.Add(Restrictions.Where<TaxClearanceCertificateRequest>(x => x.ApplicationNumber == searchParams.ApplicationNumber.Trim()));
            }
        }
    }

    public class ApplicantNameFilter : ITCCRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TCCReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ApplicantName))
            {
                criteria.Add(Restrictions.Where<TaxClearanceCertificateRequest>(x => x.ApplicantName.IsLike("%" + searchParams.ApplicantName.Trim() + "%")));
            }
        }
    }

    public class StatusFilter : ITCCRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TCCReportSearchParams searchParams)
        {
            if (searchParams.SelectedStatus != Models.Enums.TCCRequestStatus.None)
            {
                criteria.Add(Restrictions.Where<TaxClearanceCertificateRequest>(x => x.Status == (int)searchParams.SelectedStatus));
            }
        }
    }

    public class TaxEntityIdFilter : ITCCRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TCCReportSearchParams searchParams)
        {
            if(searchParams.TaxEntityId > 0)
            {
                criteria.Add(Restrictions.Where<TaxClearanceCertificateRequest>(x => x.TaxEntity.Id == searchParams.TaxEntityId));
            }
        }
    }

    public class PayerIDFilter : ITCCRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TCCReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.PayerId))
            {
                criteria.Add(Restrictions.Where<TaxClearanceCertificateRequest>(x => x.TaxEntity.PayerId == searchParams.PayerId.Trim()));
            }
        }
    }

    public class ApprovalLevelIdFilter : ITCCRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TCCReportSearchParams searchParams)
        {
            if (searchParams.ApprovalLevelId > 0)
            {
                criteria.Add(Restrictions.Where<TaxClearanceCertificateRequest>(x => x.ApprovalStatusLevelId == searchParams.ApprovalLevelId));
            }
        }
    }

}