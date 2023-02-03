using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Module.ViewModels;
using System.Web.Mvc;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IRevenueHeadHandler : IDependency
    {
        #region List view operations

        /// <summary>
        /// Gets view for the list of revenue heads belonging to this MDA
        /// </summary>
        /// <param name="slug">MDA slug</param>
        /// <returns></returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException"></exception>
        MDA GetRevenueHeadsView(string slug);

        /// <summary>
        /// Gets view for the list of revenue heads belonging to this revenue head
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="id"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHead GetSubRevenueHeadsView(string slug, int id);

        /// <summary>
        /// Get the list of sub revenue heads under the given revenue head
        /// filtered by the given filter value, ordered and search text if provided
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="filterName"></param>
        /// <param name="orderBy"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        List<RevenueHead> GetFilteredRevenueHeadsCollection(List<RevenueHead> list, string filterName = "Enabled", string orderBy = "Name", string searchText = null, bool ascending = true);

        /// <summary>
        /// Get the list of revenue Heads
        /// </summary>
        /// <returns></returns>
        IEnumerable<RevenueHead> GetFirstLevelRevenueHead();

        IEnumerable<RevenueHead> GetBillableRevenueHeads();



        #endregion

        #region Create operations

        /// <summary>
        /// Gets the view to create a revenue head for this MDA
        /// </summary>
        /// <param name="slug">this is the slug value of the mda you are creating a revenue head for</param>
        /// <returns>RevenueHeadCreateView</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="MDARecordNotFoundException">MDARecordNotFoundException</exception>
        RevenueHeadCreateFromMDAView CreateRevenueHeadView(string slug);

        void TryCreateRevenueHead(RevenueHeadController callback, ICollection<RevenueHead> revenueHeads, string slug);

        List<SelectListItem> GetMDAList();

        /// <summary>
        /// View edit form for revenue head
        /// </summary>
        /// <param name="slug"></param>
        /// <returns>SubRevenueHeadCreateViewModel</returns>
        SubRevenueHeadCreateViewModel CreateRevenueHeadViewFromRevenueHead(string slug, int id);

        void TryCreateRevenueHeadFromRevenueHead(RevenueHeadController callback, ICollection<RevenueHead> revenueHeads, string slug, int id);

        IEnumerable<RevenueHead> GetRevenueHeadsLike(string queryText);

        #endregion

        #region edit revenue head operations

        /// <summary>
        /// Gets the view for revenue head edit
        /// </summary>
        /// <param name="revenueHeadSlug">Slug of the revenue head to be edited</param>
        /// <param name="revenueHeadId">ID of the revenue head</param>
        /// <returns>RevenueHeadCreateView</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHeadCreateView GetEditRevenueHeadView(string revenuehead_slug, int revenueHeadId);

        /// <summary>
        /// Save edited revenue head record
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="updatedRevenueHead">Unsaved revenue head record</param>
        /// <param name="revenueHeadSlug">Slug of the to be updated revenue head</param>
        /// <param name="revenueHeadId">ID of the revenue head</param>
        /// <returns>RevenueHead</returns>
        /// /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="CannotUpdateRevenueHeadException"></exception>
        RevenueHead TryUpdateRevenueHead(RevenueHeadController callback, RevenueHead updatedRevenueHead, string revenueHeadSlug, int revenueHeadId);


        /// <summary>
        /// Change the isDeleted status of the revenue head with the given id
        /// </summary>
        /// <param name="id">TaxEntityId of the revenue head</param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        /// <exception cref="CannotUpdateRevenueHeadException"></exception>
        RevenueHead ChangeStatus(int id);

        #endregion

        /// <summary>
        /// Get revenue head with the slug and id
        /// </summary>
        /// <param name="slug">slug</param>
        /// <param name="id">id</param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHead GetRevenueHead(string slug, int id);

        /// <summary>
        /// Get revenue head by TaxEntityId
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHead GetRevenueHead(int id);
        

        /// <summary>
        /// Turn the visibilty of the parent revenue head recursively
        /// </summary>
        /// <param name="revenueHead"></param>
        void TurnOnParentVisibilty(RevenueHead revenueHead);

        ///// <summary>
        ///// Turn the visibilty of the parent revenue head recursively
        ///// </summary>
        ///// <param name="revenueHead"></param>
        //void TurnOffParentVisibilty(RevenueHead revenueHead);
        
        /// <summary>
        /// Get dashboard view
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns></returns>
        RevenueHeadDashboardViewModel GetDashBoardView(string revenueHeadSlug, int revenueHeadId);

        /// <summary>
        /// Get the ul tree for the revenue head hierarchy
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>SortedDictionary{string, string}</returns>
        SortedDictionary<string, string> Tree(RevenueHead revenueHead);


        List<string> GetTaxEntityCategories();


        HierarchyViewModel GetHierarchy(string revenueHeadSlug, int revenueHeadId);


        /// <summary>
        /// Get the list of revenue heads that belong to the givem MDA Id, can be accessed by the user Id and for the given access type
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="mdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>IEnumerable{RevenueHeadDropDownListViewModel}</returns>
        IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeadsOnAccessList(int adminUserId, int mdaId, AccessType accessType, bool applyAccessRestrictions = false);
    }
}
