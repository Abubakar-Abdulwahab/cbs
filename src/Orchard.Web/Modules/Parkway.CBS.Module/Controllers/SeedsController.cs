using Newtonsoft.Json;
using Orchard;
using Orchard.Data.Migration;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Notifications.Contracts;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Parkway.CBS.Module.Controllers
{
    [Authorize]
    public class SeedsController : Controller
    {
        private readonly IFormControlsSeeds _formControlsSeed;
        private readonly ITINSeeds _tinSeed;
        private readonly ISectorSeeds _sectorSeed;
        private readonly ICBSUserSeeds _cbsuserSeed;
        private readonly ITaxAccountSeeds _acctSeeds;
        private readonly ITaxEntitySeeds _taxEntitySeeds;
        private readonly IStatsSeeds _statSeeds;
        private readonly ITranxLog _tranxSeeds;
        private readonly IStatesSeeds _stateSeeds;
        private readonly ILGASeeds _lgaSeeds;
        private readonly ICoreInvoiceService _coreInvoiceService;
        private readonly IPaymentNotifications _paymentNotifications;
        private readonly IACL _aclSeeds;
        private readonly ICountrySeeds _countrySeeds;
        private readonly IEthnicitySeeds _ethnicitySeeds;
        private readonly IActivityPermissionSeeds _permissionSeeds;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }


        public SeedsController(IFormControlsSeeds formControlsSeed, ITINSeeds tinSeed, ISectorSeeds sectorSeed, ICBSUserSeeds cbsuserSeed, 
                                ITaxAccountSeeds acctSeeds, ITaxEntitySeeds taxEntitySeeds, IStatsSeeds statSeeds, ITranxLog tranxSeeds, 
                                IStatesSeeds stateSeeds, ILGASeeds lgaSeeds, ICoreInvoiceService coreInvoiceService, IPaymentNotifications paymentNotifications, 
                                IOrchardServices orchardServices, ICountrySeeds countrySeeds, IEthnicitySeeds ethnicitySeeds, IActivityPermissionSeeds permissionSeeds)
        {
            _formControlsSeed = formControlsSeed;
            _tinSeed = tinSeed;
            _sectorSeed = sectorSeed;
            _cbsuserSeed = cbsuserSeed;
            _acctSeeds = acctSeeds;
            _taxEntitySeeds = taxEntitySeeds;
            _statSeeds = statSeeds;
            _tranxSeeds = tranxSeeds;
            _stateSeeds = stateSeeds;
            _lgaSeeds = lgaSeeds;
            _coreInvoiceService = coreInvoiceService;
            _paymentNotifications = paymentNotifications;
            _orchardServices = orchardServices;
            _countrySeeds = countrySeeds;
            _ethnicitySeeds = ethnicitySeeds;
            _permissionSeeds = permissionSeeds;
            Logger = NullLogger.Instance;

        }


        public string CreateRole()
        {
            try
            {
                _aclSeeds.CreateRoles();
                return "Ok";
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public string ResendNotifs(string invoiceNumber)
        {
            try
            {
                //get invoice details
                InvoiceDetails invoiceDeets = _coreInvoiceService.GetInvoiceTransactions(invoiceNumber);

                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                Node setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                    .Node.Where(k => k.Key == TenantConfigKeys.SiteNameOnFile.ToString()).FirstOrDefault();
                string m = string.Empty;

                if (setting != null) { siteName = setting.Value; }

                foreach (var tranx in invoiceDeets.Transactions)
                {
                    string callBackURL = !string.IsNullOrEmpty(invoiceDeets.CallBackURL) ? invoiceDeets.CallBackURL : invoiceDeets.RevenueHeadCallBackURL;
                    if (string.IsNullOrEmpty(callBackURL)) { return "No call back URL found"; }
                    _paymentNotifications.SendPaymentNotification(tranx, callBackURL, siteName, invoiceDeets.ExpertSystemKey, invoiceDeets.RequestRef);
                    m = string.Format("Payment notification sent for model {0} sitename {1} callback {2}", JsonConvert.SerializeObject(tranx), siteName, callBackURL);
                    Logger.Information(m);   
                }
               
                return m;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, "Exception in resend notif " + invoiceNumber);
                throw;
            }
        }
        public string GenerateUnknownTaxEntity()
        {
            try
            {
                bool rst = _taxEntitySeeds.GenerateUnknownTaxEntity();
                return rst.ToString();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public string PopLgas()
        {
            try
            {
                _lgaSeeds.PopLGAs();
                return "OK";
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public string PopStates()
        {
            try
            {
                _stateSeeds.PopulateStates();
                return "OK";
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        //public string StatsPop2()
        //{
        //    try
        //    {
        //        _statSeeds.Populate2();
        //        return "Done";
        //    }
        //    catch (System.Exception exception)
        //    { }
        //    return "Not done";
        //}

        //public string StatsPop1()
        //{
        //    try
        //    {
        //        _statSeeds.Populate();
        //        return "Done";
        //    }
        //    catch (System.Exception exception)
        //    { }
        //    return "Not done";
        //}

        //public string StatsTruncate()
        //{
        //    try
        //    {
        //        _statSeeds.Truncate();
        //        return "Done";
        //    }
        //    catch (System.Exception exception)
        //    { }
        //    return "Not done";
        //}

        //public string CorrectBankCodeForPayDirect()
        //{
        //    try
        //    {
        //        _tranxSeeds.CorrectPaydirectBankCodeIssue();
        //        return "ok";
        //    }
        //    catch (System.Exception)
        //    {
        //    }
        //    return "Error occured";
        //}

        //public string SeedStatsQueryConcat()
        //{
        //    try
        //    {
        //        _statSeeds.DoConcats();
        //        return "Done";
        //    }
        //    catch (System.Exception exception)
        //    { }
        //    return "Not done";
        //}

        //public string SeedPayerId()
        //{
        //    return _taxEntitySeeds.GeneratePayerId().ToString();
        //}


        //public string SeedTaxAccount()
        //{
        //    if (_acctSeeds.CreateTaxAccount()) { return "OK"; }
        //    return "not ok";
        //}

        // GET: Seeds
        //public string Index()
        //{
        //    _formControlsSeed.Seed1();
        //    return "Form controls have been seeded";
        //}

        //public string Tin()
        //{
        //    if (_tinSeed.Seed3()) { return "TIN values have been seeded."; }
        //    return "Could not seed table.";
        //}

        //public string Tin1()
        //{
        //    if (_tinSeed.Seed1()) { return "TIN values have been seeded."; }
        //    return "Could not seed table.";
        //}

        //public string Tin2()
        //{
        //    if (_tinSeed.Seed2()) { return "TIN 2 values have been seeded."; }
        //    return "Could not seed tin 2";
        //}

        public string Sectors()
        {
            _sectorSeed.SeedSectors();
            return "Sectors have been seeded";
        }

        public string CBSUser()
        {
            if (_cbsuserSeed.SeedCBSUser()) { return "Seeded"; }
            else { return "seed faileed"; }
        }


        //public string Tin4()
        //{
        //    return _tinSeed.Seed4();
        //}


        public string SeedStateLga()
        {
            string lgaJson = "[{\"name\":\"Aba North\",\"id\":2},{\"name\":\"Aba South\",\"id\":2},{\"name\":\"Arochukwu\",\"id\":2},{\"name\":\"Bende\",\"id\":2},{\"name\":\"Ikwuano\",\"id\":2},{\"name\":\"Isiala Ngwa North\",\"id\":2},{\"name\":\"Isiala Ngwa South\",\"id\":2},{\"name\":\"Isuikwuato\",\"id\":2},{\"name\":\"Obi Ngwa\",\"id\":2},{\"name\":\"Ohafia\",\"id\":2},{\"name\":\"Osisioma\",\"id\":2},{\"name\":\"Ugwunagbo\",\"id\":2},{\"name\":\"Ukwa East\",\"id\":2},{\"name\":\"Ukwa West\",\"id\":2},{\"name\":\"Umuahia North\",\"id\":2},{\"name\":\"Umuahia South\",\"id\":2},{\"name\":\"Umu Nneochi\",\"id\":2},{\"name\": \"Demsa\",\"id\": 3},{\"name\": \"Fufure\",\"id\": 3},{\"name\": \"Ganye\",\"id\": 3},{\"name\": \"Gayuk\",\"id\": 3},{\"name\": \"Gombi\",\"id\": 3},{\"name\": \"Grie\",\"id\": 3},{\"name\": \"Hong\",\"id\": 3},{\"name\": \"Jada\",\"id\": 3},{\"name\": \"Lamurde\",\"id\": 3},{\"name\": \"Madagali\",\"id\": 3},{\"name\": \"Maiha\",\"id\": 3},{\"name\": \"Mayo Belwa\",\"id\": 3},{\"name\": \"Michika\",\"id\": 3},{\"name\": \"Mubi North\",\"id\": 3},{\"name\": \"Mubi South\",\"id\": 3},{\"name\": \"Numan\",\"id\": 3},{\"name\": \"Shelleng\",\"id\": 3},{\"name\": \"Song\",\"id\": 3},{\"name\": \"Toungo\",\"id\": 3},{\"name\": \"Yola North\",\"id\": 3},{\"name\": \"Yola South\",\"id\": 3},{\"name\": \"Abak\",\"id\": 4},{\"name\": \"Eastern Obolo\",\"id\": 4},{\"name\": \"Eket\",\"id\": 4},{\"name\": \"Esit Eket\",\"id\": 4},{\"name\": \"Essien Udim\",\"id\": 4},{\"name\": \"Etim Ekpo\",\"id\": 4},{\"name\": \"Etinan\",\"id\": 4},{\"name\": \"Ibeno\",\"id\": 4},{\"name\": \"Ibesikpo Asutan\",\"id\": 4},{\"name\": \"Ibiono-Ibom\",\"id\": 4},{\"name\": \"Ika\",\"id\": 4},{\"name\": \"Ikono\",\"id\": 4},{\"name\": \"Ikot Abasi\",\"id\": 4},{\"name\": \"Ikot Ekpene\",\"id\": 4},{\"name\": \"Ini\",\"id\": 4},{\"name\": \"Itu\",\"id\": 4},{\"name\": \"Mbo\",\"id\": 4},{\"name\": \"Mkpat-Enin\",\"id\": 4},{\"name\": \"Nsit-Atai\",\"id\": 4},{\"name\": \"Nsit-Ibom\",\"id\": 4},{\"name\": \"Nsit-Ubium\",\"id\": 4},{\"name\": \"Obot Akara\",\"id\": 4},{\"name\": \"Okobo\",\"id\": 4},{\"name\": \"Onna\",\"id\": 4},{\"name\": \"Oron\",\"id\": 4},{\"name\": \"Oruk Anam\",\"id\": 4},{\"name\": \"Udung-Uko\",\"id\": 4},{\"name\": \"Ukanafun\",\"id\": 4},{\"name\": \"Uruan\",\"id\": 4},{\"name\": \"Urue-OffongOruko\",\"id\": 4},{\"name\": \"Uyo\",\"id\": 4},{\"name\": \"Aguata\",\"id\": 5},{\"name\": \"Anambra East\",\"id\": 5},{\"name\": \"Anambra West\",\"id\": 5},{\"name\": \"Anaocha\",\"id\": 5},{\"name\": \"Awka North\",\"id\": 5},{\"name\": \"Awka South\",\"id\": 5},{\"name\": \"Ayamelum\",\"id\": 5},{\"name\": \"Dunukofia\",\"id\": 5},{\"name\": \"Ekwusigo\",\"id\": 5},{\"name\": \"Idemili North\",\"id\": 5},{\"name\": \"Idemili South\",\"id\": 5},{\"name\": \"Ihiala\",\"id\": 5},{\"name\": \"Njikoka\",\"id\": 5},{\"name\": \"Nnewi North\",\"id\": 5},{\"name\": \"Nnewi South\",\"id\": 5},{\"name\": \"Ogbaru\",\"id\": 5},{\"name\": \"Onitsha North\",\"id\": 5},{\"name\": \"Onitsha South\",\"id\": 5},{\"name\": \"Orumba North\",\"id\": 5},{\"name\": \"Orumba South\",\"id\": 5},{\"name\": \"Oyi\",\"id\": 5},{\"name\": \"Alkaleri\",\"id\": 6},{\"name\": \"Bauchi\",\"id\": 6},{\"name\": \"Bogoro\",\"id\": 6},{\"name\": \"Damban\",\"id\": 6},{\"name\": \"Darazo\",\"id\": 6},{\"name\": \"Dass\",\"id\": 6},{\"name\": \"Gamawa\",\"id\": 6},{\"name\": \"Ganjuwa\",\"id\": 6},{\"name\": \"Giade\",\"id\": 6},{\"name\": \"ItasGadau\",\"id\": 6},{\"name\": \"Jama'are\",\"id\": 6},{\"name\": \"Katagum\",\"id\": 6},{\"name\": \"Kirfi\",\"id\": 6},{\"name\": \"Misau\",\"id\": 6},{\"name\": \"Ningi\",\"id\": 6},{\"name\": \"Shira\",\"id\": 6},{\"name\": \"Tafawa Balewa\",\"id\": 6},{\"name\": \"Toro\",\"id\": 6},{\"name\": \"Warji\",\"id\": 6},{\"name\": \"Zaki\",\"id\": 6},{\"name\": \"Brass\",\"id\": 7},{\"name\": \"Ekeremor\",\"id\": 7},{\"name\": \"KolokumaOpokuma\",\"id\": 7},{\"name\": \"Nembe\",\"id\": 7},{\"name\": \"Ogbia\",\"id\": 7},{\"name\": \"Sagbama\",\"id\": 7},{\"name\": \"Southern Ijaw\",\"id\": 7},{\"name\": \"Yenagoa\",\"id\": 7},{\"name\": \"Agatu\",\"id\": 8},{\"name\": \"Apa\",\"id\": 8},{\"name\": \"Ado\",\"id\": 8},{\"name\": \"Buruku\",\"id\": 8},{\"name\": \"Gboko\",\"id\": 8},{\"name\": \"Guma\",\"id\": 8},{\"name\": \"Gwer East\",\"id\": 8},{\"name\": \"Gwer West\",\"id\": 8},{\"name\": \"Katsina-Ala\",\"id\": 8},{\"name\": \"Konshisha\",\"id\": 8},{\"name\": \"Kwande\",\"id\": 8},{\"name\": \"Logo\",\"id\": 8},{\"name\": \"Makurdi\",\"id\": 8},{\"name\": \"Obi\",\"id\": 8},{\"name\": \"Ogbadibo\",\"id\": 8},{\"name\": \"Ohimini\",\"id\": 8},{\"name\": \"Oju\",\"id\": 8},{\"name\": \"Okpokwu\",\"id\": 8},{\"name\": \"Oturkpo\",\"id\": 8},{\"name\": \"Tarka\",\"id\": 8},{\"name\": \"Ukum\",\"id\": 8},{\"name\": \"Ushongo\",\"id\": 8},{\"name\": \"Vandeikya\",\"id\": 8},{\"name\": \"Abadam\",\"id\": 9},{\"name\": \"AskiraUba\",\"id\": 9},{\"name\": \"Bama\",\"id\": 9},{\"name\": \"Bayo\",\"id\": 9},{\"name\": \"Biu\",\"id\": 9},{\"name\": \"Chibok\",\"id\": 9},{\"name\": \"Damboa\",\"id\": 9},{\"name\": \"Dikwa\",\"id\": 9},{\"name\": \"Gubio\",\"id\": 9},{\"name\": \"Guzamala\",\"id\": 9},{\"name\": \"Gwoza\",\"id\": 9},{\"name\": \"Hawul\",\"id\": 9},{\"name\": \"Jere\",\"id\": 9},{\"name\": \"Kaga\",\"id\": 9},{\"name\": \"KalaBalge\",\"id\": 9},{\"name\": \"Konduga\",\"id\": 9},{\"name\": \"Kukawa\",\"id\": 9},{\"name\": \"Kwaya Kusar\",\"id\": 9},{\"name\": \"Mafa\",\"id\": 9},{\"name\": \"Magumeri\",\"id\": 9},{\"name\": \"Maiduguri\",\"id\": 9},{\"name\": \"Marte\",\"id\": 9},{\"name\": \"Mobbar\",\"id\": 9},{\"name\": \"Monguno\",\"id\": 9},{\"name\": \"Ngala\",\"id\": 9},{\"name\": \"Nganzai\",\"id\": 9},{\"name\": \"Shani\",\"id\": 9},{\"name\": \"Abi\",\"id\": 10},{\"name\": \"Akamkpa\",\"id\": 10},{\"name\": \"Akpabuyo\",\"id\": 10},{\"name\": \"Bakassi\",\"id\": 10},{\"name\": \"Bekwarra\",\"id\": 10},{\"name\": \"Biase\",\"id\": 10},{\"name\": \"Boki\",\"id\": 10},{\"name\": \"Calabar Municipal\",\"id\": 10},{\"name\": \"Calabar South\",\"id\": 10},{\"name\": \"Etung\",\"id\": 10},{\"name\": \"Ikom\",\"id\": 10},{\"name\": \"Obanliku\",\"id\": 10},{\"name\": \"Obubra\",\"id\": 10},{\"name\": \"Obudu\",\"id\": 10},{\"name\": \"Odukpani\",\"id\": 10},{\"name\": \"Ogoja\",\"id\": 10},{\"name\": \"Yakuur\",\"id\": 10},{\"name\": \"Yala\",\"id\": 10},{\"name\": \"Aniocha North\",\"id\": 11},{\"name\": \"Aniocha South\",\"id\": 11},{\"name\": \"Bomadi\",\"id\": 11},{\"name\": \"Burutu\",\"id\": 11},{\"name\": \"Ethiope East\",\"id\": 11},{\"name\": \"Ethiope West\",\"id\": 11},{\"name\": \"Ika North East\",\"id\": 11},{\"name\": \"Ika South\",\"id\": 11},{\"name\": \"Isoko North\",\"id\": 11},{\"name\": \"Isoko South\",\"id\": 11},{\"name\": \"Ndokwa East\",\"id\": 11},{\"name\": \"Ndokwa West\",\"id\": 11},{\"name\": \"Okpe\",\"id\": 11},{\"name\": \"Oshimili North\",\"id\": 11},{\"name\": \"Oshimili South\",\"id\": 11},{\"name\": \"Patani\",\"id\": 11},{\"name\": \"Sapele\",\"id\": 11},{\"name\": \"Udu\",\"id\": 11},{\"name\": \"Ughelli North\",\"id\": 11},{\"name\": \"Ughelli South\",\"id\": 11},{\"name\": \"Ukwuani\",\"id\": 11},{\"name\": \"Uvwie\",\"id\": 11},{\"name\": \"Warri North\",\"id\": 11},{\"name\": \"Warri South\",\"id\": 11},{\"name\": \"Warri South West\",\"id\": 11},{\"name\": \"Abakaliki\",\"id\": 12},{\"name\": \"Afikpo North\",\"id\": 12},{\"name\": \"Afikpo South\",\"id\": 12},{\"name\": \"Ebonyi\",\"id\": 12},{\"name\": \"Ezza North\",\"id\": 12},{\"name\": \"Ezza South\",\"id\": 12},{\"name\": \"Ikwo\",\"id\": 12},{\"name\": \"Ishielu\",\"id\": 12},{\"name\": \"Ivo\",\"id\": 12},{\"name\": \"Izzi\",\"id\": 12},{\"name\": \"Ohaozara\",\"id\": 12},{\"name\": \"Ohaukwu\",\"id\": 12},{\"name\": \"Onicha\",\"id\": 12},{\"name\": \"Akoko-Edo\",\"id\": 13},{\"name\": \"Egor\",\"id\": 13},{\"name\": \"Esan Central\",\"id\": 13},{\"name\": \"Esan North-East\",\"id\": 13},{\"name\": \"Esan South-East\",\"id\": 13},{\"name\": \"Esan West\",\"id\": 13},{\"name\": \"Etsako Central\",\"id\": 13},{\"name\": \"Etsako East\",\"id\": 13},{\"name\": \"Etsako West\",\"id\": 13},{\"name\": \"Igueben\",\"id\": 13},{\"name\": \"Ikpoba Okha\",\"id\": 13},{\"name\": \"Orhionmwon\",\"id\": 13},{\"name\": \"Oredo\",\"id\": 13},{\"name\": \"Ovia North-East\",\"id\": 13},{\"name\": \"Ovia South-West\",\"id\": 13},{\"name\": \"Owan East\",\"id\": 13},{\"name\": \"Owan West\",\"id\": 13},{\"name\": \"Uhunmwonde\",\"id\": 13},{\"name\": \"Ado Ekiti\",\"id\": 14},{\"name\": \"Efon\",\"id\": 14},{\"name\": \"Ekiti East\",\"id\": 14},{\"name\": \"Ekiti South-West\",\"id\": 14},{\"name\": \"Ekiti West\",\"id\": 14},{\"name\": \"Emure\",\"id\": 14},{\"name\": \"Gbonyin\",\"id\": 14},{\"name\": \"Ido Osi\",\"id\": 14},{\"name\": \"Ijero\",\"id\": 14},{\"name\": \"Ikere\",\"id\": 14},{\"name\": \"Ikole\",\"id\": 14},{\"name\": \"Ilejemeje\",\"id\": 14},{\"name\": \"IrepodunIfelodun\",\"id\": 14},{\"name\": \"IseOrun\",\"id\": 14},{\"name\": \"Moba\",\"id\": 14},{\"name\": \"Oye\",\"id\": 14},{\"name\": \"Aninri\",\"id\": 15},{\"name\": \"Awgu\",\"id\": 15},{\"name\": \"Enugu East\",\"id\": 15},{\"name\": \"Enugu North\",\"id\": 15},{\"name\": \"Enugu South\",\"id\": 15},{\"name\": \"Ezeagu\",\"id\": 15},{\"name\": \"Igbo Etiti\",\"id\": 15},{\"name\": \"Igbo Eze North\",\"id\": 15},{\"name\": \"Igbo Eze South\",\"id\": 15},{\"name\": \"Isi Uzo\",\"id\": 15},{\"name\": \"Nkanu East\",\"id\": 15},{\"name\": \"Nkanu West\",\"id\": 15},{\"name\": \"Nsukka\",\"id\": 15},{\"name\": \"Oji River\",\"id\": 15},{\"name\": \"Udenu\",\"id\": 15},{\"name\": \"Udi\",\"id\": 15},{\"name\": \"Uzo Uwani\",\"id\": 15},{\"name\": \"Abaji\",\"id\": 37},{\"name\": \"Bwari\",\"id\": 37},{\"name\": \"Gwagwalada\",\"id\": 37},{\"name\": \"Kuje\",\"id\": 37},{\"name\": \"Kwali\",\"id\": 37},{\"name\": \"Municipal Area Council\",\"id\": 37},{\"name\": \"Akko\",\"id\": 16},{\"name\": \"Balanga\",\"id\": 16},{\"name\": \"Billiri\",\"id\": 16},{\"name\": \"Dukku\",\"id\": 16},{\"name\": \"Funakaye\",\"id\": 16},{\"name\": \"Gombe\",\"id\": 16},{\"name\": \"Kaltungo\",\"id\": 16},{\"name\": \"Kwami\",\"id\": 16},{\"name\": \"Nafada\",\"id\": 16},{\"name\": \"Shongom\",\"id\": 16},{\"name\": \"YamaltuDeba\",\"id\": 16},{\"name\": \"Aboh Mbaise\",\"id\": 17},{\"name\": \"Ahiazu Mbaise\",\"id\": 17},{\"name\": \"Ehime Mbano\",\"id\": 17},{\"name\": \"Ezinihitte\",\"id\": 17},{\"name\": \"Ideato North\",\"id\": 17},{\"name\": \"Ideato South\",\"id\": 17},{\"name\": \"IhitteUboma\",\"id\": 17},{\"name\": \"Ikeduru\",\"id\": 17},{\"name\": \"Isiala Mbano\",\"id\": 17},{\"name\": \"Isu\",\"id\": 17},{\"name\": \"Mbaitoli\",\"id\": 17},{\"name\": \"Ngor Okpala\",\"id\": 17},{\"name\": \"Njaba\",\"id\": 17},{\"name\": \"Nkwerre\",\"id\": 17},{\"name\": \"Nwangele\",\"id\": 17},{\"name\": \"Obowo\",\"id\": 17},{\"name\": \"Oguta\",\"id\": 17},{\"name\": \"OhajiEgbema\",\"id\": 17},{\"name\": \"Okigwe\",\"id\": 17},{\"name\": \"Orlu\",\"id\": 17},{\"name\": \"Orsu\",\"id\": 17},{\"name\": \"Oru East\",\"id\": 17},{\"name\": \"Oru West\",\"id\": 17},{\"name\": \"Owerri Municipal\",\"id\": 17},{\"name\": \"Owerri North\",\"id\": 17},{\"name\": \"Owerri West\",\"id\": 17},{\"name\": \"Unuimo\",\"id\": 17},{\"name\": \"Auyo\",\"id\": 18},{\"name\": \"Babura\",\"id\": 18},{\"name\": \"Biriniwa\",\"id\": 18},{\"name\": \"Birnin Kudu\",\"id\": 18},{\"name\": \"Buji\",\"id\": 18},{\"name\": \"Dutse\",\"id\": 18},{\"name\": \"Gagarawa\",\"id\": 18},{\"name\": \"Garki\",\"id\": 18},{\"name\": \"Gumel\",\"id\": 18},{\"name\": \"Guri\",\"id\": 18},{\"name\": \"Gwaram\",\"id\": 18},{\"name\": \"Gwiwa\",\"id\": 18},{\"name\": \"Hadejia\",\"id\": 18},{\"name\": \"Jahun\",\"id\": 18},{\"name\": \"Kafin Hausa\",\"id\": 18},{\"name\": \"Kazaure\",\"id\": 18},{\"name\": \"Kiri Kasama\",\"id\": 18},{\"name\": \"Kiyawa\",\"id\": 18},{\"name\": \"Kaugama\",\"id\": 18},{\"name\": \"Maigatari\",\"id\": 18},{\"name\": \"Malam Madori\",\"id\": 18},{\"name\": \"Miga\",\"id\": 18},{\"name\": \"Ringim\",\"id\": 18},{\"name\": \"Roni\",\"id\": 18},{\"name\": \"Sule Tankarkar\",\"id\": 18},{\"name\": \"Taura\",\"id\": 18},{\"name\": \"Yankwashi\",\"id\": 18},{\"name\": \"Birnin Gwari\",\"id\": 19},{\"name\": \"Chikun\",\"id\": 19},{\"name\": \"Giwa\",\"id\": 19},{\"name\": \"Igabi\",\"id\": 19},{\"name\": \"Ikara\",\"id\": 19},{\"name\": \"Jaba\",\"id\": 19},{\"name\": \"Jema'a\",\"id\": 19},{\"name\": \"Kachia\",\"id\": 19},{\"name\": \"Kaduna North\",\"id\": 19},{\"name\": \"Kaduna South\",\"id\": 19},{\"name\": \"Kagarko\",\"id\": 19},{\"name\": \"Kajuru\",\"id\": 19},{\"name\": \"Kaura\",\"id\": 19},{\"name\": \"Kauru\",\"id\": 19},{\"name\": \"Kubau\",\"id\": 19},{\"name\": \"Kudan\",\"id\": 19},{\"name\": \"Lere\",\"id\": 19},{\"name\": \"Makarfi\",\"id\": 19},{\"name\": \"Sabon Gari\",\"id\": 19},{\"name\": \"Sanga\",\"id\": 19},{\"name\": \"Soba\",\"id\": 19},{\"name\": \"Zangon Kataf\",\"id\": 19},{\"name\": \"Zaria\",\"id\": 19},{\"name\": \"Ajingi\",\"id\": 20},{\"name\": \"Albasu\",\"id\": 20},{\"name\": \"Bagwai\",\"id\": 20},{\"name\": \"Bebeji\",\"id\": 20},{\"name\": \"Bichi\",\"id\": 20},{\"name\": \"Bunkure\",\"id\": 20},{\"name\": \"Dala\",\"id\": 20},{\"name\": \"Dambatta\",\"id\": 20},{\"name\": \"Dawakin Kudu\",\"id\": 20},{\"name\": \"Dawakin Tofa\",\"id\": 20},{\"name\": \"Doguwa\",\"id\": 20},{\"name\": \"Fagge\",\"id\": 20},{\"name\": \"Gabasawa\",\"id\": 20},{\"name\": \"Garko\",\"id\": 20},{\"name\": \"Garun Mallam\",\"id\": 20},{\"name\": \"Gaya\",\"id\": 20},{\"name\": \"Gezawa\",\"id\": 20},{\"name\": \"Gwale\",\"id\": 20},{\"name\": \"Gwarzo\",\"id\": 20},{\"name\": \"Kabo\",\"id\": 20},{\"name\": \"Kano Municipal\",\"id\": 20},{\"name\": \"Karaye\",\"id\": 20},{\"name\": \"Kibiya\",\"id\": 20},{\"name\": \"Kiru\",\"id\": 20},{\"name\": \"Kumbotso\",\"id\": 20},{\"name\": \"Kunchi\",\"id\": 20},{\"name\": \"Kura\",\"id\": 20},{\"name\": \"Madobi\",\"id\": 20},{\"name\": \"Makoda\",\"id\": 20},{\"name\": \"Minjibir\",\"id\": 20},{\"name\": \"Nasarawa\",\"id\": 20},{\"name\": \"Rano\",\"id\": 20},{\"name\": \"Rimin Gado\",\"id\": 20},{\"name\": \"Rogo\",\"id\": 20},{\"name\": \"Shanono\",\"id\": 20},{\"name\": \"Sumaila\",\"id\": 20},{\"name\": \"Takai\",\"id\": 20},{\"name\": \"Tarauni\",\"id\": 20},{\"name\": \"Tofa\",\"id\": 20},{\"name\": \"Tsanyawa\",\"id\": 20},{\"name\": \"Tudun Wada\",\"id\": 20},{\"name\": \"Ungogo\",\"id\": 20},{\"name\": \"Warawa\",\"id\": 20},{\"name\": \"Wudil\",\"id\": 20},{\"name\": \"Bakori\",\"id\": 21},{\"name\": \"Batagarawa\",\"id\": 21},{\"name\": \"Batsari\",\"id\": 21},{\"name\": \"Baure\",\"id\": 21},{\"name\": \"Bindawa\",\"id\": 21},{\"name\": \"Charanchi\",\"id\": 21},{\"name\": \"Dandume\",\"id\": 21},{\"name\": \"Danja\",\"id\": 21},{\"name\": \"Dan Musa\",\"id\": 21},{\"name\": \"Daura\",\"id\": 21},{\"name\": \"Dutsi\",\"id\": 21},{\"name\": \"Dutsin Ma\",\"id\": 21},{\"name\": \"Faskari\",\"id\": 21},{\"name\": \"Funtua\",\"id\": 21},{\"name\": \"Ingawa\",\"id\": 21},{\"name\": \"Jibia\",\"id\": 21},{\"name\": \"Kafur\",\"id\": 21},{\"name\": \"Kaita\",\"id\": 21},{\"name\": \"Kankara\",\"id\": 21},{\"name\": \"Kankia\",\"id\": 21},{\"name\": \"Katsina\",\"id\": 21},{\"name\": \"Kurfi\",\"id\": 21},{\"name\": \"Kusada\",\"id\": 21},{\"name\": \"Mai'Adua\",\"id\": 21},{\"name\": \"Malumfashi\",\"id\": 21},{\"name\": \"Mani\",\"id\": 21},{\"name\": \"Mashi\",\"id\": 21},{\"name\": \"Matazu\",\"id\": 21},{\"name\": \"Musawa\",\"id\": 21},{\"name\": \"Rimi\",\"id\": 21},{\"name\": \"Sabuwa\",\"id\": 21},{\"name\": \"Safana\",\"id\": 21},{\"name\": \"Sandamu\",\"id\": 21},{\"name\": \"Zango\",\"id\": 21},{\"name\": \"Aleiro\",\"id\": 22},{\"name\": \"Arewa Dandi\",\"id\": 22},{\"name\": \"Argungu\",\"id\": 22},{\"name\": \"Augie\",\"id\": 22},{\"name\": \"Bagudo\",\"id\": 22},{\"name\": \"Birnin Kebbi\",\"id\": 22},{\"name\": \"Bunza\",\"id\": 22},{\"name\": \"Dandi\",\"id\": 22},{\"name\": \"Fakai\",\"id\": 22},{\"name\": \"Gwandu\",\"id\": 22},{\"name\": \"Jega\",\"id\": 22},{\"name\": \"Kalgo\",\"id\": 22},{\"name\": \"KokoBesse\",\"id\": 22},{\"name\": \"Maiyama\",\"id\": 22},{\"name\": \"Ngaski\",\"id\": 22},{\"name\": \"Sakaba\",\"id\": 22},{\"name\": \"Shanga\",\"id\": 22},{\"name\": \"Suru\",\"id\": 22},{\"name\": \"WasaguDanko\",\"id\": 22},{\"name\": \"Yauri\",\"id\": 22},{\"name\": \"Zuru\",\"id\": 22},{\"name\": \"Adavi\",\"id\": 23},{\"name\": \"Ajaokuta\",\"id\": 23},{\"name\": \"Ankpa\",\"id\": 23},{\"name\": \"Bassa\",\"id\": 23},{\"name\": \"Dekina\",\"id\": 23},{\"name\": \"Ibaji\",\"id\": 23},{\"name\": \"Idah\",\"id\": 23},{\"name\": \"Igalamela Odolu\",\"id\": 23},{\"name\": \"Ijumu\",\"id\": 23},{\"name\": \"KabbaBunu\",\"id\": 23},{\"name\": \"Kogi\",\"id\": 23},{\"name\": \"Lokoja\",\"id\": 23},{\"name\": \"Mopa Muro\",\"id\": 23},{\"name\": \"Ofu\",\"id\": 23},{\"name\": \"OgoriMagongo\",\"id\": 23},{\"name\": \"Okehi\",\"id\": 23},{\"name\": \"Okene\",\"id\": 23},{\"name\": \"Olamaboro\",\"id\": 23},{\"name\": \"Omala\",\"id\": 23},{\"name\": \"Yagba East\",\"id\": 23},{\"name\": \"Yagba West\",\"id\": 23},{\"name\": \"Asa\",\"id\": 24},{\"name\": \"Baruten\",\"id\": 24},{\"name\": \"Edu\",\"id\": 24},{\"name\": \"Ekiti\",\"id\": 24},{\"name\": \"Ifelodun\",\"id\": 24},{\"name\": \"Ilorin East\",\"id\": 24},{\"name\": \"Ilorin South\",\"id\": 24},{\"name\": \"Ilorin West\",\"id\": 24},{\"name\": \"Irepodun\",\"id\": 24},{\"name\": \"Isin\",\"id\": 24},{\"name\": \"Kaiama\",\"id\": 24},{\"name\": \"Moro\",\"id\": 24},{\"name\": \"Offa\",\"id\": 24},{\"name\": \"Oke Ero\",\"id\": 24},{\"name\": \"Oyun\",\"id\": 24},{\"name\": \"Pategi\",\"id\": 24},{\"name\": \"Agege\",\"id\": 1},{\"name\": \"Ajeromi-Ifelodun\",\"id\": 1},{\"name\": \"Alimosho\",\"id\": 1},{\"name\": \"Amuwo-Odofin\",\"id\": 1},{\"name\": \"Apapa\",\"id\": 1},{\"name\": \"Badagry\",\"id\": 1},{\"name\": \"Epe\",\"id\": 1},{\"name\": \"Eti Osa\",\"id\": 1},{\"name\": \"Ibeju-Lekki\",\"id\": 1},{\"name\": \"Ifako-Ijaiye\",\"id\": 1},{\"name\": \"Ikeja\",\"id\": 1},{\"name\": \"Ikorodu\",\"id\": 1},{\"name\": \"Kosofe\",\"id\": 1},{\"name\": \"Lagos Island\",\"id\": 1},{\"name\": \"Lagos Mainland\",\"id\": 1},{\"name\": \"Mushin\",\"id\": 1},{\"name\": \"Ojo\",\"id\": 1},{\"name\": \"Oshodi-Isolo\",\"id\": 1},{\"name\": \"Shomolu\",\"id\": 1},{\"name\": \"Surulere\",\"id\": 1},{\"name\": \"Abeokuta North\",\"id\": 27},{\"name\": \"Abeokuta South\",\"id\": 27},{\"name\": \"Ado-OdoOta\",\"id\": 27},{\"name\": \"Egbado North\",\"id\": 27},{\"name\": \"Egbado South\",\"id\": 27},{\"name\": \"Ewekoro\",\"id\": 27},{\"name\": \"Ifo\",\"id\": 27},{\"name\": \"Ijebu East\",\"id\": 27},{\"name\": \"Ijebu North\",\"id\": 27},{\"name\": \"Ijebu North East\",\"id\": 27},{\"name\": \"Ijebu Ode\",\"id\": 27},{\"name\": \"Ikenne\",\"id\": 27},{\"name\": \"Imeko Afon\",\"id\": 27},{\"name\": \"Ipokia\",\"id\": 27},{\"name\": \"Obafemi Owode\",\"id\": 27},{\"name\": \"Odeda\",\"id\": 27},{\"name\": \"Odogbolu\",\"id\": 27},{\"name\": \"Ogun Waterside\",\"id\": 27},{\"name\": \"Remo North\",\"id\": 27},{\"name\": \"Shagamu\",\"id\": 27},{\"name\": \"Akoko North-East\",\"id\": 28},{\"name\": \"Akoko North-West\",\"id\": 28},{\"name\": \"Akoko South-West\",\"id\": 28},{\"name\": \"Akoko South-East\",\"id\": 28},{\"name\": \"Akure North\",\"id\": 28},{\"name\": \"Akure South\",\"id\": 28},{\"name\": \"Ese Odo\",\"id\": 28},{\"name\": \"Idanre\",\"id\": 28},{\"name\": \"Ifedore\",\"id\": 28},{\"name\": \"Ilaje\",\"id\": 28},{\"name\": \"Ile OlujiOkeigbo\",\"id\": 28},{\"name\": \"Irele\",\"id\": 28},{\"name\": \"Odigbo\",\"id\": 28},{\"name\": \"Okitipupa\",\"id\": 28},{\"name\": \"Ondo East\",\"id\": 28},{\"name\": \"Ondo West\",\"id\": 28},{\"name\": \"Ose\",\"id\": 28},{\"name\": \"Owo\",\"id\": 28},{\"name\": \"Atakunmosa East\",\"id\": 29},{\"name\": \"Atakunmosa West\",\"id\": 29},{\"name\": \"Aiyedaade\",\"id\": 29},{\"name\": \"Aiyedire\",\"id\": 29},{\"name\": \"Boluwaduro\",\"id\": 29},{\"name\": \"Boripe\",\"id\": 29},{\"name\": \"Ede North\",\"id\": 29},{\"name\": \"Ede South\",\"id\": 29},{\"name\": \"Ife Central\",\"id\": 29},{\"name\": \"Ife East\",\"id\": 29},{\"name\": \"Ife North\",\"id\": 29},{\"name\": \"Ife South\",\"id\": 29},{\"name\": \"Egbedore\",\"id\": 29},{\"name\": \"Ejigbo\",\"id\": 29},{\"name\": \"Ifedayo\",\"id\": 29},{\"name\": \"Ifelodun\",\"id\": 29},{\"name\": \"Ila\",\"id\": 29},{\"name\": \"Ilesa East\",\"id\": 29},{\"name\": \"Ilesa West\",\"id\": 29},{\"name\": \"Irepodun\",\"id\": 29},{\"name\": \"Irewole\",\"id\": 29},{\"name\": \"Isokan\",\"id\": 29},{\"name\": \"Iwo\",\"id\": 29},{\"name\": \"Obokun\",\"id\": 29},{\"name\": \"Odo Otin\",\"id\": 29},{\"name\": \"Ola Oluwa\",\"id\": 29},{\"name\": \"Olorunda\",\"id\": 29},{\"name\": \"Oriade\",\"id\": 29},{\"name\": \"Orolu\",\"id\": 29},{\"name\": \"Osogbo\",\"id\": 29},{\"name\": \"Afijio\",\"id\": 30},{\"name\": \"Akinyele\",\"id\": 30},{\"name\": \"Atiba\",\"id\": 30},{\"name\": \"Atisbo\",\"id\": 30},{\"name\": \"Egbeda\",\"id\": 30},{\"name\": \"Ibadan North\",\"id\": 30},{\"name\": \"Ibadan North-East\",\"id\": 30},{\"name\": \"Ibadan North-West\",\"id\": 30},{\"name\": \"Ibadan South-East\",\"id\": 30},{\"name\": \"Ibadan South-West\",\"id\": 30},{\"name\": \"Ibarapa Central\",\"id\": 30},{\"name\": \"Ibarapa East\",\"id\": 30},{\"name\": \"Ibarapa North\",\"id\": 30},{\"name\": \"Ido\",\"id\": 30},{\"name\": \"Irepo\",\"id\": 30},{\"name\": \"Iseyin\",\"id\": 30},{\"name\": \"Itesiwaju\",\"id\": 30},{\"name\": \"Iwajowa\",\"id\": 30},{\"name\": \"Kajola\",\"id\": 30},{\"name\": \"Lagelu\",\"id\": 30},{\"name\": \"Ogbomosho North\",\"id\": 30},{\"name\": \"Ogbomosho South\",\"id\": 30},{\"name\": \"Ogo Oluwa\",\"id\": 30},{\"name\": \"Olorunsogo\",\"id\": 30},{\"name\": \"Oluyole\",\"id\": 30},{\"name\": \"Ona Ara\",\"id\": 30},{\"name\": \"Orelope\",\"id\": 30},{\"name\": \"Ori Ire\",\"id\": 30},{\"name\": \"Oyo\",\"id\": 30},{\"name\": \"Oyo East\",\"id\": 30},{\"name\": \"Saki East\",\"id\": 30},{\"name\": \"Saki West\",\"id\": 30},{\"name\": \"Surulere\",\"id\": 30},{\"name\": \"Bokkos\",\"id\": 31},{\"name\": \"Barkin Ladi\",\"id\": 31},{\"name\": \"Bassa\",\"id\": 31},{\"name\": \"Jos East\",\"id\": 31},{\"name\": \"Jos North\",\"id\": 31},{\"name\": \"Jos South\",\"id\": 31},{\"name\": \"Kanam\",\"id\": 31},{\"name\": \"Kanke\",\"id\": 31},{\"name\": \"Langtang South\",\"id\": 31},{\"name\": \"Langtang North\",\"id\": 31},{\"name\": \"Mangu\",\"id\": 31},{\"name\": \"Mikang\",\"id\": 31},{\"name\": \"Pankshin\",\"id\": 31},{\"name\": \"Qua'an Pan\",\"id\": 31},{\"name\": \"Riyom\",\"id\": 31},{\"name\": \"Shendam\",\"id\": 31},{\"name\": \"Wase\",\"id\": 31},{\"name\": \"AbuaOdual\",\"id\": 32},{\"name\": \"Ahoada East\",\"id\": 32},{\"name\": \"Ahoada West\",\"id\": 32},{\"name\": \"Akuku-Toru\",\"id\": 32},{\"name\": \"Andoni\",\"id\": 32},{\"name\": \"Asari-Toru\",\"id\": 32},{\"name\": \"Bonny\",\"id\": 32},{\"name\": \"Degema\",\"id\": 32},{\"name\": \"Eleme\",\"id\": 32},{\"name\": \"Emuoha\",\"id\": 32},{\"name\": \"Etche\",\"id\": 32},{\"name\": \"Gokana\",\"id\": 32},{\"name\": \"Ikwerre\",\"id\": 32},{\"name\": \"Khana\",\"id\": 32},{\"name\": \"ObioAkpor\",\"id\": 32},{\"name\": \"OgbaEgbemaNdoni\",\"id\": 32},{\"name\": \"OguBolo\",\"id\": 32},{\"name\": \"Okrika\",\"id\": 32},{\"name\": \"Omuma\",\"id\": 32},{\"name\": \"OpoboNkoro\",\"id\": 32},{\"name\": \"Oyigbo\",\"id\": 32},{\"name\": \"Port Harcourt\",\"id\": 32},{\"name\": \"Tai\",\"id\": 32},{\"name\": \"Binji\",\"id\": 33},{\"name\": \"Bodinga\",\"id\": 33},{\"name\": \"Dange Shuni\",\"id\": 33},{\"name\": \"Gada\",\"id\": 33},{\"name\": \"Goronyo\",\"id\": 33},{\"name\": \"Gudu\",\"id\": 33},{\"name\": \"Gwadabawa\",\"id\": 33},{\"name\": \"Illela\",\"id\": 33},{\"name\": \"Isa\",\"id\": 33},{\"name\": \"Kebbe\",\"id\": 33},{\"name\": \"Kware\",\"id\": 33},{\"name\": \"Rabah\",\"id\": 33},{\"name\": \"Sabon Birni\",\"id\": 33},{\"name\": \"Shagari\",\"id\": 33},{\"name\": \"Silame\",\"id\": 33},{\"name\": \"Sokoto North\",\"id\": 33},{\"name\": \"Sokoto South\",\"id\": 33},{\"name\": \"Tambuwal\",\"id\": 33},{\"name\": \"Tangaza\",\"id\": 33},{\"name\": \"Tureta\",\"id\": 33},{\"name\": \"Wamako\",\"id\": 33},{\"name\": \"Wurno\",\"id\": 33},{\"name\": \"Yabo\",\"id\": 33},{\"name\": \"Ardo Kola\",\"id\": 34},{\"name\": \"Bali\",\"id\": 34},{\"name\": \"Donga\",\"id\": 34},{\"name\": \"Gashaka\",\"id\": 34},{\"name\": \"Gassol\",\"id\": 34},{\"name\": \"Ibi\",\"id\": 34},{\"name\": \"Jalingo\",\"id\": 34},{\"name\": \"Karim Lamido\",\"id\": 34},{\"name\": \"Kumi\",\"id\": 34},{\"name\": \"Lau\",\"id\": 34},{\"name\": \"Sardauna\",\"id\": 34},{\"name\": \"Takum\",\"id\": 34},{\"name\": \"Ussa\",\"id\": 34},{\"name\": \"Wukari\",\"id\": 34},{\"name\": \"Yorro\",\"id\": 34},{\"name\": \"Zing\",\"id\": 34},{\"name\": \"Bade\",\"id\": 35},{\"name\": \"Bursari\",\"id\": 35},{\"name\": \"Damaturu\",\"id\": 35},{\"name\": \"Fika\",\"id\": 35},{\"name\": \"Fune\",\"id\": 35},{\"name\": \"Geidam\",\"id\": 35},{\"name\": \"Gujba\",\"id\": 35},{\"name\": \"Gulani\",\"id\": 35},{\"name\": \"Jakusko\",\"id\": 35},{\"name\": \"Karasuwa\",\"id\": 35},{\"name\": \"Machina\",\"id\": 35},{\"name\": \"Nangere\",\"id\": 35},{\"name\": \"Nguru\",\"id\": 35},{\"name\": \"Potiskum\",\"id\": 35},{\"name\": \"Tarmuwa\",\"id\": 35},{\"name\": \"Yunusari\",\"id\": 35},{\"name\": \"Yusufari\",\"id\": 35},{\"name\": \"Anka\",\"id\": 36},{\"name\": \"Bakura\",\"id\": 36},{\"name\": \"Birnin MagajiKiyaw\",\"id\": 36},{\"name\": \"Bukkuyum\",\"id\": 36},{\"name\": \"Bungudu\",\"id\": 36},{\"name\": \"Gummi\",\"id\": 36},{\"name\": \"Gusau\",\"id\": 36},{\"name\": \"Kaura Namoda\",\"id\": 36},{\"name\": \"Maradun\",\"id\": 36},{\"name\": \"Maru\",\"id\": 36},{\"name\": \"Shinkafi\",\"id\": 36},{\"name\": \"Talata Mafara\",\"id\": 36},{\"name\": \"Chafe\",\"id\": 36},{\"name\": \"Zurmi\",\"id\": 36}]";

            List<Dictionary<string, string>> lgaListObj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(lgaJson);

            _lgaSeeds.AddLgas(lgaListObj);
            

            return "LGA'S SEEDED!!!";
        }


        public string SeedCountries()
        {
            try
            {
                string countriesPath = $"{Util.GetAppRemotePath()}\\Countries.json";

                IEnumerable<dynamic> countries = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(System.IO.File.ReadAllText(countriesPath));
                _countrySeeds.AddCountries(countries);

                return "OK";
            }catch(System.Exception exception)
            {
                Logger.Error(exception, $"Exception in seedCountries. Exceptino message --- {exception.Message}");
                return "Could not seed countries.";
            }
        }


        public string SeedEthnicities()
        {
            try
            {
                string ethnicitiesPath = $"{Util.GetAppRemotePath()}\\EthnicGroups.json";

                IEnumerable<dynamic> ethnicities = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(System.IO.File.ReadAllText(ethnicitiesPath));
                _ethnicitySeeds.AddEthnicities(ethnicities);

                return "OK";
            }
            catch(Exception exception)
            {
                Logger.Error(exception, $"Exception in seedEthnicities. Exception message --- {exception.Message}");
                return "Could not seed ethnicities.";
            }
        }
    }
}