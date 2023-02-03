﻿using Orchard;
using NHibernate;
using Parkway.CBS.Police.Core.VM;
using NHibernate.Criterion;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{
    public interface IPoliceRequestSearchFilter : IDependency
    {

        /// <summary>
        /// Add criteria restrictions
        /// <para>throws exception if Id for associate tables in PSServiceRequest are not found</para>
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams);


        /// <summary>
        /// Add criteria restrictions
        /// <para>throws exception if Id for associate tables in PSServiceRequest are not found</para>
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        /// <exception cref="NoRecordFoundException"></exception>
        void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams);


        /// <summary>
        /// Add criteria restriction for invoice number search
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchParams"></param>
        void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams);

    }
}
