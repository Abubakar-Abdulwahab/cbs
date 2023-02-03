using Orchard;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.Models;
using System.Web;
using System.Collections.Generic;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface ISettingsHandler : IDependency
    {

        ChangePasswordViewModel ChangePasswordView();

        /// <summary>
        /// Get the view to set state
        /// </summary>
        /// <returns>SetStateViewModel</returns>
        SetStateViewModel SetStateView();


        /// <summary>
        /// Get expert systems view model
        /// </summary>
        /// <returns>ExpertSettingsViewModel</returns>
        ExpertSystemListViewModel GetListOfExpertSystemsView(int skip, int take);


        /// <summary>
        /// Get list of states
        /// </summary>
        /// <param name="context"></param>
        /// <returns>List{CashFlowState}</returns>
        List<CashFlowState> ListOfStates(CashFlowRequestContext context);


        /// <summary>
        /// Get list of banks
        /// </summary>
        /// <param name="context"></param>
        /// <returns>List{CashFlowBank}</returns>
        List<CashFlowBank> ListOfBanks(CashFlowRequestContext context);


        /// <summary>
        /// Get view for reference data
        /// </summary>
        /// <returns>ReferenceDataViewModel</returns>
        ReferenceDataViewModel GetReferenceDataSettingsView();


        /// <summary>
        /// Try save reference data
        /// </summary>
        /// <param name="model"></param>
        void TrySaveReferenceDataSettings(ReferenceDataViewModel model);


        ExpertSettingsViewModel CreateExpertSystemView();


        /// <summary>
        /// Get all ref data registered under this state
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        List<string> GetRegisteredRefData(string stateName);


        /// <summary>
        /// Try save tenant state settings
        /// </summary>
        /// <param name="settingsController"></param>
        /// <param name="stateId">state Id</param>
        void TrySaveTenantStateSettings(SettingsController settingsController, string stateId, string identifier);



        ExpertSettingsViewModel TrySaveNewExpertSystem(SettingsController settingsController, ExpertSystemSettings expertSystemsSettings, HttpFileCollectionBase files, string bank);


        /// <summary>
        /// Get Tenant settings
        /// </summary>
        /// <returns>TenantCBSSettings</returns>
        TenantCBSSettings GetTenantSettings();


        /// <summary>
        /// Get client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>string</returns>
        string GetClientSecret(string clientId);


        /// <summary>
        /// Get view for edit expert system
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>ExpertSettingsViewModel</returns>
        ExpertSettingsViewModel EditExpertSystemView(string identifier);


        /// <summary>
        /// Try update expert system
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="files"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        ExpertSettingsViewModel TryUpdateExpertSystemSettings(string identifier, SettingsController callback, ExpertSettingsViewModel model, HttpFileCollectionBase files, string bank);
    }
}
