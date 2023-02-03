using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;
using System;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services
{
    public class ExtractDetailsManager : BaseManager<ExtractDetails>, IExtractDetailsManager<ExtractDetails>
    {
        private readonly IRepository<ExtractDetails> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public ExtractDetailsManager(IRepository<ExtractDetails> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Get info of extract request with specified approval number
        /// </summary>
        /// <param name="approvalNumber">approval number</param>
        /// <returns>IEnumerable<ValidatedExtractInfoVM></returns>
        public IEnumerable<ValidatedExtractInfoVM> GetExtractInfoWithApprovalNumber(string approvalNumber)
        {
            var criteria = _transactionManager.GetSession().CreateCriteria<ExtractDetails>("EXTDTS");

            criteria.CreateAlias("EXTDTS.Request", "PSSReq")
                .CreateAlias("PSSReq.TaxEntity", "TaxEntity")
                .CreateAlias("PSSReq.Command", "Cm")
                .CreateAlias("Cm.State", "State")
                .CreateAlias("Cm.LGA", "LGA")
            .Add(Restrictions.Eq("PSSReq.ApprovalNumber", approvalNumber));

            var extCategoryCriteria = DetachedCriteria.For<ExtractCategory>("extCat")
                    .Add(Restrictions.EqProperty("Id", "EXTDTS.SelectedCategory"))
                    .SetProjection(Projections.Constant(1));

            var extSubCategoryCriteria = DetachedCriteria.For<ExtractCategory>("subExtCat")
                    .Add(Restrictions.EqProperty("Id", "EXTDTS.SelectedSubCategory"))
                    .SetProjection(Projections.Constant(1));

            criteria.Add(Subqueries.Exists(extCategoryCriteria));
            //criteria.Add(Subqueries.Exists(extSubCategoryCriteria));


            return criteria.SetProjection(Projections.ProjectionList()
                 .Add(Projections.Property("TaxEntity.Recipient"), nameof(ValidatedExtractInfoVM.CustomerName))
                 .Add(Projections.Property("State.Name"), nameof(ValidatedExtractInfoVM.CommandStateName))
                 .Add(Projections.Property("LGA.Name"), nameof(ValidatedExtractInfoVM.CommandLgaName))
                 .Add(Projections.Property("Cm.Name"), nameof(ValidatedExtractInfoVM.CommandName))
                 .Add(Projections.Property("SelectedCategory"), nameof(ValidatedExtractInfoVM.SelectedCategory))
                 .Add(Projections.Property("SelectedSubCategory"), nameof(ValidatedExtractInfoVM.SelectedSubCategory))
                 .Add(Projections.Property("RequestReason"), nameof(ValidatedExtractInfoVM.Reason))
                 .Add(Projections.Property("PSSReq.CreatedAtUtc"), nameof(ValidatedExtractInfoVM.RequestDate)))
                     .SetResultTransformer(Transformers.AliasToBean<ValidatedExtractInfoVM>())
                     .Future<ValidatedExtractInfoVM>();
        }


        /// <summary>
        /// Get request details
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>ExtractRequestDetailsVM</returns>
        public ExtractRequestDetailsVM GetRequestDetails(long requestId)
        {
            return _transactionManager.GetSession().Query<ExtractDetails>()
            .Where(sr => sr.Request == new  PSSRequest { Id = requestId })
            .Select(extract => new ExtractRequestDetailsVM
            {
                TaxEntity = new TaxEntityViewModel
                {
                    Recipient = extract.Request.TaxEntity.Recipient,
                    PhoneNumber = extract.Request.TaxEntity.PhoneNumber,
                    RCNumber = extract.Request.TaxEntity.RCNumber,
                    Address = extract.Request.TaxEntity.Address,
                    Email = extract.Request.TaxEntity.Email,
                    TaxPayerIdentificationNumber = extract.Request.TaxEntity.TaxPayerIdentificationNumber,
                    SelectedStateName = extract.Request.TaxEntity.StateLGA.State.Name,
                    SelectedLGAName = extract.Request.TaxEntity.StateLGA.Name
                },
                Reason = extract.RequestReason,
                StateName = extract.Request.Command.State.Name,
                LGAName = extract.Request.Command.LGA.Name,
                CommandName = extract.Request.Command.Name,
                CommandAddress = extract.Request.Command.Address,
                RequestId = extract.Request.Id,
                ServiceTypeId = extract.Request.Service.ServiceType,
                ServiceName = extract.Request.Service.Name,
                FileRefNumber = extract.Request.FileRefNumber,
                Status = extract.Request.Status,
                IsIncidentReported = extract.IsIncidentReported ? "YES" : "NO",
                IncidentReportedDate = extract.IncidentReportedDate,
                AffidavitNumber = extract.AffidavitNumber,
                Content = extract.Content,
                DiarySerialNumber = extract.DiarySerialNumber,
                IncidentDateAndTimeParsed = extract.IncidentDateAndTime,
                CrossReferencing = extract.CrossReferencing,
                DefinitionId = extract.Request.FlowDefinitionLevel.Definition.Id,
                Position = extract.Request.FlowDefinitionLevel.Position,
                ApprovalButtonName = extract.Request.FlowDefinitionLevel.ApprovalButtonName,
                AffidavitDateOfIsssuance = extract.AffidavitDateOfIssuance,
                Attachments = extract.ExtractFiles.Select(x => new ExtractRequestAttachmentVM { FileName = x.FileName, FilePath = x.FilePath, ContentType = x.ContentType, Blob = x.Blob }).ToList(),
                ApprovalPartialName = extract.Request.FlowDefinitionLevel.PartialName,
                CbsUser = new CBSUserVM { Name = extract.Request.CBSUser.Name, PhoneNumber = extract.Request.CBSUser.PhoneNumber, Email = extract.Request.CBSUser.Email }
            }).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Get extract request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>ExtractDetailsVM</returns>
        public IEnumerable<ExtractDetailsVM> GetExtractRequestViewDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<ExtractDetails>().Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity.Id == taxEntityId).Select(extract => new ExtractDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = extract.Request.TaxEntity.Recipient,
                        PhoneNumber = extract.Request.TaxEntity.PhoneNumber,
                        RCNumber = extract.Request.TaxEntity.RCNumber,
                        Address = extract.Request.TaxEntity.Address,
                        Email = extract.Request.TaxEntity.Email,
                        TaxPayerIdentificationNumber = extract.Request.TaxEntity.TaxPayerIdentificationNumber,
                        SelectedStateName = extract.Request.TaxEntity.StateLGA.State.Name,
                        SelectedLGAName = extract.Request.TaxEntity.StateLGA.Name
                    },

                    ExtractInfo = new ExtractRequestVM
                    {
                        Reason = extract.RequestReason,
                        CommandStateName = extract.Request.Command.State.Name,
                        CommandLgaName = extract.Request.Command.LGA.Name,
                        CommandName = extract.Request.Command.Name,
                        CommandAddress = extract.Request.Command.Address,
                        ServiceName = extract.Request.Service.Name,
                    },
                    FileRefNumber = extract.Request.FileRefNumber,
                    ApprovalNumber = extract.Request.ApprovalNumber,
                    RequestStatus = extract.Request.Status,
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting extract details for request with file number" + fileRefNumber));
                throw;
            }
        }


        /// <summary>
        /// Get extract document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>ExtractRequestDetailsVM</returns>
        public IEnumerable<ExtractRequestDetailsVM> GetExtractDocumentInfo(long requestId)
        {
            return _transactionManager.GetSession().Query<ExtractDetails>()
            .Where(sr => sr.Request.Id == requestId)
            .Select(extract => new ExtractRequestDetailsVM
            {
                TaxEntity = new TaxEntityViewModel
                {
                    Recipient = extract.Request.TaxEntity.Recipient,
                },
                Reason = extract.RequestReason,
                StateName = extract.Request.Command.State.Name,
                LGAName = extract.Request.Command.LGA.Name,
                CommandName = extract.Request.Command.Name,
                CommandAddress = extract.Request.Command.Address,
                ServiceName = extract.Request.Service.Name,
                RequestDate = extract.Request.CreatedAtUtc,
                ApprovalDate = extract.Request.UpdatedAtUtc.Value,
                ApprovalNumber = extract.Request.ApprovalNumber,
                FileRefNumber = extract.Request.FileRefNumber,
                Content = extract.Content,
                CbsUser = new CBSUserVM { Name = extract.Request.CBSUser.Name }
            }).ToFuture();
        }


        /// <summary>
        /// Updates content, diary serial number, cross referencing, incident date and time for extract details with specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userInput"></param>
        public void UpdateExtractDetailsContentAndDiaryInfo(long requestId, ExtractRequestDetailsVM userInput)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(ExtractDetails).Name;
                string contentName = nameof(ExtractDetails.Content);
                string diarySerialNumberName = nameof(ExtractDetails.DiarySerialNumber);
                string incidentDateAndTimeName = nameof(ExtractDetails.IncidentDateAndTime);
                string crossReferencicngName = nameof(ExtractDetails.CrossReferencing);
                string requestIdName = nameof(ExtractDetails.Request) + "_Id";
                string updatedAtName = nameof(ExtractDetails.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {contentName} = :content, {diarySerialNumberName} = :diarySN, {incidentDateAndTimeName} = :incidentDateAndTime, {crossReferencicngName} = :crossRef, {updatedAtName} = :updateDate WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("content", userInput.Content.Trim());
                query.SetParameter("diarySN", userInput.DiarySerialNumber.Trim());
                query.SetParameter("incidentDateAndTime", userInput.IncidentDateAndTimeParsed);
                query.SetParameter("crossRef", userInput.CrossReferencing);
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating content, diary serial number, cross referencing, incident date and time for extract details with request id {0}, Exception message {1}", requestId, exception.Message));
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Gets extract document details for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public ExtractDocumentVM GetExtractDocumentDetails(string fileRefNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<ExtractDetails>().Where(x => x.Request.FileRefNumber == fileRefNumber).Select(x => new ExtractDocumentVM
                {
                    ExtractDetailsId = x.Id,
                    CommandName = x.Request.Command.Name,
                    CommandStateName = x.Request.Command.State.Name,
                    ApprovalDate = x.Request.UpdatedAtUtc.Value,
                    ApprovalNumber = x.Request.ApprovalNumber,
                    DiarySerialNumber = x.DiarySerialNumber,
                    IncidenDateAndTimeParsed = x.IncidentDateAndTime,
                    CrossRef = x.CrossReferencing,
                    Content = x.Content,
                    DPOName = x.DPOName,
                    DPORankCode = x.DPORankCode
                }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, string.Format("Exception when trying to fetch extract document details for request with file ref number {0}, Exception message {1}", fileRefNumber, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Check if a content detail for an extract has been populated
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>bool</returns>
        public bool CheckExtractContentDetails(string fileRefNumber)
        {
            return _transactionManager.GetSession().Query<ExtractDetails>()
            .Where(x => x.Request.FileRefNumber == fileRefNumber && x.Content != null).Count() > 0;
        }


        /// <summary>
        /// Check if extract diary number and incident date time have been populated
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>bool</returns>
        public bool CheckExtractDiaryIncidentDetails(string fileRefNumber)
        {
            return _transactionManager.GetSession().Query<ExtractDetails>()
            .Where(x => x.Request.FileRefNumber == fileRefNumber && x.DiarySerialNumber != null && x.IncidentDateAndTime != null).Count() > 0;
        }


        /// <summary>
        /// Get a content detail for an extract using the FileRefNumber
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns>string</returns>
        public string GetExtractContentDetails(string fileRefNumber)
        {
            return _transactionManager.GetSession().Query<ExtractDetails>()
            .Where(x => x.Request.FileRefNumber == fileRefNumber && x.Content != null).Select(x=> x.Content).ToList().SingleOrDefault();
        }


        /// <summary>
        /// Updates content for extract details with specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userInput"></param>
        public void UpdateExtractDetailsContent(long requestId, string content)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(ExtractDetails).Name;
                string contentName = nameof(ExtractDetails.Content);
                string requestIdName = nameof(ExtractDetails.Request) + "_Id";
                string updatedAtName = nameof(ExtractDetails.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {contentName} = :content, {updatedAtName} = :updateDate WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("content", content.Trim());
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception updating content for extract details with request id {0}, Exception message {1}", requestId, exception.Message));
                RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Check if <paramref name="affivdavitNumber"/> does not exist with the user 
        /// with the <paramref name="taxEntityId"/>
        /// </summary>
        /// <param name="affivdavitNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public bool CheckIfExistingAffidavitNumber(string affivdavitNumber, long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<ExtractDetails>().Count(x => x.AffidavitNumber == affivdavitNumber && x.Request.TaxEntity != new CBS.Core.Models.TaxEntity { Id = taxEntityId } ) > 0;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error checking if affidavit exist" + affivdavitNumber));
                throw;
            }
        }


        /// <summary>
        /// Updates extract details DPO Name and Service Number
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceNumber"></param>
        /// <param name="name"></param>
        /// <param name="dpoRankCode"></param>
        /// <param name="adminId"></param>
        public void UpdateExtractDPONameAndServiceNumber(long requestId, string serviceNumber, string name, string dpoRankCode, int adminId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(ExtractDetails).Name;
                string nameLabel = nameof(ExtractDetails.DPOName);
                string rankCodeLabel = nameof(ExtractDetails.DPORankCode);
                string serviceNumberLabel = nameof(ExtractDetails.DPOServiceNumber);
                string requestIdName = nameof(ExtractDetails.Request) + "_Id";
                string addedByName = nameof(ExtractDetails.DPOAddedBy) + "_Id";
                string updatedAtName = nameof(ExtractDetails.UpdatedAtUtc);

                var queryText = $"UPDATE {tableName} SET {nameLabel} = :dpoName, {serviceNumberLabel} = :dpoServiceNumber, {rankCodeLabel} = :dpoRankCode, {addedByName} = :adminId, {updatedAtName} = :updateDate WHERE {requestIdName} = :requestId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("dpoName", name);
                query.SetParameter("dpoRankCode", dpoRankCode);
                query.SetParameter("dpoServiceNumber", serviceNumber);
                query.SetParameter("adminId", adminId);
                query.SetParameter("requestId", requestId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating DPO name, rank code and service number for extract details with request id {0}, Exception message {1}", requestId, exception.Message));
                throw;
            }
        }


    }
}