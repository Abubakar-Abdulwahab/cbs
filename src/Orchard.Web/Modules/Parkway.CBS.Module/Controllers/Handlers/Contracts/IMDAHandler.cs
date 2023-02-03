using Orchard;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IMDAHandler : IDependency
    {
        #region Create ops

        /// <summary>
        /// Get view to create MDA (company/SME) on cash flow
        /// </summary>
        /// <returns>MDASettingsViewModel</returns>
        /// <exception cref="TenantNotFoundException"></exception>
        MDASettingsViewModel GetMDASettingsView();

        /// <summary>
        /// Try persist mda and mda settings
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="mda"></param>
        /// <param name="bankCode"></param>
        /// <param name="files"></param>
        /// <param name="useTSA"></param>
        void CreateMDASettingsView(MDAController callback, MDA mda, string sbankId, HttpFileCollectionBase files, bool useTSA = false);

        #endregion

        #region List Operations

        /// <summary>
        /// Get hierarchy view
        /// </summary>
        /// <param name="slug"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        HierarchyViewModel GetHierarchy(string slug);

        /// <summary>
        /// Get list of MDAs view
        /// </summary>
        void ViewList();
        
        /// <summary>
        /// Filter MDA based on parameters provided
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="orderBy"></param>
        /// <param name="searchText"></param>
        /// <returns>List{MDA}</returns>
        List<MDA> GetCollection(string filterName = "Enabled", string orderBy = "Name", string searchText = null, bool ascending = true);

        /// <summary>
        /// Gets view for the list of revenue heads belonging to this MDA
        /// </summary>
        /// <param name="slug">MDA slug</param>
        /// <returns></returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        MDA GetRevenueHeadsView(string slug);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootRevHeads"></param>
        /// <param name="filterName"></param>
        /// <param name="orderBy"></param>
        /// <param name="search"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        List<RevenueHead> GetRevenueHeadsCollection(List<RevenueHead> rootRevHeads, string filterName, string orderBy, string search, bool direction);

        DashboardViewModel GetDashboardView(DateTime startDate, DateTime endDate, string mdaSelected);

        #endregion

        #region Edit Operations

        /// <summary>
        /// Get edit view
        /// </summary>
        /// <param name="slug">MDA slug</param>
        /// <returns>MDASettingsViewModel</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        MDASettingsViewModel GetEditView(string slug);
        
        /// <summary>
        /// Try update MDA record
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="mda"></param>
        /// <returns></returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordCouldNotBeUpdatedException"></exception>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="MissingFieldException"></exception>
        void TryUpdateMDA(MDAController mDAController, MDA mDA, HttpFileCollectionBase files, bool useTSA, string slug, string sbankId);

        /// <summary>
        /// Change the active status of the MDA
        /// </summary>
        /// <param name="id">TaxEntityId of the MDA</param>
        void ChangeStatus(int id);

        #endregion

        /// <summary>
        /// Get MDA record
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>MDA</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        MDA GetMDA(string slug);


        List<CashFlowBank> GetBanks();

        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission createMDA);


        /// <summary>
        /// Get the list of MDAs that this user has access to
        /// </summary>
        /// <returns>IEnumerable{MDAVM}</returns>
        IEnumerable<MDAVM> GetMDAsOnAccessList(int userId, AccessType accessType, bool applyAccessRestrictions);
    }
}
