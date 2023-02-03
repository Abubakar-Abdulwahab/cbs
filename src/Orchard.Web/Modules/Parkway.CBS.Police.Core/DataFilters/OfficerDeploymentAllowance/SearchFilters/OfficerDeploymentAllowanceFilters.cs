using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance.SearchFilters
{
    /// <summary>
    /// Account number filter
    /// </summary>
    public class AccountNumberFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.AccountNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.AccountNumber)}", searchParams.AccountNumber));
            }
        }
    }

    /// <summary>
    /// Request allowance status
    /// </summary>
    public class RequestStatusFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (searchParams.RequestStatus != Models.Enums.DeploymentAllowanceStatus.None)
            {
                criteria.Add(Restrictions.Eq("Status", (int)searchParams.RequestStatus));
            }
        }
    }

    /// <summary>
    /// Invoice number filter
    /// </summary>
    public class InvoiceNumberFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PoliceofficerDeploymentAllowance.Invoice)}.{nameof(PoliceofficerDeploymentAllowance.Invoice.InvoiceNumber)}", searchParams.InvoiceNumber));
            }
        }
    }


    /// <summary>
    /// IPPIS number
    /// </summary>
    public class IPPISNoFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.IPPISNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.IPPISNumber)}", searchParams.IPPISNumber));
            }
        }
    }


    /// <summary>
    /// IPPIS number
    /// </summary>
    public class APNoFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.APNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.IdentificationNumber)}", searchParams.APNumber));
            }
        }
    }


    /// <summary>
    /// Police rank
    /// </summary>
    public class RankFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (searchParams.RankId > 0)
            {
                criteria.Add(Restrictions.Eq($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Rank)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Rank.Id)}", searchParams.RankId));
            }
        }
    }



    /// <summary>
    /// File number
    /// </summary>
    public class RequestRefFilter : IOfficerDeploymentAllowanceFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.FileNumber))
            {
                criteria.Add(Restrictions.Eq($"{nameof(PoliceofficerDeploymentAllowance.Request)}.{nameof(PoliceofficerDeploymentAllowance.Request.FileRefNumber)}", searchParams.FileNumber));
            }
        }
    }

    public class CommandFilter : IOfficerDeploymentAllowanceFilters
    {
        private bool DoCheck(int commandId)
        {
            return commandId != 0;
        }

        private bool DoCheckForAllCommands(int commandId, int lgaId)
        {
            return commandId == 0 && lgaId != 0;
        }

        public void AddCriteriaRestriction(ICriteria criteria, OfficerDeploymentAllowanceSearchParams searchParams)
        {
            if (DoCheckForAllCommands(searchParams.CommandId, searchParams.LGA))
            {
                var allCommandsCriteria = DetachedCriteria.For<Command>("Cmd")
                    .Add(Restrictions.And(Restrictions.EqProperty("Id", $"{nameof(PoliceofficerDeploymentAllowance)}.{nameof(PoliceofficerDeploymentAllowance.Command)}.{nameof(PoliceofficerDeploymentAllowance.Command.Id)}"), Restrictions.Eq("LGA.Id", searchParams.LGA)))
                    .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(allCommandsCriteria));

            }
            else if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceofficerDeploymentAllowance>(x => x.Command == new Command { Id = searchParams.CommandId }));
            }
        }
    }


}