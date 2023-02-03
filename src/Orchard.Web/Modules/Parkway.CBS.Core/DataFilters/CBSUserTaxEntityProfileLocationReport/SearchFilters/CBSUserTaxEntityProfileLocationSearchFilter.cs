using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.DataFilters.CBSUserTaxEntityProfileLocationReport.SearchFilters
{
    public class NameFilter : ICBSUserTaxEntityProfileLocationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.SubUserName))
            {
                criteria.Add(Restrictions.InsensitiveLike("CBSUser.Name", searchParams.SubUserName, MatchMode.Anywhere));
            }
        }
    }


    public class BranchFilter : ICBSUserTaxEntityProfileLocationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, CBSUserTaxEntityProfileLocationReportSearchParams searchParams)
        {
            if (searchParams.Branch > 0)
            {
                criteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation == new TaxEntityProfileLocation { Id = searchParams.Branch }));
            }
        }
    }
}