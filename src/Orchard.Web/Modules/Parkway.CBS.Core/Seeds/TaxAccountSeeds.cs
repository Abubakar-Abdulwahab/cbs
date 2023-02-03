using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class TaxAccountSeeds : ITaxAccountSeeds
    {
        private readonly ITaxEntityAccountManager<TaxEntityAccount> _repo;
        private readonly ITaxEntityManager<TaxEntity> _entityRepo;

        public TaxAccountSeeds(ITaxEntityAccountManager<TaxEntityAccount> repo, ITaxEntityManager<TaxEntity> entityRepo)
        {
            _repo = repo;
            _entityRepo = entityRepo;
        }


        public bool CreateTaxAccount()
        {
            try
            {
                var entities = _entityRepo.GetCollection(x => x.TaxEntityAccount.Id == 0);
                foreach (var item in entities)
                {
                    var acct = new TaxEntityAccount { };
                    if (!_repo.Save(acct)) { throw new Exception { }; }
                    item.TaxEntityAccount = acct;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}