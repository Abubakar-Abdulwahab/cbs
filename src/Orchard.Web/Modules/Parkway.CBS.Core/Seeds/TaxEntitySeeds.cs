using Orchard.Security;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class TaxEntitySeeds : ITaxEntitySeeds
    {
        private readonly ITaxEntityManager<TaxEntity> _repo;
        private readonly ICoreTaxPayerService _coreService;
        private readonly ICoreUserService _coreUserService;
        private readonly IMembershipService _membershipService;

        public TaxEntitySeeds(ITaxEntityManager<TaxEntity> repo, ICoreTaxPayerService coreService, ICoreUserService coreUserService, IMembershipService membershipService)
        {
            _repo = repo;
            _coreService = coreService;
            _coreUserService = coreUserService;
            _membershipService = membershipService;
        }

        public bool GenerateUnknownTaxEntity()
        {
            try
            {
                var catty = new TaxEntityCategory { Id = 1 };

                var taxpayer = _repo.Get(p => p.UnknownProfile && p.TaxEntityCategory == catty);
                if(taxpayer != null) { throw new Exception("An existing unknown Federal profile already exisits"); }
                var errors = new List<HelperModels.ErrorModel> { };
                HelperModels.RegisterCBSUserModel registerModel = new HelperModels.RegisterCBSUserModel
                {
                    Address = "295 Herbert Macaulay way, Yaba, Lagos",
                    Password = DateTime.Now.Ticks.ToString(),
                    Email = "unknown@parkwayprojects.com",
                    Name = "Unknown Federal Agency For Unreconciled Payments",
                    PhoneNumber = "99903940231",
                    UserName = "unknownfederalagency",
                };
                //var registerModel = new HelperModels.RegisterCBSUserModel { };

                _coreUserService.TryCreateCBSUser(registerModel, catty, ref errors);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool GeneratePayerId()
        {
            try
            {
                var profiles = _repo.GetCollection(t => t.PayerId == null);
                foreach (var item in profiles)
                {
                    item.PayerId = _coreService.GetPayerId(item.Id);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}