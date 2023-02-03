using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class EscortAmountChartSheetDAOManager : Repository<EscortAmountChartSheet>, IEscortAmountChartSheetDAOManager
    {
        public EscortAmountChartSheetDAOManager(IUoW uow) : base(uow)
        {

        }


        /// <summary>
        /// get the rate for this command type and day type
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <param name="dayType"></param>
        /// <returns>decimal</returns>
        public decimal GetRateForUnknownOfficer(int commandTypeId, int dayType)
        {
            return _uow.Session.Query<EscortAmountChartSheet>()
          .Where(er => ((er.PSSEscortDayType.Id == dayType) && (er.CommandType.Id == commandTypeId)))
          .Select(er => er.Rate).ToList().FirstOrDefault();
        }
    }
}
