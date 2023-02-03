using Newtonsoft.Json;
using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl
{
    public class DevelopmentLevyDAOManager : Repository<TaxEntity>, IDevelopmentLevyDAOManager
    {
        public DevelopmentLevyDAOManager(IUoW uow) : base(uow)
        {

        }

        public List<ReferenceDataGenerateInvoiceModel> GetTaxEntitiesForDevelopmentLevy()
        {
            string configAmount = ConfigurationManager.AppSettings["DevelopmentLevyAmount"];
            string excludingTaxEntityCategory = ConfigurationManager.AppSettings["DevelopmentLevyExcludingTaxEntityCategory"];
            bool parsed = false;
            decimal Amount = 0m;

            parsed = decimal.TryParse(configAmount, out Amount);
            if (!parsed) { throw new Exception("Unable to get configured Development Levy Amount value"); }

            if (string.IsNullOrEmpty(excludingTaxEntityCategory)) { throw new Exception("Unable to get excluding Tax Entity Category configured value"); }
            List<int> excludingIds = JsonConvert.DeserializeObject<List<int>>(excludingTaxEntityCategory);

            return _uow.Session.Query<TaxEntity>()
                .Where(t => !excludingIds.Contains(t.TaxEntityCategory.Id))
                .Select(s =>
                new ReferenceDataGenerateInvoiceModel
                {
                    Address = s.Address,
                    Recipient = s.Recipient,
                    Email = s.Email,
                    TaxProfileId = s.Id,
                    TaxProfileCategoryId = s.TaxEntityCategory.Id,
                    CashflowCustomerId = s.CashflowCustomerId,
                    Amount = Amount
                }).ToList();
        }
    }
}
