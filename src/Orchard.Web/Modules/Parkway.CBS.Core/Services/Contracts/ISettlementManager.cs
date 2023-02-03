using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ISettlementRuleManager<Settlement> : IDependency, IBaseManager<Settlement>
    {

        bool SaveRoot(Models.SettlementRule persistModel);

        bool Save(Models.SettlementRule persistModel);


        /// <summary>
        /// Get the settlements that have the given parameters
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="paymentProvider_Id"></param>
        /// <param name="paymentChannel_Id"></param>
        /// <returns>IEnumerable{SettlementRuleVM}</returns>
        IEnumerable<SettlementRuleVM> GetParentSettlements(MDA mda, RevenueHead revenueHead, int paymentProvider_Id, int paymentChannel_Id);
    }
}
