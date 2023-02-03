using Orchard;
using System.Web.Mvc;
using Orchard.Users.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using System.Web.Http;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreRevenueHeadService : IDependency
    {

        #region Create

        /// <summary>
        /// Try save revenue heads from API
        /// </summary>
        /// <typeparam name="API"></typeparam>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns>CreateRevenueHeadResponseModel</returns>
        CreateRevenueHeadResponseModel TryCreateRevenueHead(UserPartRecord user, ref List<ErrorModel> errors, CreateRevenueHeadsModel model, ExpertSystemSettings expertSystem = null, string requestIdentifier = null);

        #endregion

        
        #region Update

        /// <summary>
        /// Try update a revenue head
        /// </summary>
        /// <param name="revenueHead">RevenueHead, persisted revenuehead</param>
        /// <param name="updatedRevenueHead">RevenueHead, the updated revenue head model</param>
        /// <param name="user">UserPartRecord</param>
        /// <param name="errors">List{ErrorModel}</param>
        CreateRevenueHeadResponseModel TryUpdateRevenueHead(RevenueHead revenueHead, RevenueHead updatedRevenueHead, UserPartRecord user, ref List<ErrorModel> errors);

        #endregion


        /// <summary>
        /// Check if the revenue head has billing information
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <exception cref="RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException"></exception>
        void HasBilling(RevenueHead revenueHead);


        /// <summary>
        /// Get Revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHead GetRevenueHead(int revenueHeadId);


        /// <summary>
        /// Get the view model for this revenue head 
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadVM</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHeadVM GetRevenueHeadVM(int revenueHeadId);


        /// <summary>
        /// Get revenue head with the slug and id
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHead GetRevenueHead(string revenueHeadSlug, int revenueHeadId);


        void TurnOnParentVisibilty(RevenueHead revenueHead);


        void TurnOnParentVisibilty(MDA mda, RevenueHead revenueHead);


        /// <summary>
        /// Has subrevenue heads
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        /// <exception cref="CannotFindRevenueHeadException">Null object</exception>
        bool HasSubRevenueHeads(RevenueHead revenueHead);


        /// <summary>
        /// Check if there is a paye revenue head already set-up
        /// </summary>
        /// <returns>bool</returns>
        bool PayeExists();


        /// <summary>
        /// Get the revenue head details by the revenue head short code
        /// </summary>
        /// <param name="revenueHeadCode"></param>
        /// <returns>RevenueHeadVM</returns>
        RevenueHeadVM GetRevenueHeadVMByCode(string revenueHeadCode);


        /// <summary>
        /// get the Id of the revenue head assigned to be the payee assessment
        /// </summary>
        /// <returns>int?</returns>
        int? PayeeId();


        /// <summary>
        /// Get revenue head details by revenue head Id
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <returns>RevenueHeadDetails</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHeadDetails GetRevenueHeadDetails(int revenueHeadId);


        /// <summary>
        /// Get all the details you would need for invoice generation given the revenue head Id
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHeadDetailsForInvoiceGeneration GetRevenueHeadDetailsForInvoiceGeneration(int revenueHeadId);

        /// <summary>
        /// Get the Id of the paye revenue head
        /// </summary>
        /// <returns>int</returns>
        int GetIdRevenueHeadForPaye();

        /// <summary>
        /// Get revenue head id that is assigned to collect unreconciled collection
        /// </summary>
        /// <returns>int</returns>
        int GetRevenueHeadIdForUnreconciledCollections(string tenantName);


        /// <summary>
        /// Get revenue head details by revenue head Id
        /// <para>An extra flag is passed instructing the method to either get the revenue head details along with the form details or not</para>
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <param name="dontGetFormDetails">Instruct the method to get or not get form details</param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadDetailsForInvoice(int revenueHeadId, bool withFormDetails = true);


        GenerateInvoiceRequestModel GetGroupRevenueHeadDetailsForInvoice(int groupId);
    }
}
