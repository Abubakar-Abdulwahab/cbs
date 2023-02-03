using InfoGRID.Pulse.SDK.DTO;
using InfoGRID.Pulse.SDK.Models;
using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Notification;
using Parkway.CBS.HangFireInterface.Notification.Contracts;
using Parkway.CBS.Police.Core.Mail.Provider.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Utilities;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Mail.Provider
{
    public class PSSPulseProvider : IPSSEmailProvider
    {
        private readonly IEmailNotification _hangfireNotification;
        private readonly IOrchardServices _orchardServices;
        public EmailProvider GetEmailNotificationProvider => EmailProvider.Pulse;
        public ILogger Logger { get; set; }

        public PSSPulseProvider(IOrchardServices orchardServices)
        {
            _hangfireNotification = new EmailNotification();
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Sends an email with details filled by user on the contact us page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ContactUsNotification(dynamic model)
        {
            bool isEmailSent = false;

            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
            string recipientEmail = AppSettingsConfigurations.GetSettingsValue(PSSTenantConfigKeys.ContactUsEmailAddress.ToString());
            model.BaseUrl = PSSUtil.GetBaseUrl(siteName);

            try
            {

                List<RecipientInfo> recipients = new List<RecipientInfo>
                {
                    new RecipientInfo
                    {
                        Value = recipientEmail,
                        Type = RecipientType.Email,
                        Channel = NotificationChannel.Unknown
                    }
                };

                string sDataParams = JsonConvert.SerializeObject(model);
                string subject = (string)model.Subject;

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", subject },
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };

                Pulse pulse = new Pulse
                {
                    Data = new PulseData
                    {
                        Recipients = recipients,
                        Params = emailDetails
                    },
                    Name = PSSPulseTemplateFileNames.ContactUsNotification.ToDescription()
                };

                PulseHeader pulseHeader = new PulseHeader
                {
                    Type = RecipientType.Email.ToString()
                };

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);

                isEmailSent = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            return isEmailSent;
        }

        /// <summary>
        /// Sends an email to admin user after registration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool PSSAdminUserRegistrationNotification(dynamic model)
        {
            try
            {
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), Name = (string)model.Name, Password = (string)model.Password, AdminBaseUrl = PSSUtil.GetAdminBaseUrl(siteName), Username = model.Username };
                string sDataParams = JsonConvert.SerializeObject(dataParams);
                string subject = (string)model.Subject;

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", subject },
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };

                PulseData pulseData = new PulseData
                {
                    Recipients = new List<RecipientInfo>
                    {
                         new RecipientInfo
                        {
                            Value = (string)model.Email,
                            Type = RecipientType.Email,
                            Channel = NotificationChannel.Unknown
                        }
                    },
                    Params = emailDetails
                };

                Pulse pulse = new Pulse
                {
                    Data = pulseData,
                    Name = PSSPulseTemplateFileNames.AdminUserCreationNotification.ToDescription()
                };

                PulseHeader pulseHeader = new PulseHeader
                {
                    Type = RecipientType.Email.ToString()
                };

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Sends an email containing random generated password to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool PSSSubUserPasswordNotification(dynamic model)
        {
            try
            {
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), Name = (string)model.Recipient, Password = (string)model.Password };
                string sDataParams = JsonConvert.SerializeObject(dataParams);
                string subject = (string)model.Subject;

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", subject },
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };

                PulseData pulseData = new PulseData
                {
                    Recipients = new List<RecipientInfo>
                    {
                         new RecipientInfo
                        {
                            Value = (string)model.Email,
                            Type = RecipientType.Email,
                            Channel = NotificationChannel.Unknown
                        }
                    },
                    Params = emailDetails
                };

                Pulse pulse = new Pulse
                {
                    Data = pulseData,
                    Name = PSSPulseTemplateFileNames.SubUserPasswordNotification.ToDescription()
                };

                PulseHeader pulseHeader = new PulseHeader
                {
                    Type = RecipientType.Email.ToString()
                };

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool PSSRequestApproval(dynamic model)
        {
            try
            {
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), Name = (string)model.Recipient, ApprovalNumber = (string)model.ApprovalNumber, InvoiceNumber = (string)model.InvoiceNumber, RequestType = (string)model.RequestType, RequestDate = (string)model.RequestDate };
                string sDataParams = JsonConvert.SerializeObject(dataParams);
                string subject = (string)model.Subject;

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", subject },
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };

                PulseData pulseData = new PulseData
                {
                    Recipients = new List<RecipientInfo>
                    {
                         new RecipientInfo
                        {
                            Value = (string)model.Email,
                            Type = RecipientType.Email,
                            Channel = NotificationChannel.Unknown
                        }
                    },
                    Params = emailDetails
                };

                Pulse pulse = new Pulse
                {
                    Data = pulseData
                };

                SetApprovalTemplateFileName((PSSServiceTypeDefinition)model.ServiceTypeId, pulse);

                PulseHeader pulseHeader = new PulseHeader
                {
                    Type = RecipientType.Email.ToString()
                };

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        public bool PSSRequestRejection(dynamic model)
        {
            try
            {
                Dictionary<string, string> emailDetails = new Dictionary<string, string>();

                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                var dataParams = new { Name = (string)model.Recipient, Comment = (string)model.Comment, RequestDate = (string)model.RequestDate, RequestType = (string)model.RequestType, BaseUrl = PSSUtil.GetBaseUrl(siteName) };
                string sDataParams = JsonConvert.SerializeObject(dataParams);
                string subject = (string)model.Subject;
                emailDetails.Add("Subject", subject);
                emailDetails.Add("Sender", PSSPulseTemplateFileNames.Sender.ToDescription());
                emailDetails.Add("Params", sDataParams);

                PulseData pulseData = new PulseData
                {
                    Recipients = new List<RecipientInfo>
                    {
                        new RecipientInfo
                        {
                            Value = (string)model.Email,
                            Type = RecipientType.Email,
                            Channel = NotificationChannel.Unknown
                        }
                    },
                    Params = emailDetails
                };

                Pulse pulse = new Pulse
                {
                    Data = pulseData
                };

                SetRejectionTemplateFileName((PSSServiceTypeDefinition)model.ServiceTypeId, pulse);

                PulseHeader pulseHeader = new PulseHeader
                {
                    Type = RecipientType.Email.ToString()
                };

                _hangfireNotification.SendNotificationPulse(pulse, pulseHeader);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        private void SetRejectionTemplateFileName(PSSServiceTypeDefinition serviceType, Pulse pulse)
        {
            switch (serviceType)
            {
                case PSSServiceTypeDefinition.Extract:
                    pulse.Name = EmailTemplateFileNames.ExtractRejectionNotification.ToDescription();
                    break;
                case PSSServiceTypeDefinition.Escort:
                    pulse.Name = EmailTemplateFileNames.EscortRejectionNotification.ToDescription();
                    break;
                case PSSServiceTypeDefinition.GenericPoliceServices:
                    pulse.Name = EmailTemplateFileNames.GenericRejectionNotification.ToDescription();
                    break;
                case PSSServiceTypeDefinition.CharacterCertificate:
                    pulse.Name = EmailTemplateFileNames.CharacterCertificateRejectionNotification.ToDescription();
                    break;
            }
        }

        private void SetApprovalTemplateFileName(PSSServiceTypeDefinition serviceType, Pulse pulse)
        {
            switch (serviceType)
            {
                case PSSServiceTypeDefinition.Extract:
                    pulse.Name = EmailTemplateFileNames.ExtractApprovalNotification.ToDescription();
                    break;
                case PSSServiceTypeDefinition.Escort:
                    pulse.Name = EmailTemplateFileNames.EscortApprovalNotification.ToDescription();
                    break;
                case PSSServiceTypeDefinition.GenericPoliceServices:
                    pulse.Name = EmailTemplateFileNames.GenericApprovalNotification.ToDescription();
                    break;
                case PSSServiceTypeDefinition.CharacterCertificate:
                    pulse.Name = EmailTemplateFileNames.CharacterCertificateApprovalNotification.ToDescription();
                    break;
            }
        }

    }
}