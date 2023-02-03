using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortAmountChartSheetManager : BaseManager<EscortAmountChartSheet>, IEscortAmountChartSheetManager<EscortAmountChartSheet>
    {
        private readonly IRepository<EscortAmountChartSheet> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public EscortAmountChartSheetManager(IRepository<EscortAmountChartSheet> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }



        /// <summary>
        /// get the rate for this rank and category
        /// </summary>
        /// <param name="officerRankId"></param>
        /// <param name="pssEscortServiceCategoryId"></param>
        /// <returns>decimal</returns>
        public decimal GetRateSheetId(long officerRankId, int pssEscortServiceCategoryId, int stateId, int lgaId)
        {
            return _transactionManager.GetSession().Query<EscortAmountChartSheet>()
          .Where(er => ((er.Rank == new PoliceRanking { Id = officerRankId }) && (er.PSSEscortServiceCategory == new PSSEscortServiceCategory { Id = pssEscortServiceCategoryId })
          && (er.State == new CBS.Core.Models.StateModel { Id = stateId }) && (er.LGA == new CBS.Core.Models.LGA { Id = lgaId })))
          .Select(er => er.Rate).ToList().FirstOrDefault();
        }


        /// <summary>
        /// get the rate for this command type and day type
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <param name="dayType"></param>
        /// <returns>decimal</returns>
        public decimal GetRateForUnknownOfficer(int commandTypeId, int dayType)
        {
            return _transactionManager.GetSession().Query<EscortAmountChartSheet>()
          .Where(er => ((er.PSSEscortDayType.Id == dayType) && (er.CommandType.Id == commandTypeId)))
          .Select(er => er.Rate).ToList().FirstOrDefault();
        }


        /// <summary>
        /// get the rate for personnel with this rank, category and command type in specified state and lga
        /// </summary>
        /// <param name="officerRankId"></param>
        /// <param name="pssEscortServiceCategoryId"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public decimal GetRateSheetId(long officerRankId, int pssEscortServiceCategoryId, int stateId, int lgaId, int commandTypeId)
        {
            return _transactionManager.GetSession().Query<EscortAmountChartSheet>()
          .Where(er => ((er.Rank == new PoliceRanking { Id = officerRankId }) && (er.PSSEscortServiceCategory == new PSSEscortServiceCategory { Id = pssEscortServiceCategoryId })
          && (er.State == new CBS.Core.Models.StateModel { Id = stateId }) && (er.LGA == new CBS.Core.Models.LGA { Id = lgaId }) && (er.CommandType.Id == commandTypeId)))
          .Select(er => er.Rate).ToList().FirstOrDefault();
        }


        /// <summary>
        /// Get the min rate
        /// </summary>
        /// <returns></returns>
        public decimal GetMinRankAmountRate(long officerRankId, int pssEscortServiceCategoryId, int stateId, int lgaId)
        {
            return _transactionManager.GetSession().Query<EscortAmountChartSheet>().Where(er => ((er.Rank == new PoliceRanking { Id = officerRankId }) && (er.PSSEscortServiceCategory == new PSSEscortServiceCategory { Id = pssEscortServiceCategoryId }) && (er.State == new CBS.Core.Models.StateModel { Id = stateId }) && (er.LGA == new CBS.Core.Models.LGA { Id = lgaId })))
          .Select(er => er.Rate).FirstOrDefault();
        }


        /// <summary>
        /// Get the estimate amount
        /// </summary>
        /// <returns></returns>
        public decimal GetEstimateAmount(int stateId, int lgaId)
        {
            return _transactionManager.GetSession().Query<EscortAmountChartSheet>().Where(er => (er.State == new CBS.Core.Models.StateModel { Id = stateId }) && (er.LGA == new CBS.Core.Models.LGA { Id = lgaId })).Select(er => er.Rate).FirstOrDefault();
        }

    }
}