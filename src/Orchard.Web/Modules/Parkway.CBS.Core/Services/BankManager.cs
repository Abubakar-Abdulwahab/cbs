using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class BankManager : BaseManager<Bank>, IBankManager<Bank>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public BankManager(IRepository<Bank> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets all active banks
        /// </summary>
        /// <returns></returns>
        public List<BankViewModel> GetAllActiveBanks()
        {
            return _transactionManager.GetSession().Query<Bank>().Where(x => !x.IsDeleted).Select(x => new BankViewModel
            {
                Name = x.Name,
                ShortName = x.ShortName,
                Code = x.Code,
                Id = x.Id
            }).ToList();
        }

        /// <summary>
        /// Gets active bank by <paramref name="bankCode"/>
        /// </summary>
        /// <returns></returns>
        public BankViewModel GetActiveBankByBankCode(string bankCode)
        {
            return _transactionManager.GetSession().Query<Bank>().Where(x => !x.IsDeleted && x.Code == bankCode).Select(x => new BankViewModel
            {
                Name = x.Name,
                ShortName = x.ShortName,
                Code = x.Code,
                Id = x.Id
            }).Single();
        }

        /// <summary>
        /// Gets active bank by <paramref name="bankId"/>
        /// </summary>
        /// <returns></returns>
        public BankViewModel GetActiveBankByBankId(int bankId)
        {
            return _transactionManager.GetSession().Query<Bank>().Where(x => !x.IsDeleted && x.Id == bankId).Select(x => new BankViewModel
            {
                Name = x.Name,
                ShortName = x.ShortName,
                Code = x.Code,
                Id = x.Id
            }).Single();
        }

        /// <summary>
        /// Checks if a bank exists
        /// </summary>
        /// <param name="bankId"></param>
        /// <returns>CommandVM</returns>
        public bool CheckIfBankExist(int bankId)
        {
            return _transactionManager.GetSession().Query<Bank>().Count(x => !x.IsDeleted && x.Id == bankId) > 0;
        }

    }
}