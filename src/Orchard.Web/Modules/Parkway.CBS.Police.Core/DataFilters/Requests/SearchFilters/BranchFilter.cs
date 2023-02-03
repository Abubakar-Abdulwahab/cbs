using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{

    public class LocationFilter : IPoliceCoporateBranchRequestSearchFilter
    {
        private bool DoCheck(int stateId, int lgaId)
        {
            return stateId != 0 && lgaId != 0;
        }
        private bool DoCheck(int stateId)
        {
            return stateId != 0;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.State, searchParams.LGA))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.State == new StateModel { Id = searchParams.State } && x.TaxEntityProfileLocation.LGA == new LGA { Id = searchParams.LGA }));
            }
            else if (DoCheck(searchParams.State))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.State == new StateModel { Id = searchParams.State }));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.State, searchParams.LGA))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.State == new StateModel { Id = searchParams.State } && x.TaxEntityProfileLocation.LGA == new LGA { Id = searchParams.LGA }));
            }
            else if (DoCheck(searchParams.State))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.State == new StateModel { Id = searchParams.State }));
            }
        }
    }

    public class BranchNameFilter : IPoliceCoporateBranchRequestSearchFilter
    {
        private bool DoCheck(string branchName)
        {
            return !string.IsNullOrEmpty(branchName);
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.BranchName))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.Name.IsInsensitiveLike(searchParams.BranchName, MatchMode.Anywhere)));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.BranchName))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.Name.IsInsensitiveLike(searchParams.BranchName, MatchMode.Anywhere)));
            }
        }
    }

    public class BranchFilter : IPoliceCoporateBranchRequestSearchFilter
    {
        private bool DoCheck(int branchId)
        {
            return branchId != 0;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.Branch))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.Id == searchParams.Branch));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.Branch))
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.Id == searchParams.Branch));
            }
        }
    }

}