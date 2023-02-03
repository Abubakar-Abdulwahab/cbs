using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IGenericRequestHandler : IDependency
    {

        /// <summary>
        /// Get VM for police extract
        /// </summary>
        /// <returns>GenericPoliceRequest</returns>
        GenericPoliceRequest GetVMForGenericPoliceRequest(int serviceId, int categoryId);

        /// <summary>
        /// Get next action direction for extract
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns>dynamic</returns>
        dynamic GetNextDirectionForConfirmation();


      
        void GetVMForGenericPoliceRequest(GenericPoliceRequestController callback, IEnumerable<FormControlViewModel> forms, ICollection<FormControlViewModel> controlCollectionFromUserInput);
    }
}
