using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IAdminSettingManager<CBSTenantSettings> : IDependency, IBaseManager<CBSTenantSettings>
    {

        /// <summary>
        /// Check if the collection of expert systems has a root system
        /// </summary>
        /// <returns>ExpertSystemSettings | null</returns>
        ExpertSystemSettings HasRootExpertSystem();


        /// <summary>
        /// Get a list of expert systems
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>IEnumerable{ExpertSystemSettings}</returns>
        IEnumerable<ExpertSystemSettings> GetExpertSystemList(int take, int skip);


        /// <summary>
        /// Get client secret by client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>string | null</returns>
        string GetClientSecretByClientId(string clientId);


        /// <summary>
        /// Get list of expert systems. Returns a partially populated list of objects
        /// </summary>
        /// <returns>List{ExpertSystemSettings}</returns>
        List<ExpertSystemSettings> GetExpertSystemsMDADropDown();



        void Flush();


        /// <summary>
        /// Get root expert system
        /// <para>Returns the future instance</para>
        /// </summary>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        IEnumerable<ExpertSystemVM> GetRootExpertSystem();


        /// <summary>
        /// Get expert system with specified Id
        /// </summary>
        /// <param name="expertSystemId"></param>
        /// <returns>IEnumerable<ExpertSystemVM></returns>
        IEnumerable<ExpertSystemVM> GetExpertSystemById(int expertSystemId);

    }
}
