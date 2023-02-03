using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class Handler : IHandler
    {
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly Lazy<ICoreCBSUserVerification> _coreUserVer;
        private readonly ICoreCollectionService _coreCollectionService;
        private readonly Lazy<IPoliceRankingManager<PoliceRanking>> _policeRankingManager;
        private readonly Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> _escortRateSheetManager;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }


        public Handler(ICBSUserManager<CBSUser> cbsUserService, Lazy<ICoreCBSUserVerification> coreUserVer, ICoreCollectionService coreCollectionService, Lazy<IPoliceRankingManager<PoliceRanking>> policeRankingManager, Lazy<IEscortAmountChartSheetManager<EscortAmountChartSheet>> escortRateSheetManager, IOrchardServices orchardServices)
        {
            _cbsUserService = cbsUserService;
            _coreUserVer = coreUserVer;
            _coreCollectionService = coreCollectionService;
            Logger = NullLogger.Instance;
            _policeRankingManager = policeRankingManager;
            _escortRateSheetManager = escortRateSheetManager;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="id">UserPartRecord Id</param>
        /// <returns>UserDetailsModel</returns>
        public UserDetailsModel GetUserDetails(int id)
        {
            return _cbsUserService.GetUserDetails(id);
        }


        /// <summary>
        /// Checks if user with specified user part record id is an administrator
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        public bool CheckIfUserIsAdministrator(int userPartRecordId)
        {
            return _cbsUserService.Count(x => x.UserPartRecord.Id == userPartRecordId && x.IsAdministrator) > 0;
        }


        /// <summary>
        /// Get user details for this cbs user Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserDetailsModel</returns>
        public UserDetailsModel GetUserDetailsForCBSUserId(long cbsUserId)
        {
            return _cbsUserService.GetUserDetailsForCBSUserId(cbsUserId);
        }


        /// <summary>
        /// Get the verification token
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed after verification provide this object</param>
        /// <returns>string</returns>
        public string ProviderVerificationToken(CBSUserVM cbsUser, VerificationType verificationType, RedirectReturnObject redirectObj = null)
        {
            VerTokenResult queResult = _coreUserVer.Value.QueueVerificationToken(cbsUser, new AccountVerificationEmailNotificationModel { Sender = PSSPulseTemplateFileNames.Sender.GetDescription(), Subject = "Police Service Account Registration Verification", TemplateFileName = PSSPulseTemplateFileNames.AccountVerification.GetDescription() }, verificationType);
            VerTokenEncryptionObject enobj = new VerTokenEncryptionObject { VerId = queResult.VerObjId, PaddingText = PaddingForToken(), CreatedAt = queResult.CreatedAt, RedirectObj = redirectObj };
            return Util.LetsEncrypt(JsonConvert.SerializeObject(enobj));
        }


        /// <summary>
        /// Get the verification token (SMS only)
        /// </summary>
        /// <param name="cbsUser"></param>
        /// <param name="verificationType">Enums.VerificationType</param>
        /// <param name="redirectObj">if a redirect is needed after verification provide this object</param>
        /// <returns>string</returns>
        public string ProviderVerificationTokenSMS(CBSUserVM cbsUser, VerificationType verificationType, RedirectReturnObject redirectObj = null)
        {
            VerTokenResult queResult = _coreUserVer.Value.QueueVerificationToken(cbsUser, verificationType);
            VerTokenEncryptionObject enobj = new VerTokenEncryptionObject { VerId = queResult.VerObjId, PaddingText = PaddingForToken(), VerificationType = (int)verificationType, CreatedAt = queResult.CreatedAt, RedirectObj = redirectObj };
            return Util.LetsEncrypt(JsonConvert.SerializeObject(enobj));
        }


        /// <summary>
        /// Resend verification token
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="NoRecordFoundException">when ver code not found</exception>
        /// <exception cref="InvalidOperationException">when resend count is maxed</exception>
        public void ResendVerificationCode(string token)
        {
            _coreUserVer.Value.ResendCodeNotification(token, new AccountVerificationEmailNotificationModel { Sender = PSSPulseTemplateFileNames.Sender.GetDescription(), Subject = "Police Service Account Registration Verification", TemplateFileName = PSSPulseTemplateFileNames.AccountVerification.GetDescription() });            
        }


        /// <summary>
        /// Check if this user has been verified
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public UserDetailsModel CheckIfUserIsVerified(int userPartId)
        {
            return _cbsUserService.GetUserDetailsForAccountVerification(userPartId);
        }


        private string PaddingForToken()
        {
            byte[] byteBit1 = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] byteBit2 = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(byteBit1.Concat(byteBit2).ToArray());
        }

        /// <summary>
        /// Get escort bill estimate using number of officers requested, duration and the rank amount rate for the lowest rank officer
        /// </summary>
        /// <param name="numberOfOfficers"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <param name="subSubTaxCategoryId"></param>
        /// <returns>decimal</returns>
        public APIResponse GetEscortBillEstimate(int numberOfOfficers, string startDate, string endDate, int stateId, int lgaId, int subSubTaxCategoryId)
        {
            try
            {
                decimal response = _escortRateSheetManager.Value.GetEstimateAmount(stateId, lgaId);

                if(response == 0.0m)
                {
                    return new APIResponse { StatusCode = HttpStatusCode.NoContent, ResponseObject = "Unable to get cost estimate, please try again later" };
                }

                DateTime sDate = DateTime.Now;
                DateTime eDate = DateTime.Now;

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    int escortDuration = 0;
                    try
                    {
                        sDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        eDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        escortDuration = (eDate - sDate).Days + 1; //1 is added because both start and end date are inclusive i.e 02/01/2020 - 01/01/2020 difference will be 1 without an addition of inclusive 1 day 
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    decimal computedAmount = Math.Round((response * numberOfOfficers * escortDuration), 2);
                    return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = new { computedAmount, estimateNote = PoliceLang.escortestimateamount(numberOfOfficers).Text } };
                }
                throw new Exception("Start or end date is empty");
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
                return new APIResponse { Error = true, StatusCode = HttpStatusCode.BadRequest };
            }
        }


        /// <summary>
        /// get invoice URL
        /// </summary>
        /// <param name="bIN"></param>
        /// <returns></returns>
        public string GetInvoiceURL(string bin)
        {
            return _coreCollectionService.GetInvoiceURL(bin);
        }

    }
}