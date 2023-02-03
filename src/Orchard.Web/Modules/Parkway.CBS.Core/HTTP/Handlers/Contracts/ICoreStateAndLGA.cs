using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreStateAndLGA : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns>list of LGAs of the state specified by the state Id</returns>
        List<LGA> GetLgas(int stateId);

        List<StateModel> GetStates();


        /// <summary>
        /// Get state Id for this LGA Id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <param name="stateId"></param>
        /// <returns>int</returns>
        int GetStateIdForLGA(int lgaId, int stateId);

        /// <summary>
        /// Checks if state with specified id exists
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        bool ValidateState(int stateId);

        /// <summary>
        /// Get state with specified id
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        IEnumerable<StateModel> GetState(int stateId);

        /// <summary>
        /// get all the states and their LGAs as view models
        /// </summary>
        /// <returns></returns>
        List<StateModelVM> GetStateVMs();

        /// <summary>
        /// Get LGA with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LGAVM GetLGAWithId(int id);

    }
}
