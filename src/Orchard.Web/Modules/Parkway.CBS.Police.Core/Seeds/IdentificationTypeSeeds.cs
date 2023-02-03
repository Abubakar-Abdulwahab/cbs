using Orchard;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Seeds.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Seeds
{
    public class IdentificationTypeSeeds : IIdentificationTypeSeeds
    {
        private readonly IIdentificationTypeManager<IdentificationType> _identificationTypeRepo;
        private readonly IOrchardServices _orchardServices;

        public IdentificationTypeSeeds(IOrchardServices orchardServices, IIdentificationTypeManager<IdentificationType> identificationTypeRepo)
        {
            _orchardServices = orchardServices;
            _identificationTypeRepo = identificationTypeRepo;
        }

        /// <summary>
        /// Populate identification types.
        /// </summary>
        /// <returns></returns>
        public bool PopulateIdentificationTypes()
        {
            try
            {
                List<IdentificationType> identificationTypes = new List<IdentificationType>{
                new IdentificationType{ Name = "National Identification Number", HasIntegration = false, },
                new IdentificationType{ Name = "Driver's License", HasIntegration = false },
                new IdentificationType{ Name = "International Passport", HasIntegration = false },
                new IdentificationType{ Name = "Bank Verification Number", HasIntegration = true, ImplementingClassName = typeof(PSSIdentification.BVNValidation).Name },
                new IdentificationType{ Name = "Tax Identification Number", HasIntegration = false }
            };
                
                return _identificationTypeRepo.SaveBundle(identificationTypes);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}