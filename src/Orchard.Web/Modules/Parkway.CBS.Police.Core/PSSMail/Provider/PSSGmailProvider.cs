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

namespace ParkwayCBS.Police.Core.Mail.Provider
{
    public class PSSGmailProvider : IPSSEmailProvider
    {
        public EmailProvider GetEmailNotificationProvider => EmailProvider.Gmail;
        private readonly IEmailNotification _hangfireNotification;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public PSSGmailProvider(IOrchardServices orchardServices)
        {
            _hangfireNotification = new EmailNotification();
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        public bool PSSRequestApproval(dynamic model)
        {
            try
            {
                Dictionary<string, string> emailDetails = new Dictionary<string, string>();
                Dictionary<string, string> data = new Dictionary<string, string>();
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), ApprovalNumber = (string)model.ApprovalNumber, Name = (string)model.Recipient, RequestType = (string)model.RequestType, InvoiceNumber = (string)model.InvoiceNumber, RequestDate = (string)model.RequestDate };
                data.Add("Name", dataParams.Name);
                data.Add("ApprovalNumber", dataParams.ApprovalNumber);
                data.Add("RequestType", dataParams.RequestType);
                data.Add("InvoiceNumber", dataParams.InvoiceNumber);
                data.Add("RequestDate", dataParams.RequestDate);
                data.Add("BaseUrl", dataParams.BaseUrl);

                string sDataParams = JsonConvert.SerializeObject(dataParams);
                string subject = model.Subject;
                emailDetails.Add("Subject", subject);
                emailDetails.Add("Sender", EmailTemplateFileNames.Sender.ToDescription());
                emailDetails.Add("Params", sDataParams);

                switch ((PSSServiceTypeDefinition)model.ServiceTypeId)
                {
                    case PSSServiceTypeDefinition.Extract:
                        model.TemplateName = EmailTemplateFileNames.ExtractApprovalNotification.ToDescription();
                        break;
                    case PSSServiceTypeDefinition.Escort:
                        model.TemplateName = EmailTemplateFileNames.EscortApprovalNotification.ToDescription();
                        break;
                    case PSSServiceTypeDefinition.GenericPoliceServices:
                        model.TemplateName = EmailTemplateFileNames.GenericApprovalNotification.ToDescription();
                        break;
                    case PSSServiceTypeDefinition.CharacterCertificate:
                        model.TemplateName = EmailTemplateFileNames.CharacterCertificateRejectionNotification.ToDescription();
                        break;
                }

                string sModel = JsonConvert.SerializeObject(new { Recipient = (string)model.Email, Subject = subject });
                _hangfireNotification.SendNotificationGmail(sModel, data, $"{model.TemplateName}.html");

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
                Dictionary<string, string> data = new Dictionary<string, string>();
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), Comment = (string)model.Comment, Name = (string)model.Recipient, RequestDate = (string)model.RequestDate, RequestType = (string)model.RequestType };
                data.Add("Name", dataParams.Name);
                data.Add("Comment", dataParams.Comment);
                data.Add("RequestDate", dataParams.RequestDate);
                data.Add("RequestType", dataParams.RequestType);
                data.Add("BaseUrl", dataParams.BaseUrl);

                string sDataParams = JsonConvert.SerializeObject(dataParams);
                string subject = model.Subject;
                emailDetails.Add("Subject", subject);
                emailDetails.Add("Sender", EmailTemplateFileNames.Sender.ToDescription());
                emailDetails.Add("Params", sDataParams);


                switch ((PSSServiceTypeDefinition)model.ServiceTypeId)
                {
                    case PSSServiceTypeDefinition.Extract:
                        model.TemplateName = EmailTemplateFileNames.ExtractRejectionNotification.ToDescription();
                        break;
                    case PSSServiceTypeDefinition.Escort:
                        model.TemplateName = EmailTemplateFileNames.EscortRejectionNotification.ToDescription();
                        break;
                    case PSSServiceTypeDefinition.GenericPoliceServices:
                        model.TemplateName = EmailTemplateFileNames.GenericRejectionNotification.ToDescription();
                        break;
                    case PSSServiceTypeDefinition.CharacterCertificate:
                        model.TemplateName = EmailTemplateFileNames.CharacterCertificateRejectionNotification.ToDescription();
                        break;
                }

                string sModel = JsonConvert.SerializeObject(new { Recipient = (string)model.Email, Subject = subject });
                _hangfireNotification.SendNotificationGmail(sModel, data, $"{model.TemplateName}.html");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
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

            try
            {
                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), Name = (string)model.Name, Subject = (string)model.Subject, Message = (string)model.Message, Email = (string)model.Email };

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "Name", dataParams.Name },
                    { "Message", dataParams.Message },
                    { "Subject", dataParams.Subject },
                    { "BaseUrl", dataParams.BaseUrl },
                    { "Email", dataParams.Email }
                };

                string sDataParams = JsonConvert.SerializeObject(dataParams);

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", model.Subject},
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };


                model.TemplateName = PSSPulseTemplateFileNames.ContactUsNotification.ToDescription();
                string recipientEmail = AppSettingsConfigurations.GetSettingsValue(PSSTenantConfigKeys.ContactUsEmailAddress.ToString());

                string sModel = JsonConvert.SerializeObject(new { Recipient = recipientEmail, model.Subject });
                _hangfireNotification.SendNotificationGmail(sModel, data, $"{model.TemplateName}.html");

                isEmailSent = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            return isEmailSent;
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
                var dataParams = new { BaseUrl = PSSUtil.GetBaseUrl(siteName), Name = (string)model.Name, Password = (string)model.Password };

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "Name", dataParams.Name },
                    { "Password", dataParams.Password },
                    { "BaseUrl", dataParams.BaseUrl },
                };

                string sDataParams = JsonConvert.SerializeObject(dataParams);

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", model.Subject},
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };

                model.TemplateName = PSSPulseTemplateFileNames.SubUserPasswordNotification.ToDescription();
                string sModel = JsonConvert.SerializeObject(new { Recipient = (string)model.Email, Subject = model.Subject });
                _hangfireNotification.SendNotificationGmail(sModel, data, $"{model.TemplateName}.html");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
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

                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "Name", dataParams.Name },
                    { "Username", dataParams.Username },
                    { "Password", dataParams.Password },
                    { "BaseUrl", dataParams.BaseUrl },
                    { "AdminBaseUrl", dataParams.AdminBaseUrl },
                };

                string sDataParams = JsonConvert.SerializeObject(dataParams);

                Dictionary<string, string> emailDetails = new Dictionary<string, string>
                {
                    { "Subject", model.Subject},
                    { "Sender", PSSPulseTemplateFileNames.Sender.ToDescription() },
                    { "Params", sDataParams }
                };

                model.TemplateName = PSSPulseTemplateFileNames.AdminUserCreationNotification.ToDescription();
                string sModel = JsonConvert.SerializeObject(new { Recipient = (string)model.Email, Subject = model.Subject });
                _hangfireNotification.SendNotificationGmail(sModel, data, $"{model.TemplateName}.html");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }
    }
}