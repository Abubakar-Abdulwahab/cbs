using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportBreakdown.SearchFilters
{
    /// <summary>
    /// PSS Settlement Fee Party Filter
    /// </summary>
    public class SettlementPartyFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (searchParams.SelectedSettlementParty > 0)
            {
                criteria.Add(Restrictions.Eq($"{nameof(PSSFeeParty)}.{nameof(PSSFeeParty.Id)}", searchParams.SelectedSettlementParty));
            }
        }
    }


    /// <summary>
    /// File Ref Number Filter
    /// </summary>
    public class FileNumberFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.FileNumber))
            {
                criteria.Add(Restrictions.InsensitiveLike($"{nameof(PSSRequest)}.{nameof(PSSRequest.FileRefNumber)}", searchParams.FileNumber, MatchMode.Anywhere));
            }
        }
    }


    /// <summary>
    /// Invoice Number Filter
    /// </summary>
    public class InvoiceNumberFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.InvoiceNumber))
            {
                criteria.Add(Restrictions.InsensitiveLike($"{nameof(Invoice)}.{nameof(Invoice.InvoiceNumber)}", searchParams.InvoiceNumber, MatchMode.Anywhere));
            }
        }
    }


    /// <summary>
    /// PSService Filter
    /// </summary>
    public class ServiceFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (searchParams.SelectedService > 0)
            {
                criteria.Add(Restrictions.Eq($"{nameof(PSService)}.{nameof(PSService.Id)}", searchParams.SelectedService));
            }
        }
    }


    /// <summary>
    /// State Filter
    /// </summary>
    public class StateFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (searchParams.SelectedState > 0)
            {
                criteria.CreateAlias(nameof(PSSSettlementBatchItems.State),nameof(StateModel)).Add(Restrictions.Eq($"{nameof(StateModel)}.{nameof(StateModel.Id)}", searchParams.SelectedState));
            }
        }
    }


    /// <summary>
    /// LGA Filter
    /// </summary>
    public class LGAFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (searchParams.SelectedLGA > 0)
            {
                criteria.CreateAlias(nameof(PSSSettlementBatchItems.LGA), nameof(LGA)).Add(Restrictions.Eq($"{nameof(LGA)}.{nameof(LGA.Id)}", searchParams.SelectedLGA));
            }
        }
    }


    /// <summary>
    /// Command Filter
    /// </summary>
    public class CommandFilter : IPSSSettlementReportBreakdownFilters
    {
        public void AddCriteriaRestriction(ICriteria criteria, PSSSettlementReportBreakdownSearchParams searchParams)
        {
            if (searchParams.SelectedCommand > 0)
            {
                criteria.CreateAlias(nameof(PSSSettlementBatchItems.SettlementCommand), nameof(Command)).Add(Restrictions.Eq($"{nameof(Command)}.{nameof(Command.Id)}", searchParams.SelectedCommand));
            }
        }
    }
}