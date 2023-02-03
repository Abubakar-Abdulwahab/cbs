using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using NHibernate.Linq;
using System.Linq;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortViewRubricManager : BaseManager<EscortViewRubric>, IEscortViewRubricManager<EscortViewRubric>
    {
        private readonly IRepository<EscortViewRubric> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public EscortViewRubricManager(IRepository<EscortViewRubric> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get the rubric for current level to admin level
        /// </summary>
        /// <param name="currentLevelId"></param>
        /// <param name="adminLevelId"></param>
        /// <returns>List{EscortViewRubricDTO}</returns>
        public List<EscortViewRubricDTO> GetPermissionRubric(int currentLevelId, int adminLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortViewRubric>()
                    .Where(x => ((x.RequestLevel.LevelGroupIdentifier == currentLevelId) && (x.ChildLevel.LevelGroupIdentifier == adminLevelId) && (!x.IsDeleted)))
                    .Select(r => new EscortViewRubricDTO { PermissionType = r.PermissionType }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception getting the EscortViewRubricDTO for current level Id " + currentLevelId + " and admin level Id " + adminLevelId);
                throw;
            }
        }


        /// <summary>
        /// Get the rubric for current level to admin level
        /// </summary>
        /// <param name="currentLevelId"></param>
        /// <param name="adminLevelId"></param>
        /// <returns>List{EscortViewRubricDTO}</returns>
        public List<EscortViewRubricDTO> GetPermissionRubric(int adminLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<EscortViewRubric>()
                    .Where(x => ((x.ChildLevel.LevelGroupIdentifier == adminLevelId) && (!x.IsDeleted)))
                    .Select(r => new EscortViewRubricDTO { PermissionType = r.PermissionType }).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception getting the EscortViewRubricDTO for admin level Id " + adminLevelId);
                throw;
            }
        }

    }
}