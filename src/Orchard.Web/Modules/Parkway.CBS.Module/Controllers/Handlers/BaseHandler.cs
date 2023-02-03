using System;
using System.Linq;
using System.Collections.Generic;
using Orchard;
using OrchardPermission = Orchard.Security.Permissions.Permission;
using Orchard.Security;
using System.Web.Mvc;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System.Text;
using System.Security.Cryptography;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public abstract class BaseHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        public ILogger Logger { get; set; }
        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;

        protected BaseHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            Logger = NullLogger.Instance;
            _settingsRepository = settingsRepository;
        }

        /// <summary>
        /// Get admin user
        /// </summary>
        /// <param name="id">admin id</param>
        /// <returns>UserPartRecord</returns>
        /// <exception cref="AuthorizedUserNotFoundException">User not found</exception>
        protected UserPartRecord GetUser(int id)
        {
            var user = _settingsRepository.User(id);
            if (user == null) { throw new AuthorizedUserNotFoundException("User with email not found " + id); }
            return user;
        }


        public List<ExpertSystemSettings> GetExpertSystems()
        {
            return _settingsRepository.GetExpertSystemsMDADropDown();
        }

        /// <summary>
        /// Get tenant
        /// </summary>
        /// <returns>CBSTenantSettings</returns>
        /// <exception cref="TenantNotFoundException"></exception>
        protected ExpertSystemSettings GetExpertSystem()
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
        /// Check if the user is authorized to perform an action
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <returns>IBaseHandler</returns>
        protected T IsAuthorized<T>(OrchardPermission permission) where T : BaseHandler
        {
            if (!_authorizer.Authorize(permission, ErrorLang.usernotauthorized()))
                throw new UserNotAuthorizedForThisActionException();
            return (T)this;
        }


        /// <summary>
        /// Check if model state is valid
        /// </summary>
        /// <param name="callback">Controller object</param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <returns>IBaseHandler</returns>
        protected T IsModelValid<T, C>(C callback) where C : Controller
                                                   where T : BaseHandler
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
        public T AddValidationErrorsToCallback<T, C>(C callback, List<ErrorModel> errors, bool throwException = true) where C : Controller
                                                                                          where T : BaseHandler
        {
            if (errors.Count > 0)
            {
                foreach (var item in errors) { callback.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                if (throwException) { throw new DirtyFormDataException(); }
            }
            return (T)this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="soup"></param>
        /// <param name="maggi">mangi</param>
        /// <returns></returns>
        protected string LetMeEncrypt(string soup, string maggi)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(soup);
            byte[] resultArray = null;
            using (TripleDESCryptoServiceProvider treyDES = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                {
                    byte[] keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(maggi));

                    treyDES.Key = keyArray;
                    treyDES.Mode = CipherMode.ECB;
                    treyDES.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cryptor = treyDES.CreateEncryptor();
                    resultArray = cryptor.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        public string LetsDecrypt(string input, string key)
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