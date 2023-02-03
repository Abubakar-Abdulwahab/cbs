using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeployment.SearchFilters
{
    /// <summary>
    /// Customer name filter
    /// </summary>
    public class CustomerNameFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.CustomerName))
            {
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.CustomerName.IsLike("%" + searchParams.CustomerName + "%")));
            }
        }
    }


    /// <summary>
    /// Invoice number filter
    /// </summary>
    public class InvoiceNumberFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                if (addAlias) { criteria.CreateAlias(nameof(PoliceOfficerDeploymentLog.Invoice), "Invoice"); }
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.Invoice.InvoiceNumber == searchParams.InvoiceNumber));
            }
        }
    }


    /// <summary>
    /// IPPIS number
    /// </summary>
    public class IPPISNoFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.IPPISNo))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PolicerOfficerLog)}.{nameof(PolicerOfficerLog.IPPISNumber)}", searchParams.IPPISNo));
            }
        }
    }


    /// <summary>
    /// State LGA filter
    /// </summary>
    public class StateLGAFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (searchParams.State > 0)
            {
                if (addAlias) { criteria.CreateAlias(nameof(PoliceOfficerDeploymentLog.State), "State"); }
                if (searchParams.LGA > 0)
                {
                    if (addAlias) { criteria.CreateAlias(nameof(PoliceOfficerDeploymentLog.LGA), "LGA"); }
                    criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => ((x.LGA.Id == searchParams.LGA) && (x.State.Id == searchParams.State))));
                }
                else
                {
                    criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.State.Id == searchParams.State));
                }
            }
        }
    }



    /// <summary>
    /// Police rank
    /// </summary>
    public class RankFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (searchParams.Rank > 0)
            {
                if (addAlias) { criteria.CreateAlias(nameof(PoliceOfficerDeploymentLog.OfficerRank), "OfficerRank"); }
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.OfficerRank.Id == searchParams.Rank));
            }
        }
    }


    /// <summary>
    /// Address
    /// </summary>
    public class AddressFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.Address))
            {
               
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.Address.IsLike(searchParams.Address)));
            }
        }
    }


    /// <summary>
    /// AP number
    /// </summary>
    public class APNoFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.APNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PolicerOfficerLog)}.{nameof(PolicerOfficerLog.IdentificationNumber)}", searchParams.APNumber));
            }
        }
    }


    /// <summary>
    /// File number
    /// </summary>
    public class RequestRefFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.RequestRef))
            {
               
                if (addAlias) { criteria.CreateAlias(nameof(PoliceOfficerDeploymentLog.Request), "Request"); }
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.Request.FileRefNumber == searchParams.RequestRef));
            }
        }
    }


    /// <summary>
    /// Customer name filter
    /// </summary>
    public class OfficerNameFilter : IOfficerDeploymentReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if (!string.IsNullOrEmpty(searchParams.OfficerName))
            {
                //var commandCriteria = DetachedCriteria.For<PoliceOfficerDeploymentLog>("cm")
                //   .Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.PoliceOfficer.PoliceOfficerLogs.Where(y => y.Name.IsLike("%"+searchParams.OfficerName+"%")).Count() > 0))
                //   .SetProjection(Projections.Constant(1));
                //criteria.Add(Subqueries.Exists(commandCriteria));

                //.Add(Restrictions.Disjunction()
                //.Add(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("Command.Id")))

                //var commandCriteria = DetachedCriteria.For<PolicerOfficerLog>("PolicerOfficerLog")
                //    .CreateAlias("PoliceOfficer", "PoliceOfficer")
                //    //.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.PoliceOfficer.Id != 0))
                //    //.Add(Restrictions.Where<PolicerOfficerLog>(x => (x.Name.IsLike("%" + searchParams.OfficerName + "%"))))
                //    .Add(Restrictions.Disjunction()
                //    .Add((Restrictions.And(Restrictions.EqProperty("PolicerOfficerLog.Id", "PoliceOfficer.Id"), Restrictions.Where<PolicerOfficerLog>(x => (x.Name.IsLike("%" + searchParams.OfficerName + "%"))))))
                //    )
                //    .SetProjection(Projections.Property("PoliceOfficer.Id"));
                //criteria.Add(Subqueries.Exists(commandCriteria));

                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.OfficerName.IsLike("%" + searchParams.OfficerName + "%")));

            }
        }
    }

    public class CommandFilter : IOfficerDeploymentReportFilters
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

        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentSearchParams searchParams, bool addAlias = false)
        {
            if(DoCheckForAllCommands(searchParams.CommandId, searchParams.LGA))
            {
                var allCommandsCriteria = DetachedCriteria.For<Command>("Cmd")
                    .Add(Restrictions.And(Restrictions.EqProperty("Id", $"{nameof(PoliceOfficerDeploymentLog)}.Command.Id"), Restrictions.Eq("LGA.Id", searchParams.LGA)))
                    .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(allCommandsCriteria));

            }
            else if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.Command == new Command { Id = searchParams.CommandId }));
            }
            else if (DoCheckForNoCommands(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceOfficerDeploymentLog>(x => x.Command == new Command { Id = 0 }));
            }
        }
    }

}