using Orchard;
using Orchard.Security;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.SMS.Provider.Contracts;
using Orchard.Logging;

namespace Parkway.CBS.ETCC.Admin.Controllers.Handlers
{
    public class FirstApproverHandler : IApprovalComposition
    {
        public TCCApprovalLevel GetApprovalLevelDefinition => TCCApprovalLevel.FirstLevelApprover;
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest> _tccRequestManager;
        private readonly ITaxClearanceCertificateRequestApprovalLogManager<TaxClearanceCertificateRequestApprovalLog> _tccRequestApprovalLogManager;
        private readonly IEnumerable<ISMSProvider> _smsProvider;
        public ILogger Logger { get; set; }

        public FirstApproverHandler(IOrchardServices orchardServices, ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest> tccRequestManager, IEnumerable<ISMSProvider> smsProvider, ITaxClearanceCertificateRequestApprovalLogManager<TaxClearanceCertificateRequestApprovalLog> tccRequestApprovalLogManager)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _tccRequestManager = tccRequestManager;
            _smsProvider = smsProvider;
            _tccRequestApprovalLogManager = tccRequestApprovalLogManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Save request approval details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        public bool ProcessRequestApproval(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors)
        {
            try
            {
                if (string.IsNullOrEmpty(requestDetailVM.Comment))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Comment field is required", FieldName = "Comment" });
                    throw new DirtyFormDataException("Comment field is empty for apporval");
                }

                if (requestDetailVM.Comment.Length < 10)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Comment requires atleast 10 characters", FieldName = "Comment" });
                    throw new DirtyFormDataException("Comment requires atleast 10 characters");
                }


                TaxClearanceCertificateRequest requestDetails = _tccRequestManager.Get(x => x.Id == requestDetailVM.Id);
                if (requestDetails == null)
                { throw new NoRecordFoundException("404 for TCC application request. Request Id " + requestDetailVM.Id); }

                requestDetails.ApprovalStatusLevelId = (int)TCCApprovalLevel.SecondLevelApprover;
                requestDetails.UpdatedAtUtc = DateTime.Now;
                TaxClearanceCertificateRequestApprovalLog approvalLog = new TaxClearanceCertificateRequestApprovalLog
                {
                    AddedByAdminUser = new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                    Comment = requestDetailVM.Comment,
                    Request = new TaxClearanceCertificateRequest { Id = requestDetails.Id },
                    Status = (int)TCCRequestStatus.Approved,
                    ApprovalLevelId = (int)TCCApprovalLevel.FirstLevelApprover,
                };

                SaveRequestApprovalLog(approvalLog);
                try
                {
                    if (CheckSendSMSNotification(requestDetails.TaxEntity.PhoneNumber))
                    {
                        int providerId = 0;
                        bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                        if (!result)
                        {
                            providerId = (int)SMSProvider.Pulse;
                        }
                        foreach (var impl in _smsProvider)
                        {
                            if ((SMSProvider)providerId == impl.GetSMSNotificationProvider)
                            {
                                //string message = $"Dear {requestDetails.TaxEntity.Recipient}, kindly be informed that your TCC request has been approved.";
                                //impl.SendSMS(requestDetails.TaxEntity.PhoneNumber, message);
                                break;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, exception.Message);
                }

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _tccRequestManager.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Save request rejection details
        /// </summary>
        /// <param name="requestDetailVM"></param>
        /// <param name="errors"></param>
        /// <returns>bool</returns>
        public bool ProcessRequestRejection(TCCRequestDetailVM requestDetailVM, ref List<ErrorModel> errors)
        {
            try
            {
                if (string.IsNullOrEmpty(requestDetailVM.Comment))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Comment field is required", FieldName = "Comment" });
                    throw new DirtyFormDataException("Comment field is empty for apporval");
                }

                if (requestDetailVM.Comment.Length < 10)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Comment requires atleast 10 characters", FieldName = "Comment" });
                    throw new DirtyFormDataException("Comment requires atleast 10 characters");
                }

                TaxClearanceCertificateRequest requestDetails = _tccRequestManager.Get(x => x.Id == requestDetailVM.Id);
                if (requestDetails == null)
                { throw new NoRecordFoundException("404 for TCC application request. Request Id " + requestDetailVM.Id); }

                requestDetails.Status = (int)TCCRequestStatus.Rejected;
                requestDetails.UpdatedAtUtc = DateTime.Now;

                TaxClearanceCertificateRequestApprovalLog approvalLog = new TaxClearanceCertificateRequestApprovalLog
                {
                    AddedByAdminUser = new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id },
                    Comment = requestDetailVM.Comment,
                    Request = new TaxClearanceCertificateRequest { Id = requestDetails.Id },
                    Status = (int)TCCRequestStatus.Rejected,
                    ApprovalLevelId = (int)TCCApprovalLevel.FirstLevelApprover,
                };

                SaveRequestApprovalLog(approvalLog);
                try
                {
                    if (CheckSendSMSNotification(requestDetails.TaxEntity.PhoneNumber))
                    {
                        int providerId = 0;
                        bool result = Int32.TryParse(AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.SMSProvider), out providerId);
                        if (!result)
                        {
                            providerId = (int)SMSProvider.Pulse;
                        }
                        foreach (var impl in _smsProvider)
                        {
                            if ((SMSProvider)providerId == impl.GetSMSNotificationProvider)
                            {
                                //string message = $"Dear {requestDetails.TaxEntity.Recipient}, your TCC request was rejected. Reason: {requestDetailVM.Comment}.";
                                //impl.SendSMS(requestDetails.TaxEntity.PhoneNumber, message);
                                break;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, exception.Message);
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _tccRequestManager.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// Save request approval log
        /// </summary>
        /// <param name="approvalLog"></param>
        /// <exception cref="CouldNotSaveRecord">If insert fails</exception>
        public void SaveRequestApprovalLog(TaxClearanceCertificateRequestApprovalLog approvalLog)
        {
            if (!_tccRequestApprovalLogManager.Save(approvalLog))
            {
                throw new CouldNotSaveRecord();
            }
        }

        /// <summary>
        /// Check if we can send sms notification for a specified tenant
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        private bool CheckSendSMSNotification(string phoneNumber)
        {
            bool canSendNotification = false;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.IsSMSEnabled.ToString()).FirstOrDefault();
            if (node != null && !string.IsNullOrEmpty(node.Value))
            {
                bool isSMSEnabled = false;
                bool.TryParse(node.Value, out isSMSEnabled);
                if (isSMSEnabled && !string.IsNullOrEmpty(phoneNumber))
                {
                    canSendNotification = true;
                }
            }

            return canSendNotification;
        }
    }
}