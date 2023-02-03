using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;

namespace Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport.SearchFilters
{

    /// <summary>
    /// Officer name filter
    /// </summary>
    public class PoliceOfficerNameFilter : IPoliceOfficerReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PoliceOfficerSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.OfficerName))
            {
                criteria.Add(Restrictions.Where<PoliceOfficer>(x => x.Name.IsLike("%" + searchParams.OfficerName + "%")));
            }
        }
    }


    /// <summary>
    /// Officer rank filter
    /// </summary>
    public class PoliceOfficerRankFilter : IPoliceOfficerReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PoliceOfficerSearchParams searchParams, bool addAlias = false)
        {
            if (searchParams.Rank > 0)
            {
                criteria.Add(Restrictions.Where<PoliceOfficer>(x => x.Rank.Id == searchParams.Rank));
            }
        }
    }


    /// <summary>
    /// Police IPPIS Number filter
    /// </summary>
    public class PoliceOfficerIppisNumberFilter : IPoliceOfficerReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PoliceOfficerSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.IPPISNo))
            {
                criteria.Add(Restrictions.Where<PoliceOfficer>(x => x.IPPISNumber == searchParams.IPPISNo));
            }
        }
    }


    /// <summary>
    /// Police Identification Number filter
    /// </summary>
    public class PoliceOfficerIdNumberFilter : IPoliceOfficerReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PoliceOfficerSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.IdNumber))
            {
                criteria.Add(Restrictions.Where<PoliceOfficer>(x => x.IdentificationNumber == searchParams.IdNumber));
            }
        }
    }


    /// <summary>
    /// Police Command filter
    /// </summary>
    public class PoliceOfficerCommandFilter : IPoliceOfficerReportFilters
    {
        private bool DoCheck(int commandId)
        {
            return commandId != 0;
        }

        private bool DoCheckForAllCommands(int commandId, int lgaId)
        {
            return commandId == 0 && lgaId != 0;
        }

        private bool DoCheckForNoCommands(int commandId)
        {
            return commandId == -1;
        }

        public void AddCriteriaRestriction(ICriteria criteria, PoliceOfficerSearchParams searchParams, bool addAlias = false)
        {
            if (DoCheckForAllCommands(searchParams.CommandId, searchParams.LGA))
            {
                var allCommandsCriteria = DetachedCriteria.For<Command>("Cmd")
                    .Add(Restrictions.And(Restrictions.EqProperty("Id", $"{nameof(PoliceOfficer)}.Command.Id"), Restrictions.Eq("LGA.Id", searchParams.LGA)))
                    .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(allCommandsCriteria));

            }
            else if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceOfficer>(x => x.Command == new Command { Id = searchParams.CommandId }));
            }
            else if (DoCheckForNoCommands(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceOfficer>(x => x.Command == new Command { Id = 0 }));
            }
        }
    }
}