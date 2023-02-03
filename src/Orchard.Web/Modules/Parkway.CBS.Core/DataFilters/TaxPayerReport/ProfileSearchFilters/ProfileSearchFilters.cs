using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.TaxPayerReport.ProfileSearchFilters
{
    /// <summary>
    /// for Category
    /// </summary>
    public class CategoryCR : IProfileSearchFilters
    {
        public string FilterName => typeof(CategoryCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, TaxProfilesSearchParams searchParams)
        {
            criteria.Add(Restrictions.Eq("TaxEntityCategory", new TaxEntityCategory() { Id = searchParams.CategoryId }));
        }
    }

    /// <summary>
    /// for PayerId
    /// </summary>
    public class PayerIDCR : IProfileSearchFilters
    {
        public string FilterName => typeof(PayerIDCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, TaxProfilesSearchParams searchParams)
        {
            criteria.Add(Restrictions.Eq("PayerId", searchParams.PayerId.Trim()));
        }
    }


    /// <summary>
    /// for TIN 
    /// </summary>
    public class TINCR : IProfileSearchFilters
    {
        public string FilterName => typeof(TINCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, TaxProfilesSearchParams searchParams)
        {
            criteria.Add(Restrictions.Eq("TaxPayerIdentificationNumber", searchParams.TIN.Trim()));
        }
    }


    /// <summary>
    /// for PhoneNumber
    /// </summary>
    public class PhoneNumberCR : IProfileSearchFilters
    {
        public string FilterName => typeof(PhoneNumberCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, TaxProfilesSearchParams searchParams)
        {
            criteria.Add(Restrictions.Eq("PhoneNumber", searchParams.PhoneNumber.Trim()));
        }
    }


    /// <summary>
    /// for Name
    /// </summary>
    public class NameCR : IProfileSearchFilters
    {
        public string FilterName => typeof(NameCR).Name;

        public void AddCriteriaRestriction(ICriteria criteria, TaxProfilesSearchParams searchParams)
        {
            criteria.Add(Restrictions.InsensitiveLike("Recipient", searchParams.Name.Trim(), MatchMode.Anywhere));
        }
    }
}