using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models;
using NHibernate.Transform;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRequestManager : BaseManager<PSSRequest>, IPSSRequestManager<PSSRequest>
    {
        private readonly IRepository<PSSRequest> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSRequestManager(IRepository<PSSRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get request Id by file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>long</returns>
        public long GetRequestId(string fileNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>().Where(req => req.FileRefNumber == fileNumber).Select(req => req.Id).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get service type Id by file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>long</returns>
        public int GetServiceType(string fileNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>().Where(req => req.FileRefNumber == fileNumber).Select(req => req.Service.ServiceType).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get request details by request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>GenericRequestDetails</returns>
        public GenericRequestDetails GetRequestDetails(Int64 requestId)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
           .Where(sr => sr == new PSSRequest { Id = requestId })
           .Select(req => new GenericRequestDetails
           {
               TaxEntity = new TaxEntityViewModel
               {
                   Recipient = req.TaxEntity.Recipient,
                   PhoneNumber = req.TaxEntity.PhoneNumber,
                   RCNumber = req.TaxEntity.RCNumber,
                   Address = req.TaxEntity.Address,
                   Email = req.TaxEntity.Email,
                   TaxPayerIdentificationNumber = req.TaxEntity.TaxPayerIdentificationNumber,
                   SelectedStateName = req.TaxEntity.StateLGA.State.Name,
                   SelectedLGAName = req.TaxEntity.StateLGA.Name
               },
               Reason = req.Reason,
               StateName = req.Command.State.Name,
               LGAName = req.Command.LGA.Name,
               CommandName = req.Command.Name,
               CommandAddress = req.Command.Address,
               RequestId = req.Id,
               ServiceTypeId = req.Service.ServiceType,
               ServiceRequests = req.ServiceRequests.Select(ser => new PSSServiceRequestDTO
               {
                   InvoiceAmount = ser.Invoice.Amount,
                   InvoiceNumber = ser.Invoice.InvoiceNumber,
                   Status = ser.Status,
                   ServiceName = ser.Service.Name,
                   InvoiceAmountDue = ser.Invoice.InvoiceAmountDueSummary.AmountDue,
               }),
               FlowDefinitionId = req.FlowDefinitionLevel.Definition.Id,
               FlowDefinitionLevelId = req.FlowDefinitionLevel.Id
           }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get the file ref number for this request
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string | null</returns>
        public string GetFileRefNumber(long id)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
          .Where(x => x.Id == id).Select(x => x.FileRefNumber).SingleOrDefault();
        }


        /// <summary>
        /// Get the service type of this request Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>int</returns>
        public int GetServiceType(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
         .Where(x => x.Id == requestId).Select(x => x.Service.ServiceType).SingleOrDefault();
        }


        /// <summary>
        /// Update the request with the given status
        /// <para>Returns true if successfully saved</para>
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="status"></param>
        /// <returns>bool</returns>
        public bool SetRequestStatus(long requestId, PSSRequestStatus status)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSRequest).Name;
                string statusName = nameof(PSSRequest.Status);
                string updatedAtName = nameof(PSSRequest.UpdatedAtUtc);
                string requestIdName = nameof(PSSRequest.Id);

                var queryText = $"UPDATE psr SET psr.{statusName} = :statusVal, psr.{updatedAtName} = :updateDate FROM {tableName} psr WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("statusVal", (int)status);
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating request flow id {0}, Exception message {1}", requestId, exception.Message));
                throw;
            }
        }



        /// <summary>
        /// Update the request with the new flow definition level and status
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="newDefinitionLevelId"></param>
        public void UpdateRequestFlowId(long requestId, int newDefinitionLevelId, PSSRequestStatus status)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSRequest).Name;
                string statusName = nameof(PSSRequest.Status);
                string updatedAtName = nameof(PSSRequest.UpdatedAtUtc);
                string requestIdName = nameof(PSSRequest.Id);
                string flowDefIdName = nameof(PSSRequest.FlowDefinitionLevel) + "_Id";

                var queryText = $"UPDATE psr SET psr.{statusName} = :statusVal, psr.{updatedAtName} = :updateDate, psr.{flowDefIdName} = :flowDefId FROM {tableName} psr WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("statusVal", (int)status);
                query.SetParameter("requestId", requestId);
                query.SetParameter("flowDefId", newDefinitionLevelId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating request flow id {0}, definition level Id {1}, Exception message {2}", requestId, newDefinitionLevelId, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Get invoices for request with specified File ref number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>List<RequestInvoiceVM></returns>
        public ICollection<RequestInvoiceVM> GetRequestInvoices(string fileNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
            .Where(sr => sr.FileRefNumber == fileNumber)
            .Select(requestInvoice => requestInvoice.Invoices.Select(invoices => new RequestInvoiceVM
            {
                Id = invoices.Id,
                InvoiceNumber = invoices.Invoice.InvoiceNumber,
                InvoiceUrl = invoices.Invoice.InvoiceURL,
                ServiceName = invoices.Request.Service.Name,
                FileRefNumber = invoices.Request.FileRefNumber,
                Status = invoices.Invoice.Status,
                Amount = invoices.Invoice.Amount,
                AmountDue = invoices.Invoice.InvoiceAmountDueSummary.AmountDue
            }).ToList()).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get invoices for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>List<RequestInvoiceVM></returns>
        public ICollection<RequestInvoiceVM> GetRequestInvoices(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
            .Where(sr => sr == new PSSRequest { Id = requestId })
            .Select(requestInvoice => requestInvoice.Invoices.Select(invoices => new RequestInvoiceVM
            {
                Id = invoices.Id,
                InvoiceNumber = invoices.Invoice.InvoiceNumber,
                InvoiceUrl = invoices.Invoice.InvoiceURL,
                ServiceName = invoices.Request.Service.Name,
                FileRefNumber = invoices.Request.FileRefNumber,
                Status = invoices.Invoice.Status,
                Amount = invoices.Invoice.Amount,
                AmountDue = invoices.Invoice.InvoiceAmountDueSummary.AmountDue
            }).ToList()).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Get request details by file number
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns>GenericRequestDetails</returns>
        public PSSRequestVM GetRequestDetails(string fileNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
           .Where(sr => sr.FileRefNumber == fileNumber)
           .Select(req => new PSSRequestVM
           {
               Id = req.Id,
               StatusId = req.Status,
               FileRefNumber = req.FileRefNumber,
               CommandName = req.Command.Name,
               ServiceTypeId = req.Service.ServiceType,
               TaxEntityId = req.TaxEntity.Id,
               FlowDefinitionLevelId = req.FlowDefinitionLevel.Id,
               ApprovalNumber = req.ApprovalNumber,
               CustomerName = req.TaxEntity.Recipient,
               ServiceName = req.Service.Name
           }).SingleOrDefault();
        }


        /// <summary>
        /// Gets the service id, service name, customer name and file ref number of request with specified id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public PSSRequestVM GetPSSRequestServiceDetails(long requestId)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
                    .Where(req => req.Id == requestId)
                    .Select(req => new PSSRequestVM
                    {
                        Id = req.Id,
                        ServiceId = req.Service.Id,
                        ServiceName = req.Service.Name,
                        CustomerName = req.TaxEntity.Recipient,
                        FileRefNumber = req.FileRefNumber,
                        FlowDefinitionLevelId = req.FlowDefinitionLevel.Id
                    }).SingleOrDefault();
        }


        /// <summary>
        /// Get all the form details for this request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable{FormControlRevenueHeadValueVM}</returns>
        public IEnumerable<FormControlRevenueHeadValueVM> GetFormDetails(long requestId)
        {
            try
            {
                var criteria = _transactionManager.GetSession().CreateCriteria<FormControlRevenueHeadValue>(nameof(FormControlRevenueHeadValue))
                    .CreateAlias(nameof(FormControlRevenueHeadValue.FormControlRevenueHead), "FormControlRevenueHead")
                    .CreateAlias(nameof(FormControlRevenueHeadValue.FormControlRevenueHead) + "." + nameof(FormControlRevenueHeadValue.FormControlRevenueHead.Form), "Form")
                    .SetProjection(Projections.ProjectionList()
                    .Add(Projections.Property(nameof(FormControlRevenueHeadValue.Value)), nameof(FormControlRevenueHeadValue.Value))
                    .Add(Projections.Property("Form.Name"), nameof(FormControlRevenueHeadValueVM.Name)));

                var requestInvoiceSubQuery = DetachedCriteria.For<PSSRequestInvoice>("fr")
                   .Add(Restrictions.EqProperty("FormControlRevenueHeadValue.Invoice.Id", "Invoice.Id"))
                   .Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.Id == requestId))
                   .SetProjection(Projections.Property("Invoice.Id"));

                return criteria.Add(Subqueries.Exists(requestInvoiceSubQuery))
                    .SetResultTransformer(Transformers.AliasToBean<FormControlRevenueHeadValueVM>())
                .Future<FormControlRevenueHeadValueVM>();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Set approval number for request with specified Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="approvalNumber"></param>
        public void SetApprovalNumber(long requestId, string approvalNumber)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSRequest).Name;
                string approvalNumberName = nameof(PSSRequest.ApprovalNumber);
                string updatedAtName = nameof(PSSRequest.UpdatedAtUtc);
                string requestIdName = nameof(PSSRequest.Id);

                var queryText = $"UPDATE psr SET psr.{approvalNumberName} = :approvalNumber, psr.{updatedAtName} = :updateDate FROM {tableName} psr WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("approvalNumber", approvalNumber);
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception setting approval number for request with Id {0}, Exception message {1}", requestId, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns></returns>
        public GenericRequestDetails GetRequestDetailsByFileNumber(string fileNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>()
           .Where(sr => sr.FileRefNumber == fileNumber)
           .Select(req => new GenericRequestDetails
           {
               TaxEntity = new TaxEntityViewModel
               {
                   Recipient = req.TaxEntity.Recipient,
                   PhoneNumber = req.TaxEntity.PhoneNumber,
                   RCNumber = req.TaxEntity.RCNumber,
                   Address = req.TaxEntity.Address,
                   Email = req.TaxEntity.Email,
                   TaxPayerIdentificationNumber = req.TaxEntity.TaxPayerIdentificationNumber,
                   SelectedStateName = req.TaxEntity.StateLGA.State.Name,
                   SelectedLGAName = req.TaxEntity.StateLGA.Name
               },
               Reason = req.Reason,
               StateName = req.Command.State.Name,
               LGAName = req.Command.LGA.Name,
               CommandName = req.Command.Name,
               CommandAddress = req.Command.Address,
               RequestId = req.Id,
               ServiceTypeId = req.Service.ServiceType,
               ServiceRequests = req.ServiceRequests.Select(ser => new PSSServiceRequestDTO
               {
                   InvoiceAmount = ser.Invoice.Amount,
                   InvoiceNumber = ser.Invoice.InvoiceNumber,
                   Status = ser.Status,
                   ServiceName = ser.Service.Name,
                   InvoiceAmountDue = ser.Invoice.InvoiceAmountDueSummary.AmountDue,
                   FlowDefinitionLevelId = ser.FlowDefinitionLevel.Id
               }),
               FlowDefinitionId = req.FlowDefinitionLevel.Definition.Id,
               FlowDefinitionLevelId = req.FlowDefinitionLevel.Id
           }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get info of request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <param name="taxEntityId"></param>
        /// <returns>ValidatedDocumentVM</returns>
        public ValidatedDocumentVM GetRequestInfoWithApprovalNumber(string approvalNumber, long taxEntityId)
        {
            return _transactionManager.GetSession().Query<PSSRequest>().Where(req => req.ApprovalNumber == approvalNumber && req.TaxEntity == new TaxEntity { Id = taxEntityId }).Select(req => new ValidatedDocumentVM
            {
                RequestId = req.Id,
                ServiceType = req.Service.ServiceType,
            }).SingleOrDefault();
        }

        /// <summary>
        /// Get info of request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>ValidatedDocumentVM</returns>
        public ValidatedDocumentVM GetRequestInfoWithApprovalNumber(string approvalNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>().Where(req => req.ApprovalNumber == approvalNumber).Select(req => new ValidatedDocumentVM
            {
                RequestId = req.Id,
                ServiceType = req.Service.ServiceType,
            }).SingleOrDefault();
        }


        /// <summary>
        /// Get info of request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>USSDValidateDocumentVM</returns>
        public USSDValidateDocumentVM GetRequestDetailsByApprovalNumber(string approvalNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequest>().Where(req => req.ApprovalNumber == approvalNumber).Select(req => new USSDValidateDocumentVM
            {
                DocumentNumber = req.ApprovalNumber,
                ApplicantName = req.TaxEntity.Recipient,
                ServiceName = req.Service.Name,
                ApprovalDate = req.UpdatedAtUtc.Value.ToString("dd MMMM yyyy HH:mm")
            }).SingleOrDefault();
        }


        /// <summary>
        /// Get pending PSS request statistics count for logon admin user
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<ReportStatsVM> GetAdminRequestStatistics(PSSRequestStatus status, int adminUserId, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions, bool applyDateFilter = false)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<PSSRequest>("PR")
                .Add(Restrictions.Where<PSSRequest>(x => x.Status == (int)status));

            if (applyDateFilter)
            {
                DateTime today = DateTime.Now.ToLocalTime();
                DateTime startDate = new DateTime(today.Year, today.Month, 1);
                DateTime endDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddDays(1).AddSeconds(-1);
                criteria.Add(Restrictions.Between(nameof(PSSRequest.CreatedAtUtc), startDate, endDate));
            }

            if (applyApprovalAccessRestrictions)
            {
                var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>("PRF")
                .Add(Restrictions.Eq("AssignedApprover.Id", adminUserId))
                .Add(Restrictions.EqProperty("FlowDefinitionLevel.Id", "PR.FlowDefinitionLevel.Id"))
                .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
            }

            if (applyAccessRestrictions)
            {
                ///Query equivalent without joining PSServiceRequestFlowApprover table///
                //////////////////////////////
                /*SELECT * FROM [PSSCentralBillingSystem].[dbo].[Parkway_CBS_Police_Core_PSSRequest] pr 
			    INNER JOIN [PSSCentralBillingSystem].[dbo].[Parkway_CBS_Police_Core_Command] cm ON cm.Id = pr.Command_Id
			    INNER JOIN [PSSCentralBillingSystem].[dbo].[Parkway_CBS_Police_Core_ApprovalAccessList] pa ON 
			    (cm.CommandCategory_Id = pa.CommandCategory_Id AND pa.State_Id IS NULL AND pa.LGA_Id IS NULL AND pa.Command_Id IS NULL AND pa.Service_Id IS NULL) OR (cm.CommandCategory_Id = pa.CommandCategory_Id AND cm.State_Id = pa.State_Id AND pa.LGA_Id IS NULL AND pa.Command_Id IS NULL AND pa.Service_Id IS NULL) OR (cm.CommandCategory_Id = pa.CommandCategory_Id AND cm.State_Id = pa.State_Id AND cm.LGA_Id = pa.LGA_Id AND pa.Command_Id IS NULL AND pa.Service_Id IS NULL) OR (cm.CommandCategory_Id = pa.CommandCategory_Id AND cm.State_Id = pa.State_Id AND cm.LGA_Id = pa.LGA_Id AND cm.Id = pa.Command_Id AND pa.Service_Id IS NULL) OR  (cm.CommandCategory_Id = pa.CommandCategory_Id AND cm.State_Id = pa.State_Id AND cm.LGA_Id = pa.LGA_Id AND pr.Id = pa.Service_Id AND pa.Command_Id IS NULL) OR (cm.CommandCategory_Id = pa.CommandCategory_Id AND cm.State_Id = pa.State_Id AND cm.LGA_Id = pa.LGA_Id AND cm.Id = pa.Command_Id AND pr.Id = pa.Service_Id)
			    INNER JOIN [PSSCentralBillingSystem].[dbo].[Parkway_CBS_Police_Core_ApprovalAccessRoleUser] au ON pa.ApprovalAccessRoleUser_Id = au.Id WHERE au.User_Id = 2*/

                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "PR.Command.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Service.Id", "PR.Service.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.EqProperty("Service.Id", "PR.Service.Id"))))
                .SetProjection(Projections.Constant(1));

                var accessRoleUserCriteria = DetachedCriteria.For<ApprovalAccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", adminUserId))
                    .Add(Restrictions.EqProperty("Id", "aal.ApprovalAccessRoleUser.Id"))
                    .SetProjection(Projections.Constant(1));


                accessListCriteria.Add(Subqueries.Exists(accessRoleUserCriteria));
                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
                criteria.Add(Subqueries.Exists(commandCriteria));
            }

            return criteria
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Count<PSSRequest>(x => x.Id), "TotalRecordCount")
                    ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }
    }
}