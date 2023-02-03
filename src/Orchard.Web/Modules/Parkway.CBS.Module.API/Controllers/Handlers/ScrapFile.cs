using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers.Handlers
{
    public class ScrapFile : BaseAPIHandler, IScrapFile
    {

        private readonly ITaxEntityManager<TaxEntity> _repo;
        private readonly IFileUploadConfiguration _scrap;

        public ScrapFile(ITaxEntityManager<TaxEntity> repo, IAdminSettingManager<ExpertSystemSettings> settingsRepository) : base(settingsRepository)
        {
            _repo = repo;
            _settingsRepository = settingsRepository;
            Logger = NullLogger.Instance;
            _scrap = new FileUploadConfiguration { };
        }


        public void ProcessFile()
        {
            var scraps = _scrap.ReadIndv();

            //_repo.SaveFromScrapFile(scraps);
        }

    }
}