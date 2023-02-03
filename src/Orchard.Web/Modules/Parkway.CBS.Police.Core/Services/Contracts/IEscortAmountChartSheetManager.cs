using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortAmountChartSheetManager<EscortAmountChartSheet> : IDependency, IBaseManager<EscortAmountChartSheet>
    {

        /// <summary>
        /// get the rate for this rank and category
        /// </summary>
        /// <param name="officerRankId"></param>
        /// <param name="subSubTaxCategoryId"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns>decimal</returns>
        decimal GetRateSheetId(long officerRankId, int subSubTaxCategoryId, int stateId, int lgaId);


        /// <summary>
        /// get the rate for this command type and day type
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <param name="dayType"></param>
        /// <returns>decimal</returns>
        decimal GetRateForUnknownOfficer(int commandTypeId, int dayType);


        /// <summary>
        /// get the rate for personnel with this rank, category and command type in specified state and lga
        /// </summary>
        /// <param name="officerRankId"></param>
        /// <param name="pssEscortServiceCategoryId"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        decimal GetRateSheetId(long officerRankId, int pssEscortServiceCategoryId, int stateId, int lgaId, int commandTypeId);


        /// <summary>
        /// Get the min rate
        /// </summary>
        /// <param name="officerRankId"></param>
        /// <param name="subSubTaxCategoryId"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        decimal GetMinRankAmountRate(long officerRankId, int subSubTaxCategoryId, int stateId, int lgaId);


        /// <summary>
        /// Get the estimate amount
        /// </summary>
        /// <returns></returns>
        decimal GetEstimateAmount(int stateId, int lgaId);
    }
}
