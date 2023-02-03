using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IStateModelManager<StateModel> : IDependency, IBaseManager<StateModel>
    {
        List<StatesAndLGAs> GetStatesAndLGAs();

        /// <summary>
        /// Get the list of states and lgas
        /// </summary>
        /// <returns>dynamic</returns>
        dynamic GetStatesAndLGAsForOSGOF();

        /// <summary>
        /// Gets state and LGAs
        /// </summary>
        /// <returns>List<StateModel></returns>
        List<StateModel> GetStates();

        /// <summary>
        /// Gets state and LGAs view models
        /// </summary>
        /// <returns>List<StateModelVM></returns>
        List<StateModelVM> GetStateVMs();

        /// <summary>
        /// Gets LGAs for state with specified id
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns></returns>
        List<LGA> GetLgas(int StateId);

        /// <summary>
        /// Gets LGA with specified id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        List<LGA> ValidateLga(int lgaId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        IEnumerable<StateModel> GetState(int stateId);

        /// <summary>
        /// Get state detail using the short name
        /// </summary>
        /// <param name="stateShortName"></param>
        /// <returns>IEnumerable<StateModel></returns>
        IEnumerable<StateModel> GetState(string stateShortName);



        /// <summary>
        /// Get the state Id for this LGA Id
        /// </summary>
        /// <param name="lgaId"></param>
        /// <param name="stateId"></param>
        /// <returns>int</returns>
        int CountStateIdForLGAId(int lgaId, int stateId);
    }
}
