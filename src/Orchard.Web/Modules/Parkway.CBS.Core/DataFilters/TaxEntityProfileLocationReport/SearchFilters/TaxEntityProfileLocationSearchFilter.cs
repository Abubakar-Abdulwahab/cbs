using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.SearchFilters.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.SearchFilters
{
    public class AddressFilter : ITaxEntityProfileLocationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TaxEntityProfileLocationReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.Address))
            {
                criteria.Add(Restrictions.InsensitiveLike("Address", searchParams.Address, MatchMode.Anywhere));
            }
        }
    }


    public class NameFilter : ITaxEntityProfileLocationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TaxEntityProfileLocationReportSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.Name))
            {
                criteria.Add(Restrictions.InsensitiveLike("Name", searchParams.Name, MatchMode.Anywhere));
            }
        }
    }


    public class StateFilter : ITaxEntityProfileLocationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TaxEntityProfileLocationReportSearchParams searchParams)
        {
            if (searchParams.State > 0)
            {
                criteria.Add(Restrictions.Where<TaxEntityProfileLocation>(x => x.State == new StateModel { Id = searchParams.State }));
            }
        }
    }


    public class LGAFilter : ITaxEntityProfileLocationSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, TaxEntityProfileLocationReportSearchParams searchParams)
        {
            if (searchParams.LGA > 0)
            {
                criteria.Add(Restrictions.Where<TaxEntityProfileLocation>(x => x.LGA == new LGA { Id = searchParams.LGA }));
            }
        }
    }
}