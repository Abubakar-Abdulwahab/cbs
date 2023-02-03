using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.DataFilters.CollectionReport.SearchFilters
{
    public class InvoiceNumberFilter : IPSSCollectionReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.TransactionLog.InvoiceNumber == searchParams.InvoiceNumber));
            }
        }
    }

    /// <summary>
    /// filter for file number
    /// </summary>
    public class FileNumberFilter : IPSSCollectionReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.FileNumber))
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.Request.FileRefNumber == searchParams.FileNumber));
            }
        }
    }

    public class ReceiptNumberFilter : IPSSCollectionReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.ReceiptNumber))
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.TransactionLog.ReceiptNumber == searchParams.ReceiptNumber));
            }
        }
    }

    /// <summary>
    /// filter for approval number
    /// </summary>
    public class PaymentRefFilter : IPSSCollectionReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.PaymentRef))
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.TransactionLog.PaymentReference == searchParams.PaymentRef));
            }
        }
    }

    public class CommandFilter : IPSSCollectionReportFilters
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

        public void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams)
        {
            if (DoCheckForAllCommands(searchParams.CommandId, searchParams.LGA))
            {
                var allCommandsCriteria = DetachedCriteria.For<Command>("Cmd")
                    .Add(Restrictions.And(Restrictions.EqProperty("Id", "PCL.Command.Id"), Restrictions.Eq("LGA.Id", searchParams.LGA)))
                    .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(allCommandsCriteria));

            }
            else if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.Request.Command == new Command { Id = searchParams.CommandId }));
            }
            else if (DoCheckForNoCommands(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.Request.Command == new Command { Id = 0 }));
            }
        }
    }

    public class RevenueHeadFilter : IPSSCollectionReportFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSCollectionSearchParams searchParams)
        {
            if (searchParams.RevenueHeadId != 0)
            {
                criteria.Add(Restrictions.Where<PoliceCollectionLog>(x => x.TransactionLog.RevenueHead.Id == searchParams.RevenueHeadId));
            }
        }
    }
}