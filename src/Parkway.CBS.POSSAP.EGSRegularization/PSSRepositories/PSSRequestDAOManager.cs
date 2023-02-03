using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PSSRequestDAOManager : Repository<PSSRequest>, IPSSRequestDAOManager {
        public PSSRequestDAOManager(IUoW uow) : base(uow)
        {

        }


        /// <summary>
        /// Gets pss request with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PSSRequestTaxEntityViewVM GetPSSRequest(long id)
        {
            return _uow.Session.Query<PSSRequest>().Where(x => x.Id == id).Select(x => 
            new PSSRequestTaxEntityViewVM 
            {
                PSSRequestId = x.Id,
                FileRefNumber = x.FileRefNumber,
                ServiceId = x.Service.Id,
                TaxEntity = new Core.HelperModels.TaxEntityViewModel
                    {
                        Id = x.TaxEntity.Id,
                        CategoryId = x.TaxEntity.TaxEntityCategory.Id,
                        SelectedState = x.TaxEntity.StateLGA.State.Id,
                        Email = x.TaxEntity.Email,
                        CashflowCustomerId = x.TaxEntity.CashflowCustomerId,
                        Recipient = x.TaxEntity.Recipient,
                        PhoneNumber = x.TaxEntity.PhoneNumber
                    },
                CBSUser = new Core.HelperModels.CBSUserVM { Name = x.CBSUser.Name, Email = x.CBSUser.Email, PhoneNumber = x.CBSUser.PhoneNumber }
            }).SingleOrDefault();
        }
    }
}
