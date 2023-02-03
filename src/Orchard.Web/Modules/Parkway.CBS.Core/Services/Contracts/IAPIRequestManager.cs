using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IAPIRequestManager<APIRequest> : IDependency, IBaseManager<APIRequest>
    {
        /// <summary>
        /// get resource identifier
        /// </summary>
        /// <param name="requestIdentifier">string</param>
        /// <param name="expertSystem">ExpertSystemSettings</param>
        /// <param name="callerType">CallTypeEnum</param>
        /// <returns>Int64</returns>
        Int64 GetResourseIdentifier(string requestIdentifier, ExpertSystemSettings expertSystem, CallTypeEnum callerType);

    }
}
