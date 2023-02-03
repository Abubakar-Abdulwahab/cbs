using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IRevenueHeadDAOManager : IRepository<RevenueHead>
    {
        /// <summary>
        /// Get the model for invoice generation for payee assessment
        /// </summary>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        RevenueHeadDetailsForInvoiceGenerationLite GetRevenueHeadDetailsForInvoiceGenerationForPayee();

        /// <summary>
        /// Get the model for invoice generation for payee assessment
        /// </summary>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        RevenueHeadDetailsForInvoiceGenerationLite GetRevenueHeadDetailsForInvoiceGenerationForPayee(long revenueHeadId);

        /// <summary>
        /// Get the model for invoice generation
        /// </summary>
        /// <returns>RevenueHeadDetailsForInvoiceGeneration</returns>
        RevenueHeadDetailsForInvoiceGenerationLite GetRevenueHeadDetailsForInvoiceGeneration(long revenueHeadId);

        /// <summary>
        /// Get the group revenue head details
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        GenerateInvoiceRequestModel GetGroupRevenueHeadVMForInvoiceGeneration(int groupId);

        /// <summary>
        /// Get revenue head details
        /// <para>Gets the revenue head, mda, and billing info</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadForInvoiceGenerationHelper}</returns>
        IEnumerable<RevenueHeadForInvoiceGenerationHelper> GetRevenueHeadVMForInvoiceGeneration(int id);
    }
}
