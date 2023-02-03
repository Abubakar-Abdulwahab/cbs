using Orchard;
using Parkway.CBS.Payee.PayeeAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPAYEExemptionTypeManager<PAYEExemptionType> : IDependency, IBaseManager<PAYEExemptionType>
    {
        /// <summary>
        /// Returns a list of PAYEExemptionTypeVM of all active exemptionTypes
        /// </summary>
        /// <returns cref="PAYEExemptionTypeVM">A list of PAYEExemptionTypeVM of all active exemptionTypes</returns>
        IEnumerable<PAYEExemptionTypeVM> GetAllActivePAYEExemptionTypes();
    }
    
}
