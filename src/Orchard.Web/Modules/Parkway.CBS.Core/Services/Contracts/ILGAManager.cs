using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ILGAManager<LGA> : IDependency, IBaseManager<LGA>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        IEnumerable<LGA> GetLGA(int lgaId);

        /// <summary>
        /// Get LGA detail using the lga code name
        /// </summary>
        /// <param name="lgaCodeName"></param>
        /// <returns>IEnumerable<LGAVM></returns>
        IEnumerable<LGAVM> GetLGA(string lgaCodeName);

        /// <summary>
        /// Get LGA with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        LGAVM GetLGAWithId(int id);

        /// <summary>
        /// Get LGAs
        /// </summary>
        /// <returns>List<LGAVM></returns>
        List<LGAVM> GetLGAs();
    }
}
