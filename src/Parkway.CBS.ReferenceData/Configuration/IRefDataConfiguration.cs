using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Configuration
{
    public interface IRefDataConfiguration
    {
        /// <summary>
        /// Get ref data collection for a particular tenant
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns>List{RefData}</returns>
        List<RefData> GetCollection(string identifier);

    }
}
