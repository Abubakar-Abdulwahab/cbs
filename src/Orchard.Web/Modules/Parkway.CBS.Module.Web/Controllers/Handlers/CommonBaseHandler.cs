using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public abstract class CommonBaseHandler : ICommonBaseHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        public ILogger Logger { get; set; }
        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;


        protected CommonBaseHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            Logger = NullLogger.Instance;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
        }


        /// <summary>
        /// Get admin user
        /// </summary>
        /// <param name="id">admin id</param>
        /// <returns>UserPartRecord</returns>
        /// <exception cref="AuthorizedUserNotFoundException">User not found</exception>
        protected virtual UserPartRecord GetUser(int id)
        {
            var user = _settingsRepository.User(id);
            if (user == null) { throw new AuthorizedUserNotFoundException("User with email not found " + id); }
            return user;
        }


        /// <summary>
        /// check if the invoice has been configured to be generated on another ssystem
        /// <para>If the revenue head details has a InvoiceGenerationRedirectURL value, we should redirect the user to the provided URL </para>
        /// </summary>
        /// <param name="revenueHeadDetails"></param>
        /// <returns>RevenueHeadEssentials</returns>
        public RevenueHeadEssentials CheckForInvoiceGenerationRedirect(RevenueHeadDetails revenueHeadDetails)
        {
            //check if invoice generation can happen on this platform
            if (!string.IsNullOrEmpty(revenueHeadDetails.InvoiceGenerationRedirectURL))
            { return new RevenueHeadEssentials { RevenueHeadDetails = new RevenueHeadDetails { InvoiceGenerationRedirectURL = revenueHeadDetails.InvoiceGenerationRedirectURL, Redirect = true } }; }
            return null;
        }

        protected string GetCatText(TaxEntityCategory category)
        {
            bool ifVowel = new string[5] { "a", "e", "i", "o", "u" }.Contains(category.Name.Substring(0, 1).ToLower());
            return ifVowel ? "An " + category.Name : "A " + category.Name;
        }


        public virtual List<ExpertSystemSettings> GetExpertSystems()
        {
            return _settingsRepository.GetExpertSystemsMDADropDown();
        }


        /// <summary>
        /// Get tenant
        /// </summary>
        /// <returns>CBSTenantSettings</returns>
        /// <exception cref="TenantNotFoundException"></exception>
        protected virtual ExpertSystemSettings GetExpertSystem()
        {
            ExpertSystemSettings expertSystem = new ExpertSystemSettings();
            try
            {
                expertSystem = _settingsRepository.HasRootExpertSystem();
                if (expertSystem == null) { throw new TenantNotFoundException("Could not find tenant info"); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new TenantNotFoundException(string.Format("Tenant info could not be found - Exception {0} - Type {1}", exception.Message, exception.GetType().ToString()));
            }
            return expertSystem;
        }


        /// <summary>
        /// Check if model state is valid
        /// </summary>
        /// <param name="callback">Controller object</param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <returns>IBaseHandler</returns>
        protected virtual T IsModelValid<T, C>(C callback) where C : Controller
                                                   where T : CommonBaseHandler
        {
            //var er = callback.ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            if (!callback.ModelState.IsValid) { throw new DirtyFormDataException(); }
            return (T)this;
        }


        /// <summary>
        /// Add validations errors to the controller model
        /// </summary>
        /// <typeparam name="C">Controller</typeparam>
        /// <param name="callback">Web controller model</param>
        /// <param name="errors">List{ErrorModel}</param>
        /// <returns>T</returns>
        /// <exception cref="DirtyFormDataException"></exception>
        public virtual T AddValidationErrorsToCallback<T, C>(C callback, List<ErrorModel> errors) where C : Controller
                                                                                          where T : CommonBaseHandler
        {
            if (errors.Count > 0)
            {
                foreach (var item in errors) { callback.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                throw new DirtyFormDataException();
            }
            return (T)this;
        }


        [Obsolete("Use the method in the Util class")]
        /// <summary>
        /// Decrypt input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns>string</returns>
        public virtual string LetsDecrypt(string input, string key)
        {
            Logger.Information(string.Format("{0} {0}", input, key));
            byte[] inputArray = Convert.FromBase64String(input);
            byte[] resultArray = null;
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                {
                    byte[] keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                    tripleDES.Key = keyArray;
                    tripleDES.Mode = CipherMode.ECB;
                    tripleDES.Padding = PaddingMode.PKCS7;
                    ICryptoTransform decryptor = tripleDES.CreateDecryptor();
                    resultArray = decryptor.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
            Logger.Information(UTF8Encoding.UTF8.GetString(resultArray));
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        
    }
}