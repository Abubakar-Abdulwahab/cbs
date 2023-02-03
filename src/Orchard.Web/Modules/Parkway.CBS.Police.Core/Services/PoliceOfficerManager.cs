using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PoliceOfficerManager : BaseManager<PoliceOfficer>, IPoliceOfficerManager<PoliceOfficer>
    {
        private readonly IRepository<PoliceOfficer> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PoliceOfficerManager(IRepository<PoliceOfficer> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get police officers of the rank with specified rankId that belong to the command with the specified commandId
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        public List<PoliceOfficerVM> GetPoliceOfficersByCommandAndRankId(int commandId, long rankId)
        {
            return _transactionManager.GetSession().Query<PoliceOfficer>().Where(x => x.Command.Id == commandId && x.Rank.Id == rankId).Select(officers => new PoliceOfficerVM
            {
                Id = officers.Id,
                Name = officers.Name,
            }).ToList();
        }


        /// <summary>
        /// Get police officer detials
        /// </summary>
        /// <param name="officerId"></param>
        /// <returns>PoliceOfficerVM</returns>
        public PoliceOfficerVM GetPoliceOfficerDetails(int officerId)
        {
            return _transactionManager.GetSession().Query<PoliceOfficer>()
                .Where(x => ((x.Id == officerId) && (x.IsActive)))
                .Select(officer => new PoliceOfficerVM
                {
                    Id = officer.Id,
                    Name = officer.Name,
                    RankId = officer.Rank.Id,
                    CommandId = officer.Command.Id
                }).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Get officer Id using the ID Number
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns>int</returns>
        public int GetPoliceOfficerId(string idNumber)
        {
            try
            {
                return  _transactionManager.GetSession().Query<PoliceOfficer>()
                    .Where(x => ((x.IdentificationNumber == idNumber) && (x.IsActive))).Single().Id;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Error getting police officer details. Officer identification number {idNumber}");
                throw new NoRecordFoundException($"Officer with identification number {idNumber} not found");
            }
        }

        /// <summary>
        /// Get count for number of active police officer
        /// </summary>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        public IEnumerable<ReportStatsVM> GetTotalOfficers()
        {
            return _transactionManager.GetSession().CreateCriteria<PoliceOfficer>()
                .Add(Restrictions.Where<PoliceOfficer>(x => x.IsActive))
            .SetProjection(
                        Projections.ProjectionList()
                            .Add(Projections.Count<PoliceOfficer>(x => x.Id), "TotalRecordCount")
                    ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }
    }
}