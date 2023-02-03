using System;
using Parkway.CBS.RSTVL.Core.Models;
using Parkway.CBS.RSTVL.Core.Services.Contracts;
using Parkway.CBS.RSTVL.Core.CoreServices.Contracts;


namespace Parkway.CBS.RSTVL.Core.CoreServices
{
    public class CoreLicence : ICoreLicence
    {
        private readonly IRSTVLicenceManager<RSTVLicence> _licenceRepo;

        public CoreLicence(IRSTVLicenceManager<RSTVLicence> licenceRepo)
        {
            _licenceRepo = licenceRepo;
        }



        public void SaveDetails(RSTVLicence licence)
        {
            if (!_licenceRepo.Save(licence))
            {
                _licenceRepo.RollBackAllTransactions();
                throw new CBS.Core.Exceptions.CouldNotSaveRecord("Could not save licence record");
            }
        }
    }
}