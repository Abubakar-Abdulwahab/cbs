using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreSettingsService : IDependency
    {

        void DoExpertSystemAccessListMigration();

        /// <summary>
        /// Get list of reference data for this tenant
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns>List{string}</returns>
        List<string> GetRefData(string identifier);


        /// <summary>
        /// Try save reference data settings
        /// </summary>
        /// <param name="model"></param>
        void TrySaveReferenceDataSettings(ExpertSystemSettings tenant, ReferenceDataViewModel model);


        /// <summary>
        /// Try save a new expert system
        /// </summary>
        /// <param name="user"></param>
        /// <param name="files"></param>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <param name="stateId"></param>
        /// <param name="bankId"></param>
        /// <returns>ExpertSettingsViewModel</returns>
        void TrySaveNewExpertSystem(UserPartRecord user, HttpFileCollectionBase files, ref List<ErrorModel> errors, ExpertSystemSettings model, int bankId);


        /// <summary>
        /// Get client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>string</returns>
        string GetClientSecret(string clientId);


        /// <summary>
        /// Check if the state of the tenant has already been set
        /// </summary>
        /// <returns>bool</returns>
        TenantCBSSettings HasTenantStateSettings();



        void TrySaveStateSettings(string identifier, int stateId, string stateName, UserPartRecord user);

        /// <summary>
        /// Get expert systems
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>IEnumerable{ExpertSystemSettings}</returns>
        IEnumerable<ExpertSystemSettings> GetExpertSystems(int take, int skip);


        /// <summary>
        /// Get expert system
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>ExpertSystemSettings</returns>
        ExpertSystemSettings GetExpertSystem(string identifier);


        /// <summary>
        /// try update expert system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <param name="files"></param>
        /// <param name="errors"></param>
        /// <param name="bankId"></param>
        /// <returns>ExpertSettingsViewModel</returns>
        ExpertSettingsViewModel TryUpdateExpertSystemSettings(string expertSystemIentifier, ExpertSystemSettings model, UserPartRecord user, HttpFileCollectionBase files, ref List<ErrorModel> errors, int bankId);
    }
}
