using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.PSSUserReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSUserReport.SearchFilters
{
    public class PSSUserReportNameFilters : IPSSUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSUserReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.Name))
            {
                criteria.Add(Restrictions.InsensitiveLike(nameof(CBSUser.Name), searchParams.Name, MatchMode.Anywhere));
            }
        }
    }

    public class PSSUserReportUserNameFilters : IPSSUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSUserReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserName))
            {
                criteria.Add(Restrictions.InsensitiveLike($"{nameof(CBSUser.UserPartRecord)}.{nameof(CBSUser.UserPartRecord.UserName)}", searchParams.UserName, MatchMode.Anywhere));
            }
        }
    }

    public class PSSUserReportIdentificationNumberFilters : IPSSUserReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSUserReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.IdentificationNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(CBSUser.TaxEntity)}.{nameof(CBSUser.TaxEntity.IdentificationNumber)}", searchParams.IdentificationNumber));
            }
        }
    }
}