using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreBVNValidationService : IDependency
    {
        /// <summary>
        /// Validates Bank Verification Number
        /// </summary>
        /// <returns></returns>
        string ValidateBVN(string bvn, out string errorMessage);
    }
}
