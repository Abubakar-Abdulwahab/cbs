using System.Collections.Generic;
using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreSettlementRuleService : IDependency
    {

        /// <summary>
        /// Get list of third party payment providers
        /// </summary>
        /// <returns>SettlementRuleVM</returns>
        SettlementRuleVM GetModelForCreateSettlememtView(List<string> MDAIds);


        /// <summary>
        /// Get billable revenue heads that belong to this mda
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IEnumerable{RevenueHeadVM}</returns>
        IEnumerable<RevenueHeadVM> GetRevenueHeads(int id);


        /// <summary>
        /// Get a list of revenue heads for the mdas with the specified Id
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns></returns>
        dynamic GetRevenueHeadsPerMda(string mdaIds, int userId);


        /// <summary>
        /// Try save settlement rule
        /// <para></para>
        /// </summary>
        /// <param name="model"><see cref="SettlementRuleVM"/></param>
        /// <param name="adminUser"><see cref="UserPartRecord"/></param>
        /// <param name="errors">List of ErrorModel</param>
        void TrySaveSettlementRule(SettlementRuleVM model, UserPartRecord adminUser, ref List<ErrorModel> errors);

        /// <summary>
        /// Try save settlement rule to staging
        /// </summary>
        /// <param name="model"><see cref="SettlementRuleVM"/></param>
        /// <param name="adminUser"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <exception cref="RecordAlreadyExistsException"></exception>
        /// <exception cref="Exception"></exception>
        void TrySaveSettlementRuleToStaging(SettlementRuleVM model, UserPartRecord adminUser, ref List<ErrorModel> errors);
    }
}
