using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class CBSUserSeeds : ICBSUserSeeds
    {
        private readonly ICBSUserManager<CBSUser> _cbsUserService;

        public CBSUserSeeds(ICBSUserManager<CBSUser> cbsUserService)
        {
            _cbsUserService = cbsUserService;
        }
        public bool SeedCBSUser()
        {
            CBSUser user = new CBSUser
            {
                Name = "Uzo Eziukwu",
                TaxEntity = new TaxEntity { Id = 10151 },
                UserPartRecord = new Orchard.Users.Models.UserPartRecord { Id = 4 },
            };
            if (_cbsUserService.Save(user)) { return true; }
            return false;
        }
    }
}