using NHibernate;
using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IRevenueHeadManager<RevenueHead> : IDependency, IBaseManager<RevenueHead>
    {
        IFutureValue<dynamic> CountRevenueHead();

        RevenueHead Get(string revenueHeadSlug, int mdaId);
        

        /// <summary>
        /// Get revenue head with the slug and id
        /// </summary>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHead</returns>
        RevenueHead GetById(string slug, int id);
        

        /// <summary>
        /// Return the revenue head that has the parent id and slug
        /// </summary>
        /// <param name="parentrevenueheadid"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>RevenueHead</returns>
        RevenueHead GetSubRevenueHead(int parentrevenueheadid, string revenueHeadSlug);


        /// <summary>
        /// Get a list of billable revenue head that contain query text
        /// </summary>
        /// <param name="queryText"></param>
        /// <returns>IEnumerable{RevenueHead}</returns>
        IEnumerable<RevenueHead> GetBillableCollection(string queryText);


        /// <summary>
        /// Get a list of billable revenue heads
        /// </summary>
        /// <param name="queryText"></param>
        /// <returns>IEnumerable{RevenueHead}</returns>
        List<RevenueHeadLite> GetBillableCollectionForAdminGenerateInvoice();


        /// <summary>
        /// Save bundle with API reference
        /// </summary>
        /// <param name="revenueHeads"></param>
        /// <param name="apiResult"></param>
        /// <returns></returns>
        bool SaveRevenueHeadWithRequestReference(Models.RevenueHead revenueHead, ExpertSystemSettings expertSystem, string requestReference);


        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, and billing info</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>RevenueHeadDetails</returns>
        RevenueHeadDetails GetRevenueHeadDetails(int id);


        /// <summary>
        /// Get revenue head VM
        /// <para>Gets the revenue head VM</para>
        /// </summary>
        /// <param name="id">revenue head Id</param>
        /// <returns>RevenueHeadVM</returns>
        RevenueHeadVM GetRevenueHeadVM(int id);


        /// <summary>
        /// Get all the MDAs that have a billable revenue head attached to it
        /// </summary>
        /// <returns>IEnumerable{MDAVM}</returns>
        IEnumerable<MDAVM> GetMDAsForBillableRevenueHeads();

        /// <summary>
        /// Get active and visible  billable revenue heads
        /// </summary>
        /// <param name="expertSysid"></param>
        /// <param name="mdaId"></param>
        /// <returns>IEnumerable{Models.RevenueHead}</returns>
        IEnumerable<RevenueHeadLite> GetMDAActiveRevenueHeads(int expertSysid, int mdaId);


        /// <summary>
        /// Get details you would need for invoice generation from the revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        RevenueHeadDetailsForInvoiceGeneration GetRevenueHeadDetailsForInvoiceGeneration(int revenueHeadId);


        /// <summary>
        /// Get billing information
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>BillingModel</returns>
        BillingModel GetRevenueHeadBilling(int revenueHeadId);

        /// <summary>
        /// Get revenue heads that belong to this MDA
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadVM}</returns>
        IEnumerable<RevenueHeadVM> GetRevenueHeadsForMDA(int id);

        /// <summary>
        /// Get the revenehead details
        /// </summary>
        /// <returns>RevenueHeadDetails</returns>
        RevenueHeadDetails GetRevenueHeadDetailsForPaye();


        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, billing info and form details</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadVMWithFormValidationDetails(int id);


        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, and billing info</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadVMForInvoiceGeneration(int id);


        /// <summary>
        /// Get the group revenue head details
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        GenerateInvoiceRequestModel GetGroupRevenueHeadVMForInvoiceGeneration(int groupId);


        /// <summary>
        /// Get the list of revenue heads that this user has access and belongs to the given MDA Id
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="mdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>IEnumerable{RevenueHeadDropDownListViewModel}</returns>
        IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeadsOnAccessListForMDA(int adminUserId, int mdaId, AccessType accessType, bool applyAccessRestrictions);

        /// <summary>
        /// Get the list of revenue heads that this user has access to 
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="mdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>IEnumerable{RevenueHeadDropDownListViewModel}</returns>

        IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeadsOnAccessListForMDA(int adminUserId, bool applyAccessRestrictions);

        /// <summary>
        /// Get the list of revenue heads
        /// </summary>
        /// <returns>List<RevenueHeadDropDownListViewModel></returns>
        List<RevenueHeadDropDownListViewModel> GetAllRevenueHeads();


        /// <summary>
        /// Get the list of MDAs for revenue heads that are billable
        /// </summary>
        /// <returns>IEnumerable{MDARevenueHeadsVM}</returns>
        IEnumerable<MDARevenueHeadsVM> GetBillableRevenueHeadGroupByMDA();


        /// <summary>
        /// Get the billable revenue heads IDs that belong to this MDA
        /// </summary>
        /// <param name="mDAId"></param>
        /// <returns>IEnumerable{int}</returns>
        IEnumerable<int> GetBillableRevenueHeadsIDsForMDA(int mDAId);


        /// <summary>
        /// Get the billable revenue heads that belong to this MDA
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns>IEnumerable{int}</returns>
        IEnumerable<RevenueHeadVM> GetBillableRevenueHeadsForMDA(int mdaId);

        /// <summary>
        /// Get the list of revenue heads for the MDA with the specified id that the admin user has access to.
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="adminUserId"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        IEnumerable<RevenueHeadLite> GetRevenueHeadsPerMdaOnAccessList(int mdaId, int adminUserId, bool applyAccessRestrictions);


        /// <summary>
        /// Check if revenue head Id exists with for the specified mda Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mdaId"></param>
        /// <returns>bool</returns>
        bool CheckIfRevenueHeadAndExistsWithMDA(int id, int mdaId);


        /// <summary>
        /// Get revenue head VM
        /// <para>Gets the revenue head VM</para>
        /// </summary>
        /// <param name="id">revenue head Id</param>
        /// <returns>RevenueHeadVM</returns>
        RevenueHeadVM GetRevenueHeadVMByCode(string revenueHeadShortCode);

    }
}
