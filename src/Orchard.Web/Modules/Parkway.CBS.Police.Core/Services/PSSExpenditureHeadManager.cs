using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSExpenditureHeadManager : BaseManager<PSSExpenditureHead>, IPSSExpenditureHeadManager<PSSExpenditureHead>
    {
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }
        public PSSExpenditureHeadManager(IRepository<PSSExpenditureHead> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;

        }

        /// <summary>
        /// Checks if the Name and Code Already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckIfNameOrCodeAlreadyExist(string name, string code)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSExpenditureHead>().Count(x => x.Name == name || x.Code == code) > 0;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Check if the expenditure head exist using <paramref name="expenditureHeadId"/>
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <returns></returns>
        public bool CheckIExpenditureHeadExist(int expenditureHeadId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSExpenditureHead>().Count(x => x.Id == expenditureHeadId) > 0;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Checks if the Name and Code Already does not exist for <paramref name="expenditureHeadId"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="expenditureHeadId"></param>
        /// <returns></returns>
        public bool CheckIfNameOrCodeAlreadyExist(string name, string code, int expenditureHeadId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSExpenditureHead>().Count(x => (x.Name == name || x.Code == code) && x.Id != expenditureHeadId) > 0;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get expenditure head by <paramref name="expenditureHeadId"/>
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <returns></returns>
        public ExpenditureHeadVM GetExpenditureHeadById(int expenditureHeadId)
        {
            return _transactionManager.GetSession().Query<PSSExpenditureHead>().Where(x => x.Id == expenditureHeadId).Select(x => new ExpenditureHeadVM { Id = x.Id, Name = x.Name, Code = x.Code, IsActive = x.IsActive }).SingleOrDefault();
        }

        /// <summary>
        /// Gets all active expenditure head
        /// </summary>
        /// <returns></returns>
        public List<ExpenditureHeadVM> GetActiveExpenditureHead()
        {
            return _transactionManager.GetSession().Query<PSSExpenditureHead>().Where(x => x.IsActive).Select(x => new ExpenditureHeadVM { Id = x.Id, Name = x.Name, Code = x.Code }).ToList();
        }

        /// <summary>
        /// Updates certain columns in PSSExpenditureHead
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lastUpdatedById"></param>
        public void UpdateExpenditureHead(AddExpenditureHeadVM model, int lastUpdatedById)
        {
            PSSExpenditureHead pssExpenditureHead = Get(model.Id) ?? throw new NoRecordFoundException();

            pssExpenditureHead.UpdatedAtUtc = DateTime.Now.ToLocalTime();
            pssExpenditureHead.Code = model.Code.Trim();
            pssExpenditureHead.Name = model.Name.Trim();
            pssExpenditureHead.LastUpdatedBy = new UserPartRecord { Id = lastUpdatedById };
        }

        /// <summary>
        /// Sets IsActive to false
        /// </summary>
        /// <param name="expenditureHeadId"></param>
        /// <param name="isActive"></param>
        /// <param name="lastUpdatedById"></param>
        public void ToggleIsActiveExpenditureHead(int expenditureHeadId, bool isActive, int lastUpdatedById)
        {

            PSSExpenditureHead pssExpenditureHead = Get(expenditureHeadId) ?? throw new NoRecordFoundException();

            pssExpenditureHead.UpdatedAtUtc = DateTime.Now.ToLocalTime();
            pssExpenditureHead.IsActive = isActive;
            pssExpenditureHead.LastUpdatedBy = new UserPartRecord { Id = lastUpdatedById };
        }

    }
}