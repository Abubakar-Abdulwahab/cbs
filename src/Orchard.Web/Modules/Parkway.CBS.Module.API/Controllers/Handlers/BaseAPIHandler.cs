using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public abstract class BaseAPIHandler
    {
        public ILogger Logger { get; set; }
        protected IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        public BaseAPIHandler(IAdminSettingManager<ExpertSystemSettings> settingsRepository)
        {
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get admin user
        /// </summary>
        /// <param name="email">admin email</param>
        /// <returns>UserPartRecord</returns>
        /// <exception cref="AuthorizedUserNotFoundException">User not found</exception>
        protected UserPartRecord GetUser(string email)
        {
            var user = _settingsRepository.User(email);
            if(user == null) { throw new AuthorizedUserNotFoundException("User with email not found " + email); }
            return user;
        }

        /// <summary>
        /// Get tenant
        /// </summary>
        /// <returns>CBSTenantSettings</returns>
        /// <exception cref="TenantNotFoundException"></exception>
        protected ExpertSystemSettings GetExpertSystem(string clientID)
        {
            try
            {
                clientID = clientID.Trim();
                if (string.IsNullOrEmpty(clientID)) throw new TenantNotFoundException("Tenant info could not be found");

                ExpertSystemSettings expertSystem = _settingsRepository.GetCollection(t => t.ClientId == clientID).Single();
                if (expertSystem.ClientId != clientID)
                {
                    throw new TenantNotFoundException("Tenant info could not be found");
                }
                return expertSystem;
            }
            catch (Exception exception)
            {
                throw new TenantNotFoundException(string.Format("Tenant info could not be found - Exception {0} - Type {1}", exception.Message, exception.GetType().ToString()));
            }
        }



        protected void HasStringValue(string value, string fieldName, ref List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(value))
            {
                errors.Add(new ErrorModel { ErrorMessage = string.Format("{0} is required", fieldName), FieldName = fieldName });
                throw new DirtyFormDataException(string.Format("{0} is required", fieldName));
            }
        }


        /// <summary>
        /// Validate date
        /// </summary>
        /// <param name="stringDateValue"></param>
        /// <param name="fieldName"></param>
        /// <param name="format"></param>
        /// <param name="errors"></param>
        /// <returns>DateTime</returns>
        /// <exception cref="DirtyFormDataException">if date value could not be parsed</exception>
        protected DateTime ValidateDate(string stringDateValue, string fieldName, string format, ref List<ErrorModel> errors)
        {
            try
            { return DateTime.ParseExact(stringDateValue.Trim(), format, CultureInfo.InvariantCulture); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error validating date time value: {0}", stringDateValue));
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.dateandtimecouldnotbeparsed(stringDateValue).ToString(), FieldName = fieldName });
                throw new DirtyFormDataException(string.Format("Error validating date time value: {0}", stringDateValue));
            }
        }


        /// <summary>
        /// Compute a hash
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maggi">client secret</param>
        /// <returns>string</returns>
        protected string ComputeHash(string value, string maggi)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(maggi);
            byte[] messageBytes = Encoding.UTF8.GetBytes(value);

            byte[] hashmessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);
            
            // to lowercase hexits
            String.Concat(Array.ConvertAll(hashmessage, x => x.ToString("x2")));

            // to base64
            return Convert.ToBase64String(hashmessage);
        }


        /// <summary>
        /// Check if the hash computed is equal to the hash the user sent
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assertedHash"></param>
        /// <param name="maggi">client secret</param>
        /// <returns>bool</returns>
        protected bool CheckHash(string value, string assertedHash, string maggi)
        {
            return (ComputeHash(value, maggi) == assertedHash);
        }


        /// <summary>
        /// Check model state
        /// </summary>
        /// <typeparam name="API">based on ApiController</typeparam>
        /// <param name="model">API</param>
        /// <returns>List{ErrorModel} <seealso cref="ErrorModel"/></returns>
        protected List<ErrorModel> CheckModelStateWithoutException<API>(API model) where API : ApiController
        {
            if (model.ModelState.IsValid) { return null; }
            List<ErrorModel> errors = new List<ErrorModel> { };
            errors.AddRange(model.ModelState.Keys.SelectMany(key => model.ModelState[key].Errors.Select(x => new ErrorModel { FieldName = key.Contains('.') ? key.Split('.')[1] : key, ErrorMessage = x.ErrorMessage })).ToList());
            return errors;
        }


        /// <summary>
        /// Check model state
        /// </summary>
        /// <typeparam name="API">based on ApiController</typeparam>
        /// <param name="model">API</param>
        /// <param name="errors">List{ErrorModel} <seealso cref="ErrorModel"/></param>
        /// <exception cref="DirtyFormDataException">If model state is invalid</exception>
        protected void CheckModelState<API>(API model, ref List<ErrorModel> errors) where API : ApiController
        {
            if (model.ModelState.IsValid) { return; }

            errors.AddRange(model.ModelState.Keys.SelectMany(key => model.ModelState[key].Errors.Select(x => new ErrorModel { FieldName = key.Contains('.') ? key.Split('.')[1] : key, ErrorMessage = x.ErrorMessage })).ToList());
            throw new DirtyFormDataException("Invalid model state");
        }


        /// <summary>
        /// Check that this scheme is allow for the given params
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="flatScheme"></param>
        /// <param name="mDASettlementType"></param>
        /// <param name="revenueHeadSettlementType"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="Exception"></exception>
        public void CheckSettlementType(string invoiceNumber, bool flatScheme, int mDASettlementType, int revenueHeadSettlementType)
        {
            string msg = string.Empty;
            try
            {
                int settlementType = revenueHeadSettlementType == ((int)SettlementType.None) ? mDASettlementType : revenueHeadSettlementType;
                if (settlementType == ((int)SettlementType.None))
                {
                    msg = string.Format("settlement scheme not specified {0} {1}", (SettlementType)settlementType, invoiceNumber);
                    throw new UserNotAuthorizedForThisActionException(msg);
                }

                if (flatScheme)
                {
                    if (settlementType != (int)SettlementType.Flat)
                    {
                        msg = string.Format("scheme is flat but settlement type is {0} {1} ", (SettlementType)settlementType, invoiceNumber);
                        throw new UserNotAuthorizedForThisActionException(msg);
                    }
                }
                else
                {
                    if (settlementType != (int)SettlementType.Percentage)
                    {
                        msg = string.Format("scheme is percentage but settlement type is {0} ", (SettlementType)settlementType);
                        throw new UserNotAuthorizedForThisActionException();
                    }
                }
            }
            catch (UserNotAuthorizedForThisActionException exception)
            { Logger.Error("Customer ref : " + invoiceNumber + " " + exception.Message); throw; }
            catch (Exception exception)
            { Logger.Error(exception, "Customer ref : " + invoiceNumber + " " + exception.Message); throw; }
        }


    }
}