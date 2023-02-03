using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Integrations.Contract
{
    /// <summary>
    /// Contains methods for tenant specific reference data access and payment notifcation
    /// All integration adapters for each state module must implement this interface
    /// </summary>
    public interface IBaseIntegration : IDependency
    {
        /// <summary>
        /// Get tax payers 
        /// </summary>
        /// <returns></returns>
        string TaxPayers();

        /// <summary>
        /// Send this tenanat payment notification
        /// </summary>
        /// <returns></returns>
        string PaymentNotification();
    }
}
