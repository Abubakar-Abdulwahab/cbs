using System;
using Orchard;
using NHibernate;
using Orchard.Data;
using System.Dynamic;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.Requests.Contracts;
using Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Linq;
using System.Text;

namespace Parkway.CBS.Police.Core.DataFilters.Requests
{
    public class AdminRequestFilter : IAdminRequestFilter
    {
        private readonly IEnumerable<Lazy<IAdminApprovalRequestSearchFilter>> _searchFilters;
        private readonly ITransactionManager _transactionManager;

        public AdminRequestFilter(IOrchardServices orchardService, IEnumerable<Lazy<IAdminApprovalRequestSearchFilter>> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get pending PSS request statistics count for logon admin user
        /// </summary>
        /// <param name="status"></param>
        /// <returns>IEnumerable{ReportStatsVM}</returns>
        public IEnumerable<ReportStatsVM> GetAdminRequestStatistics(PSSRequestStatus status, int adminUserId, int accessRoleUserId, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions, bool applyDateFilter = false, bool checkWorkFlowLogActiveStatus = true)
        {
            DateTime today = DateTime.Now.ToLocalTime();

            return GetCriteria(new RequestsReportSearchParams
            {
                AdminUserId = adminUserId,
                StartDate = new DateTime(today.Year, today.Month, 1),
                EndDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddDays(1).AddSeconds(-1),
                RequestOptions = new RequestOptions { RequestStatus = status },
                ApprovalAccessRoleUserId = accessRoleUserId,
                CheckWorkFlowLogActiveStatus = checkWorkFlowLogActiveStatus
            }, applyAccessRestrictions, applyApprovalAccessRestrictions, applyDateFilter)

            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Count<PSSRequest>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
                    ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Build criteria for request filter
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>ICriteria</returns>
        public ICriteria GetCriteria(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions, bool applyDateFilter = true)
        {
            ICriteria criteria = _transactionManager.GetSession()
              .CreateCriteria<RequestCommandWorkFlowLog>(nameof(RequestCommandWorkFlowLog))
              .CreateAlias(nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Request), nameof(RequestCommandWorkFlowLog.Request));

            if (applyDateFilter)
            {
                criteria.Add(Restrictions.Between(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));
            }

            if (searchParams.CheckWorkFlowLogActiveStatus)
            {
                criteria.Add(Restrictions.Eq(nameof(RequestCommandWorkFlowLog.IsActive), true));
            }

            if(searchParams.SelectedRequestPhase > 0)
            {
                criteria.Add(Restrictions.Eq(nameof(RequestCommandWorkFlowLog.RequestPhaseId), searchParams.SelectedRequestPhase));
            }

            if (applyApprovalAccessRestrictions)
            {
                var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>(nameof(PSServiceRequestFlowApprover))
                .Add(Restrictions.Eq(nameof(PSServiceRequestFlowApprover.AssignedApprover) + "." + nameof(PSServiceRequestFlowApprover.AssignedApprover.Id), searchParams.AdminUserId))
                .Add(Restrictions.Eq(nameof(PSServiceRequestFlowApprover.IsDeleted), false))
                .Add(Restrictions.EqProperty(nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel) + "." + nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel.Id),
                nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.DefinitionLevel) + "." + nameof(RequestCommandWorkFlowLog.DefinitionLevel.Id)))
                .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
            }

