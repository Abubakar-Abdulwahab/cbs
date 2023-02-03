using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.ExpenditureHeadReport.SearchFilters
{
    public class ExpenditureHeadReportNameFilters : IExpenditureHeadReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, ExpenditureHeadReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ExpenditureHeadName))
            {
                criteria.Add(Restrictions.InsensitiveLike(nameof(PSSExpenditureHead.Name), searchParams.ExpenditureHeadName, MatchMode.Anywhere));
            }
        }
    }

    public class ExpenditureHeadReportCodeFilters : IExpenditureHeadReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, ExpenditureHeadReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.Code))
            {
                criteria.Add(Restrictions.Where<PSSExpenditureHead>(x => x.Code == searchParams.Code));
            }
        }
    }

    public class ExpenditureHeadReportActiveFilters : IExpenditureHeadReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, ExpenditureHeadReportSearchParams searchParams)
        {
            if (searchParams.Status != (int)StatusFilter.All)
            {
                bool status = searchParams.Status == 1;
                criteria.Add(Restrictions.Where<PSSExpenditureHead>(x => x.IsActive == status));

            }
        }
    }
}