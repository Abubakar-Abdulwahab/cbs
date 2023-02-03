using Orchard;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts
{
    public interface IExternalDataOfficers : IDependency
    {
        /// <summary>
        /// Get policer officer details from the HR external data source. If the reponse has Error to be true, ResponseObject will contain List<PersonnelErrorResponseModel>. If the reponse has Error to be false, ResponseObject will contain PersonnelResponseModel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns>RootPersonnelResponse</returns>
        RootPersonnelResponse GetPoliceOfficer(PersonnelRequestModel requestModel);
    }
}