using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.PSSUserReport.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.PSSIdentification.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Orchard;
using Orchard.Users.Models;
using System.Text.RegularExpressions;
using Orchard.Users.Services;
using Orchard.ContentManagement;
using Orchard.UI.Notify;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSUserReportHandler : IPSSUserReportHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IPSSUserReportFilter _userReportFilter;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly IIdentificationTypeManager<Core.Models.IdentificationType> _identificationTypeManager;
        private readonly IEnumerable<IIdentificationNumberValidationImpl> _identificationNumbervalidationImpl;
        private readonly IOrchardServices _orchardServices;
        private readonly IUserService _userService;
        private readonly ICBSUserManager<CBSUser> _cbsUserManager;
        ILogger Logger { get; set; }

        public PSSUserReportHandler(IOrchardServices orchardServices, IHandlerComposition handlerComposition, IPSSUserReportFilter userReportFilter, ITaxEntityManager<TaxEntity> taxEntityManager, IIdentificationTypeManager<Core.Models.IdentificationType> identificationTypeManager, IEnumerable<IIdentificationNumberValidationImpl> identificationNumbervalidationImpl, IUserService userService, ICBSUserManager<CBSUser> cbsUserManager)
        {
            _handlerComposition = handlerComposition;
            _userReportFilter = userReportFilter;
            _taxEntityManager = taxEntityManager;
            _identificationTypeManager = identificationTypeManager;
            _identificationNumbervalidationImpl = identificationNumbervalidationImpl;
            _orchardServices = orchardServices;
            _userService = userService;
            _cbsUserManager = cbsUserManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewUsers"></param>

        public void CheckForPermission(Permission canViewUsers)
        {
            _handlerComposition.IsAuthorized(canViewUsers);
        }

        /// <summary>
        /// Gets the view model 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSUserReportVM GetVMForReports(PSSUserReportSearchParams searchParams)
        {
            dynamic recordsAndAggregate = _userReportFilter.GetUserReportViewModel(searchParams);
            IEnumerable<PSSUserReportCBSUserVM> reports = (IEnumerable<PSSUserReportCBSUserVM>)recordsAndAggregate.ReportRecords;

            return new PSSUserReportVM
            {
                Name = searchParams.Name,
                UserName = searchParams.UserName,
                IdentificationNumber = searchParams.IdentificationNumber,
                From = searchParams.StartDate.ToString("dd'/'MM'/'yyyy"),
                End = searchParams.EndDate.ToString("dd'/'MM'/'yyyy"),
                CBSUsers = reports.ToList(),
                TotalNumberOfUsers = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.TotalNumberOfUserRecords).First().TotalRecordCount,
            };
        }


        /// <summary>
        /// Revalidate user with specified payer id
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="errorMessage"></param>
        public void RevalidateUserWithIdentificationNumber(string payerId, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                TaxEntityViewModel taxEntity = _taxEntityManager.GetTaxEntityDetailsWithPayerId(payerId);
                if (taxEntity == null) { throw new Exception($"Tax entity with profile id {payerId} not found"); }
                IdentificationTypeVM identificationType = _identificationTypeManager.GetIdentificationTypeVM(taxEntity.IdType);
                if (identificationType == null) { throw new Exception($"Identification type with id {taxEntity.IdType} not found"); }


                foreach (var identificationTypeImpl in _identificationNumbervalidationImpl)
                {
                    if (identificationTypeImpl.ImplementingClassName == identificationType.ImplementingClassName)
                    {
                        ValidateIdentificationNumberResponseModel response = identificationTypeImpl.Validate(taxEntity.IdNumber, taxEntity.IdType, out errorMessage, true);
                        if (response != null && !response.HasError) 
                        { 
                            identificationTypeImpl.Revalidate(taxEntity.IdNumber);
                            UpdateUserNameAndEmail(response.EmailAddress, _cbsUserManager.GetUserPartRecordIdForAdminCBSUserWithTaxEntityId(taxEntity.Id));
                            return;
                        }
                        else if(response != null && response.HasError)
                        {
                            errorMessage = response.ErrorMessage;
                            return;
                        }
                        else
                        {
                            errorMessage = $"Unable to revalidate user with identification number {taxEntity.IdNumber}";
                            return;
                        }
                    }
                }

                throw new Exception($"No integration for identification type with id {identificationType.Id} and name {identificationType.Name}.");
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Updates username and email for user with specified user part record id
        /// </summary>
        /// <param name="currentEmail"></param>
        /// <param name="userPartRecordId"></param>
        private void UpdateUserNameAndEmail(string currentEmail, int userPartRecordId)
        {
            try
            {
                Logger.Information($"About to update username and email for user with UserPartRecordId {userPartRecordId}");
                if (string.IsNullOrEmpty(currentEmail?.Trim())) 
                { 
                    Logger.Error($"Cannot update username and email for user with UserPartRecordId {userPartRecordId} with email specified - {currentEmail}");
                    return;
                }

                UserPart user = _orchardServices.ContentManager.Get<UserPart>(userPartRecordId, VersionOptions.DraftRequired);

                if (user.Email != currentEmail)
                {
                    Logger.Information($"Updating old email - {user.Email} to new Email - {currentEmail} for user with UserPartRecordId - {userPartRecordId}");
                    if (!_userService.VerifyUserUnicity(userPartRecordId, currentEmail, currentEmail))
                    {
                        throw new Exception($"User with that username and/or email {currentEmail} already exists.");
                    }
                    else if (!Regex.IsMatch(currentEmail ?? "", UserPart.EmailPattern, RegexOptions.IgnoreCase))
                    {
                        throw new Exception($"Invalid email address - {currentEmail} specified for updating user part record with id {userPartRecordId}");
                    }
                    else
                    {
                        user.NormalizedUserName = currentEmail.ToLowerInvariant();
                        user.UserName = currentEmail;
                        user.Email = currentEmail;
                    }
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, $"Error updating username for user with UserPartRecordId {userPartRecordId} and email {currentEmail}");
                throw;
            }
        }
    }
}