            foreach (var filter in _searchFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            var commandCriteria = DetachedCriteria.For<Command>("cm");

            if (searchParams.CommandId > 0)
            {
                commandCriteria
                    .Add(Restrictions.EqProperty(nameof(Command.Id), nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Command) + "." + nameof(RequestCommandWorkFlowLog.Command.Id)))
                    .Add(Restrictions.Eq(nameof(Command.Id), searchParams.CommandId))
                    .SetProjection(Projections.Constant(1));
            }
            else
            {
                commandCriteria
                   .Add(Restrictions.EqProperty(nameof(Command.Id), nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Command) + "." + nameof(RequestCommandWorkFlowLog.Command.Id)))
                   .SetProjection(Projections.Constant(1));
            }

            if (searchParams.LGA != 0)
            {
                //commandCriteria
                //     .Add(Restrictions.Eq(nameof(Command) + "." + nameof(RequestCommandWorkFlowLog.Command.State) + "." + nameof(RequestCommandWorkFlowLog.Command.State.Id), searchParams.State))
                //    .Add(Restrictions.Eq(nameof(Command) + "." + nameof(RequestCommandWorkFlowLog.Command.LGA) + "." + nameof(RequestCommandWorkFlowLog.Command.LGA.Id), searchParams.LGA));

                commandCriteria
                     .Add(Restrictions.Eq("cm." + nameof(RequestCommandWorkFlowLog.Command.State) + "." + nameof(RequestCommandWorkFlowLog.Command.State.Id), searchParams.State))
                    .Add(Restrictions.Eq("cm." + nameof(RequestCommandWorkFlowLog.Command.LGA) + "." + nameof(RequestCommandWorkFlowLog.Command.LGA.Id), searchParams.LGA));
            }
            else if ((searchParams.State != 0 && searchParams.LGA == 0))
            {
                //commandCriteria
                //    .Add(Restrictions.Eq(nameof(Command) + "." + nameof(RequestCommandWorkFlowLog.Command.State) + "." + nameof(RequestCommandWorkFlowLog.Command.State.Id), searchParams.State));
                commandCriteria
                   .Add(Restrictions.Eq("cm." + nameof(RequestCommandWorkFlowLog.Command.State) + "." + nameof(RequestCommandWorkFlowLog.Command.State.Id), searchParams.State));
            }

            if (applyAccessRestrictions)
            {
                ///Query equivalent without joining PSServiceRequestFlowApprover table///
                //////////////////////////////
                /*SELECT TOP 10 rqe.FileRefNumber as FileRefNumber, rqe.ApprovalNumber
                 as ApprovalNumber, this_.DefinitionLevel_Id as deflevel, this_.Command_Id, rqe.Status as Status, taxentity2_.Recipient as Recipient, this_.Id as Id, service1_.ServiceType as ServiceType, service1_.Name as Name, this_.CreatedAtUtc as CreatedAtUtc, this_.UpdatedAtUtc as 
                 UpdatedAtUtc 
                 FROM Parkway_CBS_Police_Core_RequestCommandWorkFlowLog this_
                 inner join Parkway_CBS_Police_Core_PSSRequest rqe on this_.Request_Id = rqe.Id
                 inner join Parkway_CBS_Police_Core_PSService service1_ on rqe.Service_Id = service1_.Id 
                 inner join Parkway_CBS_Core_TaxEntity 
                 taxentity2_ on rqe.TaxEntity_id=taxentity2_.Id WHERE this_.IsActive = 1 AND  rqe.CreatedAtUtc between '2001-11-01 11:27:53' and '2022-02-01 23:59:59' and exists 

                 (SELECT 1 as y0_ FROM 
                 Parkway_CBS_Police_Core_PSServiceRequestFlowApprover this_0_ WHERE this_0_.AssignedApprover_id = 2 and this_0_.FlowDefinitionLevel_id = this_.DefinitionLevel_Id) and rqe.Status = 6 and exists 

                 (SELECT 1 as y0_ FROM Parkway_CBS_Police_Core_Command this_0_ WHERE this_0_.Id = this_.Command_id and exists 

                     (SELECT 1 as y0_ FROM Parkway_CBS_Police_Core_ApprovalAccessList this_0_0_ WHERE 

                     (((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id is null) and this_0_0_.LGA_id is null) and this_0_0_.Command_id is null) and this_0_0_.Service_id is null) or ((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id is null) and this_0_0_.Command_id is null) and this_0_0_.Service_id is null) or ((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Command_id is null) and this_0_0_.Service_id is null) or ((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Command_id = this_0_.Id) and this_0_0_.Service_id is null) or ((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Service_id = rqe.Service_id) and this_0_0_.Command_id is null) or ((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Command_id = this_0_.Id) and this_0_0_.Service_id = rqe.Service_id)) and exists 

                     (SELECT 1 as y0_ FROM Parkway_CBS_Police_Core_ApprovalAccessRoleUser this_0_0_0_ WHERE this_0_0_0_.User_id = 2 and this_0_0_0_.Id = this_0_0_.ApprovalAccessRoleUser_id))) ORDER BY rqe.UpdatedAtUtc desc;*/

                
                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>(nameof(ApprovalAccessList))
                .Add(Restrictions.Eq(nameof(ApprovalAccessList.IsDeleted), false))
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And
                (Restrictions.And
                    (Restrictions.And
                        (Restrictions.And
                            (Restrictions.EqProperty(nameof(ApprovalAccessList.CommandCategory) + "." + nameof(ApprovalAccessList.CommandCategory.Id), "cm." + nameof(Command.CommandCategory) + "." + nameof(Command.CommandCategory.Id)),
                            Restrictions.IsNull(nameof(ApprovalAccessList.State) + "." + nameof(ApprovalAccessList.State.Id))),
                        Restrictions.IsNull(nameof(ApprovalAccessList.LGA) + "." + nameof(ApprovalAccessList.LGA.Id))),
                    Restrictions.IsNull(nameof(ApprovalAccessList.Command) + "." + nameof(ApprovalAccessList.Command.Id))),
                Restrictions.IsNull(nameof(ApprovalAccessList.Service) + "." + nameof(ApprovalAccessList.Service.Id))))

                .Add(Restrictions.And
                    (Restrictions.And
                        (Restrictions.And(Restrictions.And
                            (Restrictions.EqProperty(nameof(ApprovalAccessList.CommandCategory) + "." + nameof(ApprovalAccessList.CommandCategory.Id), "cm." + nameof(Command.CommandCategory) + "." + nameof(Command.CommandCategory.Id)), Restrictions.EqProperty(nameof(ApprovalAccessList.State) + "." + nameof(ApprovalAccessList.State.Id), "cm." + nameof(Command.State) + "." + nameof(Command.State.Id))),
                        Restrictions.IsNull(nameof(ApprovalAccessList.LGA) + "." + nameof(ApprovalAccessList.LGA.Id))), Restrictions.IsNull("cm." + nameof(Command.Id))),
                    Restrictions.IsNull(nameof(ApprovalAccessList.Service) + "." + nameof(ApprovalAccessList.Service.Id))))

                .Add(Restrictions.And
                    (Restrictions.And
                        (Restrictions.And
                            (Restrictions.And
                                (Restrictions.EqProperty(nameof(ApprovalAccessList.CommandCategory) + "." + nameof(ApprovalAccessList.CommandCategory.Id), "cm." + nameof(Command.CommandCategory) + "." + nameof(Command.CommandCategory.Id)), Restrictions.EqProperty(nameof(ApprovalAccessList.State) + "." + nameof(ApprovalAccessList.State.Id), "cm." + nameof(Command.State) + "." + nameof(Command.State.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.LGA) + "." + nameof(ApprovalAccessList.LGA.Id), "cm." + nameof(Command.LGA) + "." + nameof(Command.LGA.Id))),
                         Restrictions.IsNull(nameof(ApprovalAccessList.Command) + "." + nameof(ApprovalAccessList.Command.Id))), Restrictions.IsNull(nameof(ApprovalAccessList.Service) + "." + nameof(ApprovalAccessList.Service.Id))))
                .Add(Restrictions.And
                (Restrictions.And
                    (Restrictions.And(Restrictions.And
                        (Restrictions.EqProperty(nameof(ApprovalAccessList.CommandCategory) + "." + nameof(ApprovalAccessList.CommandCategory.Id), "cm." + nameof(Command.CommandCategory) + "." + nameof(Command.CommandCategory.Id)), Restrictions.EqProperty(nameof(ApprovalAccessList.State) + "." + nameof(ApprovalAccessList.State.Id), "cm." + nameof(Command.State) + "." + nameof(Command.State.Id))),
                        Restrictions.EqProperty(nameof(ApprovalAccessList.LGA) + "." + nameof(ApprovalAccessList.LGA.Id), "cm." + nameof(Command.LGA) + "." + nameof(Command.LGA.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.Command) + "." + nameof(ApprovalAccessList.Command.Id), "cm." + nameof(Command.Id))), Restrictions.IsNull(nameof(ApprovalAccessList.Service) + "." + nameof(ApprovalAccessList.Service.Id))))
                .Add(Restrictions.And
                    (Restrictions.And
                        (Restrictions.And(Restrictions.And(Restrictions.EqProperty(nameof(ApprovalAccessList.CommandCategory) + "." + nameof(ApprovalAccessList.CommandCategory.Id), "cm." + nameof(Command.CommandCategory) + "." + nameof(Command.CommandCategory.Id)), Restrictions.EqProperty(nameof(ApprovalAccessList.State) + "." + nameof(ApprovalAccessList.State.Id), "cm." + nameof(Command.State) + "." + nameof(Command.State.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.LGA) + "." + nameof(ApprovalAccessList.LGA.Id), "cm." + nameof(Command.LGA) + "." + nameof(Command.LGA.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.Service) + "." + nameof(ApprovalAccessList.Service.Id), nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Service) + "." + nameof(RequestCommandWorkFlowLog.Request.Service.Id))), Restrictions.IsNull(nameof(ApprovalAccessList.Command) + "." + nameof(RequestCommandWorkFlowLog.Command.Id))))
                .Add(Restrictions.And(Restrictions.And
                    (Restrictions.And
                        (Restrictions.And
                            (Restrictions.EqProperty(nameof(ApprovalAccessList.CommandCategory) + "." + nameof(ApprovalAccessList.CommandCategory.Id), "cm." + nameof(Command.CommandCategory) + "." + nameof(Command.CommandCategory.Id)), Restrictions.EqProperty(nameof(ApprovalAccessList.State) + "." + nameof(ApprovalAccessList.State.Id), "cm." + nameof(Command.State) + "." + nameof(Command.State.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.LGA) + "." + nameof(ApprovalAccessList.LGA.Id), "cm." + nameof(Command.LGA) + "." + nameof(Command.LGA.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.Command) + "." + nameof(ApprovalAccessList.Command.Id), "cm." + nameof(Command.Id))), Restrictions.EqProperty(nameof(ApprovalAccessList.Service) + "." + nameof(ApprovalAccessList.Service.Id), nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Service) + "." + nameof(RequestCommandWorkFlowLog.Request.Service.Id)))))
                .SetProjection(Projections.Constant(1));

                var accessRoleUserCriteria = DetachedCriteria.For<ApprovalAccessRoleUser>(nameof(ApprovalAccessRoleUser))
                   .Add(Restrictions.Eq(nameof(ApprovalAccessRoleUser.Id), searchParams.ApprovalAccessRoleUserId))
                   .Add(Restrictions.EqProperty(nameof(ApprovalAccessRoleUser.Id), nameof(ApprovalAccessList) + "." + nameof(ApprovalAccessList.ApprovalAccessRoleUser) + "." + nameof(ApprovalAccessList.ApprovalAccessRoleUser.Id)))
                   .SetProjection(Projections.Constant(1));

                accessListCriteria.Add(Subqueries.Exists(accessRoleUserCriteria));
                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
            }
            criteria.Add(Subqueries.Exists(commandCriteria));

            return criteria;
        }


        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetRecordsBasedOnActiveWorkFlow(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalRecordCount = GetAggregateTotalRecordCount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<PSSRequestVM>();
                returnOBJ.TotalRecordCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
            }
            return returnOBJ;

        }

        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetRecordsBasedOnWorkFlow(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            try
            {
                returnOBJ.ReportRecords = GetRequestReport(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalRecordCount = GetAggregateTotalRecordCount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalInvoiceCount = GetAggregateInvoiceCount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalInvoiceAmount = GetAggregateInvoiceAmount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
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


        /// <summary>
        /// Get the aggregate of the request invoices
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateInvoiceCount(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            query.CreateAlias($"{nameof(PSSRequestInvoice.Request)}.{nameof(PSSRequestInvoice.Request.Invoices)}", nameof(PSSRequestInvoice))
                .CreateAlias($"{nameof(PSSRequestInvoice)}.{nameof(PSSRequestInvoice.Request)}", "PSSRequestInvoiceReq")
                .CreateAlias($"{nameof(PSSRequestInvoice)}.{nameof(PSSRequestInvoice.Invoice)}", "Invoice")
                .Add(Restrictions.EqProperty("PSSRequestInvoiceReq.Id", $"{nameof(RequestCommandWorkFlowLog.Request)}.{nameof(RequestCommandWorkFlowLog.Request.Id)}"));

            return query.SetProjection(
                Projections.ProjectionList()
                    .Add(Projections.CountDistinct($"{nameof(PSSRequestInvoice)}.Id"), "TotalRecordCount")
                ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the aggregate of the request invoice amount
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateInvoiceAmount(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder additionalClause = new StringBuilder();
            StringBuilder additionalClauseForSubQuery = new StringBuilder();
            StringBuilder additionalClauseNoAccessRestrictions = new StringBuilder();

            if (searchParams.IntValueSelectedServiceId != 0)
            {
                additionalClause.Append($" and request1_.Service_Id = {searchParams.IntValueSelectedServiceId}");
            }

            if (!string.IsNullOrEmpty(searchParams.RequestOptions.FileNumber))
            {
                additionalClause.Append($" and request1_.FileRefNumber = '{searchParams.RequestOptions.FileNumber}'");
            }

            if (searchParams.RequestOptions.RequestStatus != PSSRequestStatus.None)
            {
                additionalClause.Append($" and request1_.Status = {(int)searchParams.RequestOptions.RequestStatus}");
            }

            if(searchParams.LGA != 0)
            {
                additionalClauseForSubQuery.Append($" and this_0_.State_id = {searchParams.State} and this_0_.LGA_id = {searchParams.LGA}");
            }

            if(searchParams.State != 0 && searchParams.LGA == 0)
            {
                additionalClauseForSubQuery.Append($" and this_0_.State_id = {searchParams.State}");
            }

            if(searchParams.CommandId > 0)
            {
                additionalClause.Append($" and request1_.Command_Id = {searchParams.CommandId}");
            }

            if (applyAccessRestrictions)
            {
                additionalClause.Append($" and exists (SELECT 1 as y0_ FROM Parkway_CBS_Police_Core_Command this_0_ WHERE this_0_.Id = this_.Command_id and exists(SELECT 1 as y0_ FROM Parkway_CBS_Police_Core_ApprovalAccessList this_0_0_ WHERE this_0_0_.IsDeleted = {0} and (((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id is null) and this_0_0_.LGA_id is null) and this_0_0_.Command_id is null) and this_0_0_.Service_id is null) or((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id is null) and this_0_.Id is null) and this_0_0_.Service_id is null) or((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Command_id is null) and this_0_0_.Service_id is null) or((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Command_id = this_0_.Id) and this_0_0_.Service_id is null) or((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Service_id = request1_.Service_id) and this_0_0_.Command_id is null) or((((this_0_0_.CommandCategory_id = this_0_.CommandCategory_id and this_0_0_.State_id = this_0_.State_id) and this_0_0_.LGA_id = this_0_.LGA_id) and this_0_0_.Command_id = this_0_.Id) and this_0_0_.Service_id = request1_.Service_id)) and exists(SELECT 1 as y0_ FROM Parkway_CBS_Police_Core_ApprovalAccessRoleUser this_0_0_0_ WHERE this_0_0_0_.Id = {searchParams.ApprovalAccessRoleUserId} and this_0_0_0_.Id = this_0_0_.ApprovalAccessRoleUser_id)) {additionalClauseForSubQuery})");
            }
            else
            {
                additionalClauseNoAccessRestrictions.Append($" inner join Parkway_CBS_Police_Core_Command this_0_ on this_0_.Id = this_.Command_id ");
                additionalClause.Append(additionalClauseForSubQuery);
            }

            sb.Append($"SELECT SUM(this_.Amount) as TotalAmount FROM Parkway_CBS_Core_Invoice this_ inner join Parkway_CBS_Police_Core_PSSRequestInvoice pssrequest2_ on this_.Id = pssrequest2_.Invoice_Id inner join Parkway_CBS_Police_Core_PSSRequest request1_ on pssrequest2_.Request_id = request1_.Id WHERE request1_.Id IN (SELECT distinct request1_.Id FROM Parkway_CBS_Police_Core_RequestCommandWorkFlowLog this_ inner join Parkway_CBS_Police_Core_PSSRequest request1_ on this_.Request_id= request1_.Id {additionalClauseNoAccessRestrictions} WHERE request1_.CreatedAtUtc between :startDate and :endDate {additionalClause})");

            var query = _transactionManager.GetSession().CreateSQLQuery(sb.ToString());
            query.SetParameter("startDate", searchParams.StartDate);
            query.SetParameter("endDate", searchParams.EndDate);
            return query.SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).List<ReportStatsVM>();
        }


        public IEnumerable<ReportStatsVM> GetAggregateTotalRecordCount(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            return GetCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions)
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.CountDistinct($"{nameof(RequestCommandWorkFlowLog.Request)}.{nameof(RequestCommandWorkFlowLog.Id)}"), nameof(ReportStatsVM.TotalRecordCount))
                    ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get requests to be displayed on the approval list, this query doesn't get distinct items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns>IEnumerable<PSSRequestVM></returns>
        private IEnumerable<PSSRequestVM> GetReport(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {

            ICriteria query = GetCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            string orderByColumnName = (string.IsNullOrEmpty(searchParams.OrderByColumnName) ? "Id" : searchParams.OrderByColumnName);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }


            return query
                .AddOrder(Order.Asc(orderByColumnName))
               .CreateAlias(nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Service), nameof(RequestCommandWorkFlowLog.Request.Service))
               .CreateAlias(nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.CBSUser), nameof(RequestCommandWorkFlowLog.Request.CBSUser))
               .SetProjection(Projections.ProjectionList()
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.FileRefNumber)), nameof(PSSRequestVM.FileRefNumber))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.ApprovalNumber)), nameof(PSSRequestVM.ApprovalNumber))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Status)), nameof(PSSRequestVM.StatusId))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request.CBSUser) + "." + nameof(RequestCommandWorkFlowLog.Request.CBSUser.Name)), nameof(PSSRequestVM.CustomerName))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Id)), nameof(PSSRequestVM.Id))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request.Service) + "." + nameof(RequestCommandWorkFlowLog.Request.Service.ServiceType)), nameof(PSSRequestVM.ServiceTypeId))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request.Service) + "." + nameof(RequestCommandWorkFlowLog.Request.Service.Name)), nameof(PSSRequestVM.ServiceName))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.CreatedAtUtc)), nameof(PSSRequestVM.RequestDate))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.UpdatedAtUtc)), nameof(PSSRequestVM.LastActionDate))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.RequestPhaseName)), nameof(PSSRequestVM.RequestPhaseName))
               ).SetResultTransformer(Transformers.AliasToBean<PSSRequestVM>()).Future<PSSRequestVM>();
        }

        /// <summary>
        /// Get requests to be displayed on the report list, this query gets distinct items and doesn't include RequestPhaseName property in the projections
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <param name="applyApprovalAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<PSSRequestVM> GetRequestReport(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {

            ICriteria query = GetCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            string orderByColumnName = (string.IsNullOrEmpty(searchParams.OrderByColumnName) ? "Id" : searchParams.OrderByColumnName);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .AddOrder(Order.Desc(orderByColumnName))
               .CreateAlias(nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Service), nameof(RequestCommandWorkFlowLog.Request.Service))
               .CreateAlias(nameof(RequestCommandWorkFlowLog) + "." + nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.CBSUser), nameof(RequestCommandWorkFlowLog.Request.CBSUser))
               .SetProjection(Projections.Distinct(Projections.ProjectionList()
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Id)), nameof(PSSRequestVM.Id))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.FileRefNumber)), nameof(PSSRequestVM.FileRefNumber))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.ApprovalNumber)), nameof(PSSRequestVM.ApprovalNumber))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Status)), nameof(PSSRequestVM.StatusId))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request.CBSUser) + "." + nameof(RequestCommandWorkFlowLog.Request.CBSUser.Name)), nameof(PSSRequestVM.CustomerName))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Id)), nameof(PSSRequestVM.Id))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request.Service) + "." + nameof(RequestCommandWorkFlowLog.Request.Service.ServiceType)), nameof(PSSRequestVM.ServiceTypeId))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request.Service) + "." + nameof(RequestCommandWorkFlowLog.Request.Service.Name)), nameof(PSSRequestVM.ServiceName))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.CreatedAtUtc)), nameof(PSSRequestVM.RequestDate))
               .Add(Projections.Property(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.UpdatedAtUtc)), nameof(PSSRequestVM.LastActionDate))
               )).SetResultTransformer(Transformers.AliasToBean<PSSRequestVM>()).Future<PSSRequestVM>();
        }

    }
}