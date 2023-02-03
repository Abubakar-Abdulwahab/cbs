using Newtonsoft.Json;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.Seeds.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Seeds.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Authorize]
    public class SeedsController : Controller
    {
        private readonly ICommandSeeds _commandSeed;
        private readonly ITaxEntitySubCategorySeeds _taxEntitySubCategorySeed;
        private readonly IPoliceOfficerSeeds _officerSeeds;
        private readonly IBankSeeds _bankSeeds;
        private readonly IPSSReasonSeeds _pssSeeds;
        private readonly IEscortChartSheetSeeds _escortChartSheetSeeds;
        private readonly IIdentificationTypeSeeds _identificationTypeSeeds;
        private readonly IPSSHRExternalDataCommandSeed _externalDataCommandSeed;
        private readonly ISettlementsSeed _settlementsSeed;

        public SeedsController(IOrchardServices orchardServices, ICommandSeeds commandSeed, ITaxEntitySubCategorySeeds taxEntitySubCategorySeed, IPSSReasonSeeds pssSeeds, IPoliceOfficerSeeds officerSeeds, IEscortChartSheetSeeds escortChartSheetSeeds, IIdentificationTypeSeeds identificationTypeSeeds, IPSSHRExternalDataCommandSeed externalDataCommandSeed, ISettlementsSeed settlementsSeed, IBankSeeds bankSeeds)
        {
            if (orchardServices.WorkContext.CurrentUser.UserName.ToLower() != "admin") { throw new CBS.Core.Exceptions.UserNotAuthorizedForThisActionException(); }
            _commandSeed = commandSeed;
            _taxEntitySubCategorySeed = taxEntitySubCategorySeed;
            _officerSeeds = officerSeeds;
            _pssSeeds = pssSeeds;
            _escortChartSheetSeeds = escortChartSheetSeeds;
            _identificationTypeSeeds = identificationTypeSeeds;
            _externalDataCommandSeed = externalDataCommandSeed;
            _settlementsSeed = settlementsSeed;
            _bankSeeds = bankSeeds;
        }


        public string PSSSettlementsSeed1(int settlementId, int mdaId, int serviceId, string revenueHeads)
        {
            try
            {
                _settlementsSeed.SeedSettlementConfig1(settlementId, mdaId, serviceId, revenueHeads);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "Seeded";
        }

        public string SeedSettlementsPerService()
        {
            try
            {
                //get all payment providers
                _settlementsSeed.SeedSettlementConfig();
            }
            catch (Exception exception)
            {
                return exception.Message;
                throw;
            }
            return "done";
        }


        public string PopCommands()
        {
            try
            {
                //Get json file
                var commands = Util.GetListOfObjectsFromJSONFile<CommandVM>("commands");
                var commandImportStat = _commandSeed.AddCommands(commands);
                string response = $"{JsonConvert.SerializeObject(commandImportStat)}";
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string PopBanks()
        {
            try
            {
                System.Collections.Generic.List<BankVM> banks = Util.GetListOfObjectsFromJSONFile<BankVM>(SettingsFileNames.Banks.ToString());
                _bankSeeds.PopBanks(banks);
                return "Done seeding banks";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string TaxEntitySubCategory()
        {
            try
            {
                _taxEntitySubCategorySeed.AddTaxEntitySubCategory();
                return "Ok";
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        public string TaxEntitySubSubCategory()
        {
            try
            {
                _taxEntitySubCategorySeed.AddTaxEntitySubSubCategory();
                return "Ok";
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        public string PopOfficers()
        {
            try
            {
                var stats =_officerSeeds.UploadOfficers();
                string response = $"{JsonConvert.SerializeObject(stats)}";
                return response;
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        public string RequestReason()
        {
            try
            {
                _pssSeeds.AddPSSReason();
                return "Ok";
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        public string PopChartSheet()
        {
            try
            {
                var stats = _escortChartSheetSeeds.UploadChartSheet();
                string response = $"{JsonConvert.SerializeObject(stats)}";
                return response;
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        public string PopIdentificationTypes()
        {
            try
            {
                if (_identificationTypeSeeds.PopulateIdentificationTypes()) { return "OK"; }
                else { return "Error!!!"; }
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        /// <summary>
        /// Get and process HR list of commands
        /// </summary>
        /// <returns></returns>
        public string PopHRCommands()
        {
            try
            {
                return _externalDataCommandSeed.GetHRCommands();
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

        public string ChartSheet()
        {
            try
            {
                
                _escortChartSheetSeeds.ProcessChartSheet();
                return "Ok";
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }

    }
}