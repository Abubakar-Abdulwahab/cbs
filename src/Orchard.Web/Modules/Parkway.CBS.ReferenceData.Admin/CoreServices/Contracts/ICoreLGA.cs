using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts
{
    public interface ICoreLGA : IDependency
    {
        IEnumerable<LGA> GetLGAs(int stateId);

        /// <summary>
        /// Get LGA by Id
        /// </summary>
        /// <param name="lGAId"></param>
        /// <returns>LGA</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        LGA GetLGA(int id);
    }
}
