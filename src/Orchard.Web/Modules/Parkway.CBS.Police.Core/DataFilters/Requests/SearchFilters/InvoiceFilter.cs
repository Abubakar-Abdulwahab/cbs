using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{
    public class InvoiceFilter : IInvoiceFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IPoliceRequestSearchFilter>> _searchFilters;

        public InvoiceFilter(IOrchardServices orchardService, IEnumerable<Lazy<IPoliceRequestSearchFilter>> searchFilters, Lazy<IInvoiceFilter> invoiceFilter)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        public dynamic GetRequestReportInvoiceVM(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            try
            {
                IEnumerable<PSSRequestVM> result = GetReport(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions).ToList();
                if (result != null && result.Any())
                {
                    returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                    returnOBJ.TotalRecordCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 1 } };
                    returnOBJ.TotalInvoiceCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 1 } };
                    returnOBJ.TotalInvoiceAmount = new List<ReportStatsVM> { new ReportStatsVM { TotalAmount = result.ElementAt(0).InvoiceAmount } };
                }
                else
                {
                    returnOBJ.ReportRecords = new List<PSSRequestVM>();
                    returnOBJ.TotalRecordCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                    returnOBJ.TotalInvoiceCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                    returnOBJ.TotalInvoiceAmount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                }
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<PSSRequestVM>();
                returnOBJ.TotalRecordCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                returnOBJ.TotalInvoiceCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                returnOBJ.TotalInvoiceAmount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
            }
            return returnOBJ;
        }


        private IEnumerable<PSSRequestVM> GetReport(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            ICriteria criteria = _transactionManager.GetSession()
                .CreateCriteria<PSSRequestInvoice>("PSRI")
                .CreateAlias(nameof(PSSRequestInvoice.Invoice), "Invoice")
                .CreateAlias(nameof(PSSRequestInvoice.Request), "Request")
                .CreateAlias(nameof(PSSRequestInvoice.Request) + "." + nameof(PSSRequestInvoice.Request.TaxEntity), "TaxEntity")
                .CreateAlias(nameof(PSSRequestInvoice.Request) + "." + nameof(PSSRequestInvoice.Request.Service), "Service")
                .Add(Restrictions.Between(nameof(PSSRequestInvoice.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate))
                .Add(Restrictions.Where<PSSRequestInvoice>(x => x.Invoice.InvoiceNumber == searchParams.RequestOptions.InvoiceNumber))
                .Add(Restrictions.Where<PSSRequestInvoice>(x => x.Invoice.Status != (int)InvoiceStatus.WriteOff))
                .SetProjection(Projections.ProjectionList()
           .Add(Projections.Property("Request.FileRefNumber"), nameof(PSSRequestVM.FileRefNumber))
           .Add(Projections.Property("Request.ApprovalNumber"), nameof(PSSRequestVM.ApprovalNumber))
           .Add(Projections.Property("Request.Status"), nameof(PSSRequestVM.StatusId))
           .Add(Projections.Property("TaxEntity.Recipient"), nameof(PSSRequestVM.CustomerName))
           .Add(Projections.Property("Request.Id"), nameof(PSSRequestVM.Id))
           .Add(Projections.Property("Service.ServiceType"), nameof(PSSRequestVM.ServiceTypeId))
           .Add(Projections.Property("Service.Name"), nameof(PSSRequestVM.ServiceName))
           .Add(Projections.Property("Request.CreatedAtUtc"), nameof(PSSRequestVM.RequestDate))
           .Add(Projections.Property("Request.UpdatedAtUtc"), nameof(PSSRequestVM.LastActionDate))
           .Add(Projections.Property("Invoice.Amount"), nameof(PSSRequestVM.InvoiceAmount))
           );

            foreach (var filter in _searchFilters)
            {
                filter.Value.AddCriteriaRestrictionForInvoiceNumber(criteria, searchParams);
            }



            if (applyAccessRestrictions)
            {
                var pRequestCriteria = DetachedCriteria.For<PSSRequestInvoice>("pr")
                    .Add(Restrictions.EqProperty("Id", "PSRI.Request.Id"))
                    .SetProjection(Projections.Constant(1));

                if (applyApprovalAccessRestrictions)
                {
                    var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>("PRF")
                    .Add(Restrictions.Eq("AssignedApprover.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("FlowDefinitionLevel.Id", "pr.FlowDefinitionLevel.Id"))
                    .SetProjection(Projections.Constant(1));
                    criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
                }

                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "pr.Command.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Service.Id", "PR.Service.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.EqProperty("Service.Id", "pr.Service.Id"))))
                .SetProjection(Projections.Constant(1));

                var accessRoleUserCriteria = DetachedCriteria.For<ApprovalAccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("Id", "aal.ApprovalAccessRoleUser.Id"))
                    .SetProjection(Projections.Constant(1));


                accessListCriteria.Add(Subqueries.Exists(accessRoleUserCriteria));
                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
                pRequestCriteria.Add(Subqueries.Exists(commandCriteria));
                criteria.Add(Subqueries.Exists(pRequestCriteria));
            }

            return criteria.SetResultTransformer(Transformers.AliasToBean<PSSRequestVM>()).Future<PSSRequestVM>();
        }

    }
}