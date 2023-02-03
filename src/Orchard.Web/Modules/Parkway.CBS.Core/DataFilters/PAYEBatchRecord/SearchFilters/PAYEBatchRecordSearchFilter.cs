using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.PAYEBatchRecord.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.DataFilters.PAYEBatchRecord.SearchFilters
{
    public class PAYEBatchRefSearchFilter : IPAYEBatchRecordSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEBatchRecordSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.BatchRef))
            {
                criteria.Add(Restrictions.Like("PBR.BatchRef", searchParams.BatchRef.Trim(), MatchMode.Anywhere));
            }
        }
    }

    public class TaxEntityIdFilter : IPAYEBatchRecordSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, PAYEBatchRecordSearchParams searchParams)
        {
            if (searchParams.TaxEntityId > 0)
            {
                    criteria.Add(Restrictions.Eq("TaxEntity.Id", searchParams.TaxEntityId));
            }
        }
    }
}