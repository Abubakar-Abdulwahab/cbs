using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientServices.Settlement.Contract;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts;

namespace Parkway.CBS.ClientServices.Settlement
{
    public class Settlement : ISettlement
    {
        public IUoW UoW { get; set; }
        public ITransactionLogDAOManager TransactionLogDAO { get; set; }
        public ISettlementEngineDetailsDAOManager SettlementEngineDetailsDAO { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName + "_SessionFactory", "SettlementJob");
            }
        }


        /// <summary>
        /// Do settlement work
        /// </summary>
        /// <param name="tenantName"></param>
        public void DoSettlement(string tenantName)
        {
            try
            {
                TimeSpan interval = new TimeSpan(1, 0, 0);
                Thread.Sleep(interval);

                if (tenantName != "Nasarawa") { return; }

                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                //yesterday = new DateTime(2021, 6, 18).AddMilliseconds(-1);
                //120955

                DoNetPay(tenantName, yesterday);
                DoReadycash(tenantName, PaymentChannel.MOB, yesterday);
                DoReadycash(tenantName, PaymentChannel.AgencyBanking, yesterday);
                //PG
                DoNetPayPG(tenantName, yesterday);
                DoReadycashPG(tenantName, PaymentChannel.MOB, yesterday);
                DoReadycashPG(tenantName, PaymentChannel.AgencyBanking, yesterday);

                //do settlement for agric
                DoNetPayAG1(tenantName, yesterday);
                DoReadycashAG1(tenantName, PaymentChannel.MOB, yesterday);
                DoReadycashAG1(tenantName, PaymentChannel.AgencyBanking, yesterday);

                DoNetPayAG2(tenantName, yesterday);
                DoReadycashAG2(tenantName, PaymentChannel.MOB, yesterday);
                DoReadycashAG2(tenantName, PaymentChannel.AgencyBanking, yesterday);
                DoRemitaForCOA(yesterday);
                //school of health
                DoNetPaySOH(tenantName, yesterday);
                DoReadycashSOH(tenantName, PaymentChannel.MOB, yesterday);
                DoReadycashSOH(tenantName, PaymentChannel.AgencyBanking, yesterday);


                DoNetPayIDEC(yesterday);
                DoReadycashIDEC(PaymentChannel.MOB, yesterday);
                DoReadycashIDEC(PaymentChannel.AgencyBanking, yesterday);
                DoEbillsIDEC(yesterday);
                DoRemitaIDEC(yesterday);

                //
                DoNigerMinEdu(yesterday);

                ///
                DoNigPoly(yesterday);
                NigerInstitutionsFLAILSCOACOE(yesterday);
                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/497
                Do_NSUK_PRELIM(yesterday);
                DoSETTLEMENT_CONFIGURATION_DOKA_NANEM_TEAMWORK(yesterday);
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/394/
                DoSONAcceptanceFee(yesterday);
                //
                DoDibiamSettlements(yesterday);

                Niger_IBBUL_INNO_SON(yesterday);
                //
                DoSoftDualNetDigital(yesterday);

                //Do school of health minna
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/397/
                DoSchoolOfHealthMinna(yesterday);
                DoGeneralHospitalMinna(yesterday);
                DoNigerSettlements(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/466/
                DoMarketMGTBureauPausharSettlement(yesterday);
                //Modern market
                DoSingleSettlement("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "MMBM01", 60, 584, 60584, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "MMBM01", 60, 584, 60584, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MMBM01", 60, 584, 60584, yesterday);
                //Karu market
                DoSingleSettlement("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "MMBK01", 60, 585, 60585, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "MMBK01", 60, 585, 60585, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MMBK01", 60, 585, 60585, yesterday);
                //Neighborhood market
                DoSingleSettlement("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "MMBN02", 60, 583, 60583, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "MMBN02", 60, 583, 60583, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MMBN02", 60, 583, 60583, yesterday);
                //SOHT Accommodation Fee
                DoSingleSettlement("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "SOHT05", 45, 376, 45376, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "SOHT06", 45, 376, 45376, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "SOHT06", 45, 376, 45376, yesterday);
                //SOHT Admission Form
                DoNasarawaSOHT("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "SOHT03", yesterday);
                DoNasarawaSOHT("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "SOHT04", yesterday);
                DoNasarawaSOHT("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "SOHT04", yesterday);

                //MINISTRY OF TRADE, INDUSTRIES & INVESTMENT (MDA Id = 18)
                DoMinTradeIndustriesAndInvestment(yesterday);
                //BUREAU FOR ICT (MDA Id = 68)
                DoBUREAUForICT(yesterday);

                DoNasPoly(yesterday);

                //COLLEGE OF AGRIC – NON-PORTAL SERVICES (MDA Id = 10)
                DoCollAgr("Nasarawa", 10, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COA05", yesterday);
                DoCollAgr("Nasarawa", 10, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COA06", yesterday);
                DoCollAgr("Nasarawa", 10, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COA06", yesterday);

                //NSUK TRANSCRIPT UNDERGRADUATE (MDA Id = 7)
                //NASARAWA BRAODCASTING SERVICE (MDA Id = 32)
                DoNasAllMDA("Nasarawa", 32, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NBS01", yesterday);
                DoNasAllMDA("Nasarawa", 32, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NBS01", yesterday);
                DoNasAllMDA("Nasarawa", 32, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NBS01", yesterday);

                //NASARAWA STATE WATER BOARD (MDA Id = 35)
                DoNasAllMDA("Nasarawa", 35, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NWB01", yesterday);
                DoNasAllMDA("Nasarawa", 35, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NWB01", yesterday);
                DoNasAllMDA("Nasarawa", 35, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NWB01", yesterday);


                //CWG
                DoSingleSettlement("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "CWG01", 3, 86, 386, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "CWG01", 3, 86, 386, yesterday);
                DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "CWG01", 3, 86, 386, yesterday);
                //DASH
                DoDASH01("Nasarawa", 12, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "DASH01", yesterday);
                DoDASH01("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "DASH01", yesterday);
                DoDASH01("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "DASH01", yesterday);

                DoDASH02("Nasarawa", 12, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "DASH02", yesterday);
                DoDASH02("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "DASH02", yesterday);
                DoDASH02("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "DASH02", yesterday);

                DoDASH03("Nasarawa", 12, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "DASH03", yesterday);
                DoDASH03("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "DASH03", yesterday);
                DoDASH03("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "DASH03", yesterday);

                DoDASH04("Nasarawa", 12, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "DASH04", yesterday);
                DoDASH04("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "DASH04", yesterday);
                DoDASH04("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "DASH04", yesterday);

                DoDASH05("Nasarawa", 12, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "DASH05", yesterday);
                DoDASH05("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "DASH05", yesterday);
                DoDASH05("Nasarawa", 12, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "DASH05", yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/484/
                DoVREG(yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/484/
                Do_MARKET_MANAGEMENT_BUREAU(yesterday);

                //NUDB FISH
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/500/
                Do_NUDB_FISH(yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/493/
                Do_NUDB_EDU(yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/485/
                DoFrands(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/501/
                Do_New_NSUK(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/501/
                Do_Nasarawa_State_Pension_Bureau(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/501/
                Do_DEEM(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_MARARABA_GURKU_MEDICAL_CENTRE(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_AGBASHI(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_TOTO(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_UMAISHA(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_KEFFI(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_UKE(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_DOMA(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_AKWANGA(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_GARAKU(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_OBI(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_KEANA(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_PANDA(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_AWE(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_NASARAWA_EGGON(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_UDEGE(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_WAMBA(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/506/
                Do_HMB_Service_NASARAWA(yesterday);


                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/456/
                DoStampOsoft(yesterday);

                DoMinistryOfWorksHousingTransport(yesterday);
                //
                DoTSC(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/452/
                DoUmaishaSettlement(yesterday);

                DoCOE(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/483/
                DoNSUKGOWNSANDSAHUS(yesterday);

               
                //
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/279/
                DoMDAAndOneRev("Nasarawa", 7, 823, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NSUK05", yesterday);
                DoMDAAndOneRev("Nasarawa", 7, 823, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NSUK06", yesterday);
                DoMDAAndOneRev("Nasarawa", 7, 823, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NSUK06", yesterday);

                DoMinistryOfJustice(yesterday);
                //SOSAP
                DoSOSAP(yesterday);

                //DoTemple
                DoTemple(yesterday);

                //Do LGAs settlement
                DoKARUCellBoneSettlements(yesterday);
                DoKARUTenementRateZoneDSettlements(yesterday);
                DoNasarawaLGA406(yesterday);
                DoLGASettlements3(yesterday);//https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/377/
                DoLGAsSettlement(yesterday);
                DoLGASettlements2(yesterday);
                DoKOKOZUM(yesterday);
                DoJoy(yesterday);
                DoNSUKPUTME(yesterday);
                //ITEX
                DoITEX(yesterday);
                //
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/278/
                //REMITA

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/281/
                DoMDAAndOneRev("Nasarawa", 6, 825, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE19", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 825, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE20", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 825, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE20", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 826, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE31", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 826, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE32", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 826, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE32", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 827, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE31", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 827, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE32", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 827, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE32", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 828, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE34", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 828, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE33", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 828, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE33", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 829, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE34", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 829, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE33", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 829, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE33", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 830, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE36", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 830, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE35", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 830, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE35", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 831, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE36", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 831, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE35", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 831, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE35", yesterday);

                DoMDAAndOneRev("Nasarawa", 6, 832, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE38", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 832, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE37", yesterday);
                DoMDAAndOneRev("Nasarawa", 6, 832, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE37", yesterday);

                //
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/292
                DoCITISERVE(yesterday);

                DoNasarawaInvestment(yesterday);

                DoHighCourtOfJustice(yesterday);
                DoShariaCourtOfAppeal(yesterday);
                DoCustomaryCourt(yesterday);
                DoAreaCourt(yesterday);
                DoMagistrateCourt(yesterday);
                DoUpperAreaCourt(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/326/
                DoZenithPOS(yesterday);
                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/328
                DoFCMBPOS(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/336/
                DoSchoolOfNursing(yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/340
                DoRSTVLSettlement(yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/356/
                DoHBM(yesterday);
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/372/
                DoHBMDrugs(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/373/
                DoRemitaHBM(yesterday);

                //https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/370/
                DoRemita(yesterday);
                https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/380/
                DoRemitaNiger(yesterday);

                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/379/
                DoPausharPOS(yesterday);
                //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/386/
                DoSettlementRangeWithRevList("Nasarawa", 3, 1741, yesterday, "TECHVIBES0");
                DoAraAssociates(yesterday);

                DoNBSandNWB(yesterday);
                DoMMBandCWGRemitaSP(yesterday);

                DoNasarawa("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NAS01", yesterday);
                DoNasarawa("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NAS01", yesterday);
                DoNasarawa("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NAS01", yesterday);
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }

            }
        }

        private void Do_NUDB_FISH(DateTime date)
        {
            DoSettlementRangeWithRevList("Nasarawa", 51, 1847, date, "FISH0", "FISHR01");
        }

        private void Do_New_NSUK(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 7, "1867, 1868, 1869, 1870", date, "NSUKNEW03", "NSUKUTMER03");

            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 7, 1871, date, "NSUKNEW04", "NSUKUTMER04");
        }

        private void Do_Nasarawa_State_Pension_Bureau(DateTime date)
        {
            DoMDAOnlyWithAllChannelsNewCodes("Nasarawa", 117, date, "NAS0", "NASR01");
        }

        private void Do_DEEM(DateTime date)
        {
            DoSettlementRangeWithRevListNewCodes("Nasarawa", 3, "419, 1732", date, "DEEM0", "DEEMR01");
        }

        private void Do_HMB_Service_MARARABA_GURKU_MEDICAL_CENTRE(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 73, 1880, date, "HMBS01", "HMBSR01");
        }

        private void Do_HMB_Service_AGBASHI(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 74, 1873, date, "HMBS02", "HMBSR02");
        }

        private void Do_HMB_Service_TOTO(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 75, 1885, date, "HMBS03", "HMBSR03");
        }

        private void Do_HMB_Service_UMAISHA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 76, 1887, date, "HMBS04", "HMBSR04");
        }

        private void Do_HMB_Service_KEFFI(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 77, 1872, date, "HMBS05", "HMBSR05");
        }

        private void Do_HMB_Service_UKE(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 78, 1886, date, "HMBS06", "HMBSR06");
        }

        private void Do_HMB_Service_DOMA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 79, 1876, date, "HMBS07", "HMBSR07");
        }

        private void Do_HMB_Service_AKWANGA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 80, 1874, date, "HMBS08", "HMBSR08");
        }

        private void Do_HMB_Service_GARAKU(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 81, 1877, date, "HMBS09", "HMBSR09");
        }

        private void Do_HMB_Service_OBI(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 82, 1883, date, "HMBS10", "HMBSR10");
        }

        private void Do_HMB_Service_KEANA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 83, 1878, date, "HMBS11", "HMBSR11");
        }

        private void Do_HMB_Service_PANDA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 84, 1884, date, "HMBS12", "HMBSR12");
        }

        private void Do_HMB_Service_AWE(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 85, 1875, date, "HMBS13", "HMBSR13");
        }

        private void Do_HMB_Service_NASARAWA_EGGON(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 86, 1881, date, "HMBS14", "HMBSR14");
        }

        private void Do_HMB_Service_UDEGE(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 87, 1879, date, "HMBS15", "HMBSR15");
        }

        private void Do_HMB_Service_WAMBA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 88, 1888, date, "HMBS16", "HMBSR16");
        }

        private void Do_HMB_Service_NASARAWA(DateTime date)
        {
            DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes("Nasarawa", 89, 1882, date, "HMBS17", "HMBSR17");
        }


        private void DoSoftDualNetDigital(DateTime date)
        {
            DoSettlementRangeWithRevList("Nasarawa", 3, 1840, date, "SOFT0", "SOFTR01");
            DoSettlementRangeWithRevList("Nasarawa", 3, 1839, date, "DUALNET0", "DUALNETR01");
            DoSettlementRangeWithRevList("Nasarawa", 3, 1838, date, "DIGITAL0", "DIGITALR01");
        }


        //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/483/
        private void DoNSUKGOWNSANDSAHUS(DateTime date)
        {
            List<string> list = new List<string> { "1806, 1807, 1808", "1805, 1801, 1802, 1803, 1804" };

            //NASARAWA STATE UNIVERSITY (MDA Id = 7)
            //Revenue head – 1806, 1807, 1808
            DoMDAAndRevList("Nasarawa", 7, list[0], PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NASUK01", date);

            DoMDAAndRevList("Nasarawa", 7, list[0], PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NASUK02", date);

            DoMDAAndRevList("Nasarawa", 7, list[0], PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NASUK02", date);

            DoMDAAndRevListAllChannels("Nasarawa", 7, list[0], PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASUKR01", date);


            //NASARAWA STATE UNIVERSITY (MDA Id = 7)
            //Revenue head – 1805, 1801, 1802, 1803, 1804
            DoMDAAndRevList("Nasarawa", 7, list[1], PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NASUK03", date);

            DoMDAAndRevList("Nasarawa", 7, list[1], PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NASUK04", date);

            DoMDAAndRevList("Nasarawa", 7, list[1], PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NASUK04", date);

            DoMDAAndRevListAllChannels("Nasarawa", 7, list[1], PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASUKR03", date);

            //BOARD OF INTERNAL REVENUE (MDA Id = 3)
            //Revenue head – 1809
            DoSettlementRangeWithRevList("Nasarawa", 3, 1809, date, "SAHUS0", "SAHUSR01");


        }

        public void DoBUREAUForICT(DateTime date)
        {
            UDoSettlementRangeWithPaushar("Nasarawa", 68, date, "BICT0", "BICTR01");
        }

        public void DoMinTradeIndustriesAndInvestment(DateTime date)
        {
            UDoSettlementRangeWithPaushar("Nasarawa", 18, date, "TRADE0", "TRADER01");
        }

        private void UDoSettlementRangeWithPaushar(string tenant, int mdaId, DateTime date, string settlementCode, string remitaCode)
        {
            UDoSettlementRange(tenant, mdaId, date, settlementCode);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", settlementCode + "7", date);
            DoOnlyMDAAllChannels(tenant, mdaId, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoMarketMGTBureauPausharSettlement(DateTime date)
        {
            DoMDAAndOneRev("Nasarawa", 60, 583, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "MMBN07", date);
            DoMDAAndOneRev("Nasarawa", 60, 585, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "MMBK07", date);
            DoMDAAndOneRev("Nasarawa", 60, 584, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "MMBM07", date);
            //
            DoMDAAndRevList("Nasarawa", 60, "582, 580, 581", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "NAS07", date);
        }


        private void DoVREG(DateTime date)
        {
            DoOnlyMDA("VREG", 1, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "VREG01", date, "2WbdgALQx7D4gD1vhF2EFYfyDtwlwAHvPVGfG7IRpic=", "2PKpuJ67AY17Ga60GtDMwwOhNa4VVHTDU9SwfIaDQ1WOxITscuFF8+3OExqG");
            DoOnlyMDA("VREG", 1, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "VREG02", date, "2WbdgALQx7D4gD1vhF2EFYfyDtwlwAHvPVGfG7IRpic=", "2PKpuJ67AY17Ga60GtDMwwOhNa4VVHTDU9SwfIaDQ1WOxITscuFF8+3OExqG");
            DoOnlyMDA("VREG", 1, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "VREG02", date, "2WbdgALQx7D4gD1vhF2EFYfyDtwlwAHvPVGfG7IRpic=", "2PKpuJ67AY17Ga60GtDMwwOhNa4VVHTDU9SwfIaDQ1WOxITscuFF8+3OExqG");

            DoOnlyMDAAllChannels("VREG", 1, PaymentProvider.RemitaSingleProduct, "REMITA SP", "VREGR01", date, "2WbdgALQx7D4gD1vhF2EFYfyDtwlwAHvPVGfG7IRpic=", "2PKpuJ67AY17Ga60GtDMwwOhNa4VVHTDU9SwfIaDQ1WOxITscuFF8+3OExqG");
        }


        private void Do_MARKET_MANAGEMENT_BUREAU(DateTime date)
        {
            DoSettlementRangeWithRevList("Nasarawa", 60, 1810, date, "NAS0", "NASR01");
        }


        private void Do_NUDB_EDU(DateTime date)
        {
            DoSettlementRange("Nasarawa", 51, date, "NUDB0", "NUDBR01");

            DoSettlementRangeWithRevList("Nasarawa", 19, "1772, 1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 1137, 1135, 1134, 1132, 1131, 1130, 1106, 448, 449, 450, 180, 445, 446, 447, 181,433, 184, 179, 183", date, "EDU0");

            DoMDAAndRevList("Nasarawa", 19, "1772, 1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 1137, 1135, 1134, 1132, 1131, 1130, 1106, 448, 449, 450, 180, 445, 446, 447, 181,433, 184, 179, 183", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "EDU07", date);

            DoMDAAndRevListAllChannels("Nasarawa", 19, "1772, 1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 1137, 1135, 1134, 1132, 1131, 1130, 1106, 448, 449, 450, 180, 445, 446, 447, 181,433, 184, 179, 183", PaymentProvider.RemitaSingleProduct, "REMITA SP", "EDUR01", date);
        }


        private void DoFrands(DateTime date)
        {
            DoSettlementRangeWithRevList("Nasarawa", 3, 1812, date, "FRANDS0", "FRANDSR01");
        }

        private void DoStampOsoft(DateTime date)
        {
            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            DoSettlementRangeWithRevList("Nasarawa", 3, 1770, date, "STAMP0", "STAMPR01");

            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            DoSettlementRangeWithRevList("Nasarawa", 3, "1790, 1791, 1792, 1793", date, "OSOFTK0");
            DoMDAAndRevList("Nasarawa", 3, "1790, 1791, 1792, 1793", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "OSOFTK07", date);
            DoMDAAndRevListAllChannels("Nasarawa", 3, "1790, 1791, 1792, 1793", PaymentProvider.RemitaSingleProduct, "REMITA SP", "OSOFTKR01", date);

            //MINISTRY OF HEALTH (MDA Id = 22)
            DoMDAAndOneRev("Nasarawa", 22, 1794, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "SON01", date);
            DoMDAAndOneRev("Nasarawa", 22, 1794, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "SON01", date);
            DoMDAAndOneRev("Nasarawa", 22, 1794, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "SON01", date);
            DoMDAAndOneRevAllChannels("Nasarawa", 22, 1794, PaymentProvider.RemitaSingleProduct, "REMITA SP", "SONR01", date);

            //SCHOOL OF NURSING & MIDWIFERY, LAFIA (MDA Id = 46)
            DoMDAAndOneRevAllChannels("Nasarawa", 46, 380, PaymentProvider.RemitaSingleProduct, "REMITA SP", "SONR01", date);
        }

        private void DoUmaishaSettlement(DateTime date)
        {
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "UMAISHA01", date);
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "UMAISHA01", date);
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "UMAISHA01", date);
            //GTB
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "UMAISHA02", date);
            //UBA
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "UMAISHA03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "UMAISHA04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "UMAISHA05", date);
            //Paushar
            DoOnlyMDA("Nasarawa", 98, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "UMAISHA07", date);

            DoOnlyMDAAllChannels("Nasarawa", 98, PaymentProvider.RemitaSingleProduct, "REMITA SP", "UMAISHAR01", date);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoMinistryOfWorksHousingTransport(DateTime date)
        {
            //Revenue head – 1784
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL01", date);
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL01", date);
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL01", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL02", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL03", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL04", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1784, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL05", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1784, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR01", date);

            //Revenue head – 1785
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL06", date);
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL06", date);
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL06", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL07", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL08", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL09", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1785, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL10", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1785, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR06", date);

            //Revenue head – 1786
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL11", date);
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL11", date);
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL11", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL12", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL13", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL14", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1786, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL15", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1786, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR11", date);

            //Revenue head – 1787
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL16", date);
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL16", date);
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL16", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL17", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL18", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL19", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1787, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL20", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1787, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR16", date);

            //Revenue head – 1780
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL21", date);
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL21", date);
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL21", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL22", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL23", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL24", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1780, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL25", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1780, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR21", date);

            //Revenue head – 1781
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL26", date);
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL26", date);
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL26", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL27", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL28", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL29", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1781, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL30", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1781, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR26", date);

            //Revenue head – 1782
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL31", date);
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL31", date);
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL31", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL32", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL33", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL34", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1782, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL35", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1782, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR31", date);

            //Revenue head – 1783
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL36", date);
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL36", date);
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL36", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL37", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL38", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL39", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1783, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL40", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1783, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR36", date);

            //Revenue head – 1776
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL41", date);
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL41", date);
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL41", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL42", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL43", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL44", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1776, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL45", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1776, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR41", date);

            //Revenue head – 1777
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL46", date);
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL46", date);
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL46", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL47", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL48", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL49", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1777, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL50", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1777, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR46", date);

            //Revenue head – 1778
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL51", date);
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL51", date);
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL51", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL52", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL53", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL54", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1778, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL55", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1778, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR51", date);

            //Revenue head – 1779
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPL56", date);
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPL56", date);
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPL56", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPL57", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPL58", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPL59", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1779, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPL60", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1779, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLR56", date);

            //Revenue head – 1788
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NICE01", date);
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NICE01", date);
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NICE01", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NICE02", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NICE03", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NICE04", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 28, 1788, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NICE05", date);
            //remita
            DoMDAAndOneRevAllChannels("Nasarawa", 28, 1788, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NICER01", date);

        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/403/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoJoy(DateTime date)
        {
            DoSettlementRangeWithRevList("Nasarawa", 3, 1766, date, "ADM0", "ADMR01");
            DoSettlementRangeWithRevList("Nasarawa", 3, "1765, 1764", date, "JOY0");

            DoMDAAndRevList("Nasarawa", 3, "1765, 1764", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "JOY07", date);
            DoMDAAndRevListAllChannels("Nasarawa", 3, "1765, 1764", PaymentProvider.RemitaSingleProduct, "REMITA SP", "JOYR01", date);
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/407/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoGeneralHospitalMinna(DateTime yesterday)
        {
            DoMDAOnlyWith3ChannelsAndRemitaAllChannels("Niger", 48, yesterday, "GHMINNA0", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", "GHMINNAR01");
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/408/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoNSUKPUTME(DateTime yesterday)
        {
            DoMDAAndOneRev("Nasarawa", 7, 1768, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NSUKUTME01", yesterday);
            DoMDAAndOneRev("Nasarawa", 7, 1768, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NSUKUTME01", yesterday);
            DoMDAAndOneRev("Nasarawa", 7, 1768, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NSUKUTME01", yesterday);

            DoMDAAndOneRevAllChannels("Nasarawa", 7, 1768, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NSUKUTMER01", yesterday);
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/397/
        /// </summary>
        /// <param name="date"></param>
        private void DoSchoolOfHealthMinna(DateTime date)
        {
            string list2 = "31, 3599";
            DoMDAAndRevList("Niger", 6, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGSOHM01", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevList("Niger", 6, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGSOHM02", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevList("Niger", 6, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGSOHM02", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevListAllChannels("Niger", 6, list2, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGSOHM03", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            ///
            list2 = "3598, 3597, 3596, 28, 29, 30";
            DoMDAAndRevList("Niger", 6, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGSOHM04", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevList("Niger", 6, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGSOHM05", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevList("Niger", 6, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGSOHM05", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevListAllChannels("Niger", 6, list2, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGSOHM06", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.xyz/tfs/Parkway/CBS/_workitems/edit/497
        /// </summary>
        /// <param name="yesterday"></param>
        public void Do_NSUK_PRELIM(DateTime date)
        {
            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            DoSettlementRangeWithRevList("Nasarawa", 3, 1825, date, "DOKA0", "DOKAR01");

            //NASARAWA STATE UNIVERSITY, KEFFI (MDA Id = 7)
            DoMDAAndRevList("Nasarawa", 7, "1827, 1830, 1829, 1828", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NSUKNEW01", date);

            DoMDAAndRevList("Nasarawa", 7, "1827, 1830, 1829, 1828", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NSUKNEW01", date);

            DoMDAAndRevList("Nasarawa", 7, "1827, 1830, 1829, 1828", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NSUKNEW01", date);

            DoMDAAndRevListAllChannels("Nasarawa", 7, "1827, 1830, 1829, 1828", PaymentProvider.RemitaSingleProduct, "REMITA SP", "NSUKNEWR01", date);

            //NASARAWA STATE UNIVERSITY, KEFFI (MDA Id = 7)
            DoMDAAndOneRev("Nasarawa", 7, 1831, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NSUKNEW02", date);
            DoMDAAndOneRev("Nasarawa", 7, 1831, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NSUKNEW02", date);
            DoMDAAndOneRev("Nasarawa", 7, 1831, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NSUKNEW02", date);
            DoMDAAndOneRevAllChannels("Nasarawa", 7, 1831, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NSUKNEWR02", date);
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/495/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoSETTLEMENT_CONFIGURATION_DOKA_NANEM_TEAMWORK(DateTime yesterday)
        {
            DoSettlementRangeWithRevList("Nasarawa", 3, 1818, yesterday, "DOKA0", "DOKAR01");
            DoSettlementRangeWithRevList("Nasarawa", 3, 1821, yesterday, "NANEM0", "NANEMR01");
            DoSettlementRangeWithRevList("Nasarawa", 20, 1816, yesterday, "TEAMWORK0", "TEAMWORKR01");
        }

        private void DoDibiamSettlements(DateTime yesterday)
        {
            DoSettlementRangeWithRevList("Nasarawa", 3, 1761, yesterday, "DIBIAM0", "DIBIAMR01");
            DoSettlementRangeWithRevList("Nasarawa", 3, 1762, yesterday, "DIBIAM0", "DIBIAMR01");
            DoSettlementRangeWithRevList("Nasarawa", 3, 1763, yesterday, "DIBIAM0", "DIBIAMR01");
        }


        public void DoNasarawaLGA406(DateTime date)
        {
            UDoSettlementRange("Nasarawa", 92, date, "NASARAWA0");
            DoOnlyMDA("Nasarawa", 92, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "NASARAWA07", date);
            DoOnlyMDAAllChannels("Nasarawa", 92, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASARAWAR01", date);
        }


        public void DoKARUTenementRateZoneDSettlements(DateTime date)
        {
            DoSettlementRangeWithRevList("Nasarawa", 71, 1767, date, "KARUIBIAM0", "KARUIBIAMR01");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoKARUCellBoneSettlements(DateTime yesterday)
        {
            DoSettlementRangeWithRevList("Nasarawa", 71, 1759, yesterday, "KARUCELL0", "KARUCELLR01");
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/395/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoRemitaForCOA(DateTime yesterday)
        {
            DoMDAAndOneRevAllChannels("Nasarawa", 10, 98, PaymentProvider.RemitaSingleProduct, "REMITA SP", "COAR01", yesterday);
            DoMDAAndRevListAllChannels("Nasarawa", 10, "660, 103, 104", PaymentProvider.RemitaSingleProduct, "REMITA SP", "COAR03", yesterday);
            DoMDAAndRevListAllChannels("Nasarawa", 10, "677, 96, 684, 685, 686, 687, 688, 679, 680, 681, 682, 683, 678, 673, 674, 675, 676, 672, 666, 671, 667, 668, 669, 670, 665, 663, 664, 662, 105, 99, 95", PaymentProvider.RemitaSingleProduct, "REMITA SP", "COAR05", yesterday);
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/394/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoSONAcceptanceFee(DateTime yesterday)
        {
            Dictionary<int, int> dc = new Dictionary<int, int> { { 249, 3538 }, { 248, 3531 }, { 72 , 3525 }, { 3, 3522 } };

            foreach (var item in dc)
            {
                DoMDAAndOneRev("Nasarawa", item.Key, item.Value, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGSON04", yesterday);
                DoMDAAndOneRev("Nasarawa", item.Key, item.Value, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGSON05", yesterday);
                DoMDAAndOneRev("Nasarawa", item.Key, item.Value, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGSON05", yesterday);
                DoMDAAndOneRevAllChannels("Nasarawa", item.Key, item.Value, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGSON06", yesterday);
            }
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/393/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoMMBandCWGRemitaSP(DateTime yesterday)
        {
            DoMDAAndOneRevAllChannels("Nasarawa", 60, 583, PaymentProvider.RemitaSingleProduct, "REMITA SP", "MMBNR02", yesterday);
            DoMDAAndOneRevAllChannels("Nasarawa", 60, 585, PaymentProvider.RemitaSingleProduct, "REMITA SP", "MMBKR01", yesterday);
            DoMDAAndOneRevAllChannels("Nasarawa", 60, 584, PaymentProvider.RemitaSingleProduct, "REMITA SP", "MMBMR01", yesterday);

            DoMDAAndRevListAllChannels("Nasarawa", 60, "582, 580, 581", PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASR01", yesterday);

            DoMDAAndOneRevAllChannels("Nasarawa", 3, 86, PaymentProvider.RemitaSingleProduct, "REMITA SP", "CWGR01", yesterday);
        }



        /// <summary>
        /// do https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/392/
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoNBSandNWB(DateTime yesterday)
        {
            DoOnlyMDAAllChannels("Nasarawa", 35, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NWBR01", yesterday);
            DoOnlyMDAAllChannels("Nasarawa", 32, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NBSR01", yesterday);
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/473/
        /// </summary>
        /// <param name="date"></param>
        public void DoNigerMinEdu(DateTime date)
        {
            DoMDAOnlyWith3ChannelsAndRemitaAllChannels("Niger", 254, date, "NIGEDU0", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", "NIGEDUR01");
        }


        /// <summary>
        /// Do NIg poly
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoNigPoly(DateTime yesterday)
        {
            DoMDAOnlyWith3ChannelsAndRemitaAllChannels("Niger", 7, yesterday, "NIGPOLY0", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");
        }


        /// <summary>
        /// Ara associates
        /// </summary>
        /// <param name="yesterday"></param>
        private void DoAraAssociates(DateTime yesterday)
        {
            DoSettlementRangeWithRevList("Nasarawa", 3, 1744, yesterday, "ARA0", "ARAR01");
        }

        private void DoPausharPOS(DateTime yesterday)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>
            {
                { 90, "KARSHI07"},
                { 96, "PANDA07"},
                { 91, "KEFFI07"},
                { 95, "TOTO07"},
                { 97, "GADABUKE07"},
                { 105, "LAFIANORTH07"},
                { 106, "LAFIAEAST07"},
                { 93, "AKWANGA07"},
                { 102, "AKWANGAWEST07"},
                { 111, "GIZA07"},
                { 99, "FARINRUWA07"},
                { 100, "NEGGON07"},
                { 101, "AKUN07"},
                { 94, "KOKONA07"},
                { 113, "AGWADA07"},
                { 104, "AWE07"},
                { 114, "AZARA07"},
                { 103, "DOMA07"},
                { 110, "EKYE07"},
                { 107, "JENKWE07"},
                { 109, "DADDARE07"},
                { 72, "LAFIA07"},
                { 67, "AGIDI07"},
                { 66, "OBI07"},
                { 65, "WAMBA07"},
                { 64, "KEANA07"},
            };

            foreach (var item in dic)
            {
                DoOnlyMDA("Nasarawa", item.Key, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", item.Value, yesterday);
            }

            DoMDAAndRevList("Nasarawa", 71, "1738, 1304, 1303, 1305, 1302", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "KARUNYAKO07", yesterday);

            DoMDAAndOneRev("Nasarawa", 71, 1740, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "KARUALEWA07", yesterday);

            DoMDAAndOneRev("Nasarawa", 71, 1739, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "KARUALPAA07", yesterday);

            DoOnlyMDA("Nasarawa", 71, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "KARU07", yesterday);
            //paushar update
            List<int> mdas = new List<int>
            {
                20, 4, 8, 9, 15, 16, 17, 21, 22, 24, 28, 29, 33, 34, 38, 51, 52, 55, 61, 69, 70, 115, 2, 14, 13, 47, 11, 53, 54, 58
            };

            foreach (var item in mdas)
            {
                DoOnlyMDA("Nasarawa", item, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "NAS07", yesterday);
            }



            DoMDAAndRevList("Nasarawa", 3, "416, 1732, 572, 77, 940, 78, 945, 944, 936, 929, 930, 931, 926, 927, 928, 923, 924, 925, 847, 87, 84, 578, 549, 545, 420, 419, 418, 417, 88, 89, 82, 83, 85,79, 80, 81", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "NAS07", yesterday);

            DoMDAAndRevList("Nasarawa", 19, "1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 1137, 1135, 1134, 1132, 1131, 1130, 1106, 448, 449, 450, 180, 445, 446, 447, 181, 433, 184, 179, 183", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "NAS07", yesterday);

            DoMDAAndRevList("Nasarawa", 3, "1096, 1092, 1094, 1088, 1090, 1084, 1086, 1081, 1083, 1077, 1079, 1074, 1075, 1068, 1069, 1057", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "TEMPLEP01", yesterday);

            DoMDAAndRevList("Nasarawa", 3, "1097, 1093, 1095, 1089, 1091, 1085, 1087, 1080, 1082, 1076, 1078, 1072, 1073, 1070, 1071, 1056", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "TEMPLEP06", yesterday);

            DoMDAAndRevList("Nasarawa", 3, "1103, 1104, 1105, 1098, 1099, 1100, 1101, 1102, 1055", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "TEMPLEP11", yesterday);
            
            DoMDAAndOneRev("Nasarawa", 3, 1054, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "TEMPLEP16", yesterday);

            DoMDAAndRevList("Nasarawa", 3, "1041, 1042", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "SOSAP07", yesterday);

            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "RTVLS07", yesterday);

            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "KOKOZUM07", yesterday);

            DoMDAAndRevList("Nasarawa", 19, "441, 442, 443, 444, 437, 438, 439, 440, 435, 434, 433, 436, 182, 430, 431, 432", PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "MNP07", yesterday);

            DoMDAAndOneRev("Nasarawa", 22, 226, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", "MNP07", yesterday);

        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/381/
        /// </summary>
        /// <param name="date"></param>
        public void NigerInstitutionsFLAILSCOACOE(DateTime date)
        {
            Dictionary<int, string> list = new Dictionary<int, string>
            {
                { 8, "NIGFLAILAS0" },
                { 10, "NIGCOE0" },
            };

            foreach (var item in list)
            {
                DoMDAOnlyWith3ChannelsAndRemitaAllChannels("Niger", item.Key, date, item.Value, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");
            }


            DoMDAAndOneRev("Niger", 9, 60, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGCOA01", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRev("Niger", 9, 60, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGCOA02", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRev("Niger", 9, 60, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGCOA02", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRevAllChannels("Niger", 9, 60, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGCOA03", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");


            DoMDAAndOneRev("Niger", 9, 56, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGCOA04", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRev("Niger", 9, 56, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGCOA05", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRev("Niger", 9, 56, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGCOA05", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRevAllChannels("Niger", 9, 56, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGCOA06", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            //
            string list2 = "62, 63, 57, 58, 59, 61, 52, 53, 54, 55";
            DoMDAAndRevList("Niger", 9, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGCOA07", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevList("Niger", 9, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGCOA08", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevList("Niger", 9, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGCOA08", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            DoMDAAndRevListAllChannels("Niger", 9, list2, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGCOA09", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");
        }

        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/375/
        /// </summary>
        /// <param name="yesterday"></param>
        private void Niger_IBBUL_INNO_SON(DateTime date)
        {
            //NIGER STATE INSTITUTE OF INNOVATION (MDA Id = 11)
            //Revenue head – 76
            DoMDAAndOneRev("Niger", 11, 76, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGINNO04", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRev("Niger", 11, 76, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGINNO05", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRev("Niger", 11, 76, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGINNO05", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRevAllChannels("Niger", 11, 76, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGINNO06", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");

            //NIGER STATE INSTITUTE OF INNOVATION (MDA Id = 11)
            //Revenue head – 75
            DoMDAAndOneRev("Niger", 11, 75, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIGINNO07", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRev("Niger", 11, 75, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIGINNO08", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRev("Niger", 11, 75, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIGINNO08", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            DoMDAAndOneRevAllChannels("Niger", 11, 75, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NIGINNO09", date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");


            Dictionary<int, string> list = new Dictionary<int, string>
            {
                { 70, "NIGIBBUL0" },
                { 11, "NIGINNO0" },
                { 249, "NIGSON0" },
                { 248, "NIGSON0" },
                { 72, "NIGSON0" },
                { 3, "NIGSON0" },
                { 29, "IBBSH0" },
            };

            foreach (var item in list)
            {
                DoMDAOnlyWith3ChannelsAndRemitaAllChannels("Niger", item.Key, date, item.Value, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");
            }

            

        }


        private void DoMDAOnlyWith3ChannelsAndRemitaAllChannels(string tenant, int mdaId, DateTime date, string settlementCode, string code, string secret, string remitaCode = null)
        {
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1", date, code, secret);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "2", date, code, secret);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "2", date, code, secret);

            DoOnlyMDAAllChannels(tenant, mdaId, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode ?? settlementCode + "3", date, code, secret);
        }


        private void DoRemitaHBM(DateTime date)
        {
            Dictionary<string, string> revs = new Dictionary<string, string>
            {
                { "1107, 1109, 1111, 1112", "HMBR01" },
                { "1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123", "HMBR02" },
                { "1124, 1129, 1125, 1126, 1127, 1128, 1133, 1136", "HMBR03" },
                { "1142, 1146, 1149", "HMBR04" },
                { "1150, 1151, 1152, 1153", "HMBR05" },
                { "1154, 1155", "HMBR06" },
                { "1156, 1157", "HMBR07" },
                { "1158, 1159", "HMBR08" },
                { "1160, 1161, 1162", "HMBR09" },
                { "1163, 1164, 1165", "HMBR10" },
                { "1166, 1167, 1168", "HMBR011" },
                { "1169, 1170, 1171", "HMBR012" },
                { "1172, 1173, 1174", "HMBR013" },
                { "1715", "HMBR13" },
                { "1716", "HMBR14" },
                { "1717", "HMBR15" },
                { "1718", "HMBR16" },
                { "1719", "HMBR17" },
                { "1720", "HMBR18" },
                { "1721", "HMBR19" },
                { "1722", "HMBR20" },
                { "1723", "HMBR21" },
                { "1724", "HMBR22" },
                { "1725", "HMBR23" },
                { "1726", "HMBR24" },
                { "1727", "HMBR25" },
                { "1728", "HMBR26" },
                { "1729", "HMBR27" },
                { "1730", "HMBR28" },
                { "1731", "HMBR29" },
                { "769", "DASHR01" },
                { "770", "DASHR02" },
                { "766, 767, 768", "DASHR03" },
                { "772, 764, 765", "DASHR04" },
                { "771", "DASHR05" },
                { "1301", "KOKOZUMR01" }
            };

            foreach (var item in revs)
            {
                DoRevListAllChannels("Nasarawa", item.Key, PaymentProvider.RemitaSingleProduct, "REMITA SP", item.Value, date);
            }
        }


        public void DoRemitaNiger(DateTime date)
        {
            List<int> list = new List<int>
            {
                247, 216, 26, 71, 69, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 28, 27, 25, 24, 23, 22, 21, 20, 19, 18, 14, 13, 12, 1
            };
            string tenant = "Niger";
            string settlmentCode = "NIGR01";

            foreach (var item in list)
            {
                DoOnlyMDAAllChannels(tenant, item, PaymentProvider.RemitaSingleProduct, "REMITA SP", settlmentCode, date, "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv");
            }
        }


        //https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/383/
        private void DoRemita(DateTime date)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>
            {
                { 90, "KARSHI06" },
                { 96, "PANDA06" },
                { 91, "KEFFI06" },
                { 95, "TOTO06" },
                { 97, "GADABUKE06" },
                { 105, "LAFIANORTH06" },
                { 106, "LAFIAEAST06" },
                { 93, "AKWANGA06" },
                { 102, "AKWANGAWEST06" },
                { 111, "GIZA06" },
                { 99, "FARINRUWA06" },
                { 100, "NEGGON06" },
                { 101, "AKUN06" },
                { 94, "KOKONA06" },
                { 113, "AGWADA06" },
                { 104, "AWE06" },
                { 114, "AZARA06" },
                { 103, "DOMA06" },
                { 110, "EKYE06" },
                { 107, "JENKWE06" },
                { 109, "DADDARE06" },
                { 67, "AGIDI06" },
                { 66, "OBI06" },
                { 65, "WAMBA06" },
                { 64, "KEANA06" },
            };

            foreach (var item in dic)
            {
                DoOnlyMDAAllChannels("Nasarawa", item.Key, PaymentProvider.RemitaSingleProduct, "REMITA SP", item.Value, date);
            }

            List<int> mdas = new List<int> { 20, 2, 4, 8, 9, 14, 15, 16, 17, 21, 24, 28, 29,33, 34, 38, 51, 52, 55, 61, 69, 70, 115 };
            foreach (var item in mdas)
            {
                DoOnlyMDAAllChannels("Nasarawa", item, PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASR01", date);
            }

            DoMDAAndRevListAllChannels("Nasarawa", 3, "416, 1732, 572, 77, 940, 78, 945, 944, 936, 929, 930, 931, 926, 927, 928, 923, 924, 925, 847, 87, 84, 578, 549, 545, 420, 419, 418, 417, 88, 89, 82, 83, 85, 79, 80, 81", PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASR01", date);

            DoMDAAndRevListAllChannels("Nasarawa", 19, "1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 1137, 1135, 1134, 1132, 1131, 1130, 1106, 448, 449, 450, 180, 445, 446, 447, 181, 433, 184, 179, 183", PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASR01", date);

            DoMDAAndRevListAllChannels("Nasarawa", 22, "1735, 491, 492, 490, 489, 221, 225, 226, 220, 222, 223, 224", PaymentProvider.RemitaSingleProduct, "REMITA SP", "NASR01", date);

            DoMDAAndRevListAllChannels("Nasarawa", 3, "1096, 1092, 1094, 1088, 1090, 1084, 1086, 1081, 1083, 1077, 1079, 1074, 1075, 1068, 1069, 1057", PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLER01", date);

            DoMDAAndRevListAllChannels("Nasarawa", 3, "1097, 1093, 1095, 1089, 1091, 1085, 1087, 1080, 1082, 1076, 1078, 1072, 1073, 1070, 1071, 1056", PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLER06", date);

            DoMDAAndRevListAllChannels("Nasarawa", 3, "1103, 1104, 1105, 1098, 1099, 1100, 1101, 1102, 1055", PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLER11", date);

            DoMDAAndOneRevAllChannels("Nasarawa", 3, 1054, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TEMPLER16", date);

            DoMDAAndRevListAllChannels("Nasarawa", 3, "1041, 1042", PaymentProvider.RemitaSingleProduct, "REMITA SP", "SOSAPR01", date);

            DoMDAAndOneRevAllChannels("Nasarawa", 3, 938, PaymentProvider.RemitaSingleProduct, "REMITA SP", "RTVLSR01", date);

            DoMDAAndRevListAllChannels("Nasarawa", 19, "441, 442, 443, 444, 437, 438, 439, 440, 435, 434, 433, 436, 182, 430, 431, 432", PaymentProvider.RemitaSingleProduct, "REMITA SP", "MNPR01", date);

            DoMDAAndOneRevAllChannels("Nasarawa", 22, 226, PaymentProvider.RemitaSingleProduct, "REMITA SP", "MNPR01", date);
        }

        private void DoMinistryOfJustice(DateTime date)
        {
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "JUSTICE01", date);
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "JUSTICE01", date);
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "JUSTICE01", date);
            //GTB
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "JUSTICE02", date);
            //UBA
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "JUSTICE03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "JUSTICE04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", 23, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "JUSTICE05", date);
        }


        private void DoKOKOZUM(DateTime date)
        {
            //BOARD OF INTERNAL REVENUE SERVICE(MDA Id = 3)
            //Revenue head – 1301
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "KOKOZUM01", date);
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "KOKOZUM01", date);
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "KOKOZUM01", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "KOKOZUM02", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "KOKOZUM03", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "KOKOZUM04", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 3, 1301, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "KOKOZUM05", date);
        }


        /// <summary>
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/377/
        /// </summary>
        /// <param name="date"></param>
        private void DoLGASettlements3(DateTime date)
        {

            DoMDAAndRevListAllChannels("Nasarawa", 71, "1327, 1328, 1329, 1322, 1323, 1324, 1325, 1326, 1308, 1309, 1310, 1311, 1306, 1307, 1199, 1053, 1052, 1051, 1050, 1049, 1048, 1047, 1046, 1045, 1044, 1043, 1040, 1039, 1038, 1037, 1036, 1035, 1034, 1033, 1032, 1031, 1030, 1029, 1028, 1027, 1026, 1025, 1024, 1023, 1022, 1021, 1020, 1019, 1018, 1017, 1016, 1015, 1014, 1013, 1012, 1011, 1010, 1009, 1008, 1007, 1002, 1006, 1005, 1004, 946, 947, 948, 949, 950, 951, 952, 1003", PaymentProvider.RemitaSingleProduct, "REMITA SP", "KARU06", date);


            DoSettlementRangeWithRevList("Nasarawa", 71, "1738, 1304, 1303, 1305, 1302", date, "KARUNYAKO0");
            DoMDAAndRevListAllChannels("Nasarawa", 71, "1738, 1304, 1303, 1305, 1302", PaymentProvider.RemitaSingleProduct, "REMITA SP", "KARUNYAKOR01", date);

            DoSettlementRangeWithRevList("Nasarawa", 71, "1740", date, "KARUALEWA0");
            DoMDAAndRevListAllChannels("Nasarawa", 71, "1740", PaymentProvider.RemitaSingleProduct, "REMITA SP", "KARUALEWAR01", date);

            DoSettlementRangeWithRevList("Nasarawa", 71, "1739", date, "KARUALPAA0");
            DoMDAAndRevListAllChannels("Nasarawa", 71, "1739", PaymentProvider.RemitaSingleProduct, "REMITA SP", "KARUALPAAR01", date);

            DoOnlyMDAAllChannels("Nasarawa", 72, PaymentProvider.RemitaSingleProduct, "REMITA SP", "LAFIA06", date);
        }


        private void DoLGASettlements2(DateTime date)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>
            {
                { 90, "KARSHI0" },
                { 96, "PANDA0" },
                { 91, "KEFFI0" },
                { 95, "TOTO0" },
                { 97, "GADABUKE0" },
                { 105, "LAFIANORTH0" },
                { 106, "LAFIAEAST0" },
                { 93, "AKWANGA0" },
                { 102, "AKWANGAWEST0" },
                { 111, "GIZA0" },
                { 99, "FARINRUWA0" },
                { 100, "NEGGON0" },
                { 101, "AKUN0" },
                { 94, "KOKONA0" },
                { 113, "AGWADA0" },
                { 104, "AWE0" },
                { 114, "AZARA0" },
                { 103, "DOMA0" },
                { 110, "EKYE0" },
                { 107, "JENKWE0" },
                { 109, "DADDARE0" },
            };

            foreach (var item in dic)
            {
                UDoSettlementRange("Nasarawa", item.Key, date, item.Value);
            }
        }

        private void DoSettlementRange(string tenant, int mdaId, DateTime date, string settlementCode, string remitaCode)
        {
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1", date);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "1", date);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "1", date);
            //GTB
            DoOnlyMDA(tenant, mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", settlementCode + "2", date);
            //UBA
            DoOnlyMDA(tenant, mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", settlementCode + "3", date);
            //Zenith
            DoOnlyMDA(tenant, mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", settlementCode + "4", date);
            //FCMB
            DoOnlyMDA(tenant, mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", settlementCode + "5", date);

            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", settlementCode + "7", date);

            DoOnlyMDAAllChannels("Nasarawa", mdaId, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }


        private void DoSettlementRangeWithRevList(string tenant, int mdaId, int revId, DateTime date, string settlementCode, string remitaCode = "TECHVIBESR01")
        {
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1", date);
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "1", date);
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "1", date);
            //GTB
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", settlementCode + "2", date);
            //UBA
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", settlementCode + "3", date);
            //Zenith
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", settlementCode + "4", date);
            //FCMB
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", settlementCode + "5", date);

            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", settlementCode + "7", date);

            DoMDAAndOneRevAllChannels(tenant, mdaId, revId, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }

        private void DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes(string tenant, int mdaId, int revId, DateTime date, string settlementCode, string remitaCode = "TECHVIBESR01")
        {
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "W", date);
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode, date);
            DoMDAAndOneRev(tenant, mdaId, revId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode, date);

            DoMDAAndOneRevAllChannels(tenant, mdaId, revId, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }

        private void DoSettlementRangeWithRevListFor3ChannelsAndRemitaNewCodes(string tenant, int mdaId, string revList, DateTime date, string settlementCode, string remitaCode = "TECHVIBESR01")
        {
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "W", date);
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode, date);
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode, date);

            DoMDAAndRevListAllChannels(tenant, mdaId, revList, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }

        private void DoMDAOnlyWithAllChannelsNewCodes(string tenant, int mdaId, DateTime date, string settlementCode, string remitaCode = null)
        {
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1W", date);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "1", date);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "1", date);

            //GTB
            DoOnlyMDA(tenant, mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", settlementCode + "2", date);
            //UBA
            DoOnlyMDA(tenant, mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", settlementCode + "3", date);
            //Zenith
            DoOnlyMDA(tenant, mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", settlementCode + "4", date);
            //FCMB
            DoOnlyMDA(tenant, mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", settlementCode + "5", date);

            DoOnlyMDA(tenant, mdaId, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", settlementCode + "7", date);

            DoOnlyMDAAllChannels(tenant, mdaId, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }

        private void DoSettlementRangeWithRevListNewCodes(string tenant, int mdaId, string revList, DateTime date, string settlementCode, string remitaCode = null)
        {
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1W", date);
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "1", date);
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "1", date);
            //GTB
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.ITEX, PaymentChannel.POS, "POS", settlementCode + "2", date);
            //UBA
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", settlementCode + "3", date);
            //Zenith
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", settlementCode + "4", date);
            //FCMB
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", settlementCode + "5", date);

            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.PAUSHAR, PaymentChannel.POS, "POS", settlementCode + "7", date);

            DoMDAAndRevListAllChannels(tenant, mdaId, revList, PaymentProvider.RemitaSingleProduct, "REMITA SP", remitaCode, date);
        }


        private void DoSettlementRangeWithRevList(string tenant, int mdaId, string revList, DateTime date, string settlementCode)
        {
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1", date);
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "1", date);
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "1", date);
            //GTB
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.ITEX, PaymentChannel.POS, "POS", settlementCode + "2", date);
            //UBA
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", settlementCode + "3", date);
            //Zenith
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", settlementCode + "4", date);
            //FCMB
            DoMDAAndRevList(tenant, mdaId, revList, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", settlementCode + "5", date);
        }


        private void UDoSettlementRange(string tenant, int mdaId, DateTime date, string settlementCode)
        {
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", settlementCode + "1", date);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", settlementCode + "1", date);
            DoOnlyMDA(tenant, mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", settlementCode + "1", date);
            //GTB
            DoOnlyMDA(tenant, mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", settlementCode + "2", date);
            //UBA
            DoOnlyMDA(tenant, mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", settlementCode + "3", date);
            //Zenith
            DoOnlyMDA(tenant, mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", settlementCode + "4", date);
            //FCMB
            DoOnlyMDA(tenant, mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", settlementCode + "5", date);
        }


        private void DoLGAsSettlement(DateTime? date = null)
        {
            //LAFIA LGA(MDA Id = 72)
            //Revenue head – All revenue heads under this MDA
            int mdaId = 72;
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "LAFIA01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "LAFIA01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "LAFIA01", date);
            //GTB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "LAFIA02", date);
            //UBA
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "LAFIA03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "LAFIA04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "LAFIA05", date);

            //KARU LGA(MDA Id = 71)
            //Revenue head – All revenue heads under this MDA
            mdaId = 71;
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "KARU01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "KARU01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "KARU01", date);
            //GTB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "KARU02", date);
            //UBA
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "KARU03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "KARU04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "KARU05", date);

            //AGIDI DEVELOPMENT AREA COUNCIL(MDA Id = 67)
            //Revenue head – All revenue heads under this MDA
            mdaId = 67;
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "AGIDI01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "AGIDI01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "AGIDI01", date);
            //GTB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "AGIDI02", date);
            //UBA
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "AGIDI03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "AGIDI04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "AGIDI05", date);

            //OBI LOCAL GOVERNMENT(MDA Id = 66)
            //Revenue head – All revenue heads under this MDA
            mdaId = 66;
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "OBI01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "OBI01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "OBI01", date);
            //GTB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "OBI02", date);
            //UBA
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "OBI03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "OBI04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "OBI05", date);

            //WAMBA LOCAL GOVERNMENT COUNCIL(MDA Id = 65)
            //Revenue head – All revenue heads under this MDA
            mdaId = 65;
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "WAMBA01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "WAMBA01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "WAMBA01", date);
            //GTB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "WAMBA02", date);
            //UBA
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "WAMBA03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "WAMBA04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "WAMBA05", date);

            //KEANA LOCAL GOVERNMENT COUNCIL(MDA Id = 64)
            //Revenue head – All revenue heads under this MDA
            mdaId = 64;
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "KEANA01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "KEANA01", date);
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "KEANA01", date);
            //GTB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "KEANA02", date);
            //UBA
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "KEANA03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "KEANA04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", mdaId, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "KEANA05", date);


            //MINISTRY OF EDUCATION(MDA Id = 19)
            //Revenue head – 1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 
            //1137, 1135, 1134, 1132, 1131, 1130, 1106
            string list2 = "1108, 1110, 1148, 1147, 1145, 1144, 1143, 1141, 1140, 1139, 1138, 1137, 1135, 1134, 1132, 1131, 1130, 1106";
            mdaId = 19;

            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NAS01", date);

            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NAS01", date);

            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NAS01", date);


            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NAS02", date);

            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NAS03", date);

            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NAS04", date);

            DoMDAAndRevList("Nasarawa", mdaId, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NAS05", date);
        }



        private void DoTemple(DateTime? date = null)
        {
            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            //Revenue head – 1096, 1092, 1094, 1088, 1090, 1084, 1086, 1081, 1083, 1077, 1079, 
            //1074, 1075, 1068, 1069, 1057
            string list2 = "1096, 1092, 1094, 1088, 1090, 1084, 1086, 1081, 1083, 1077, 1079, 1074, 1075, 1068, 1069, 1057";

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPLE01", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPLE01", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPLE01", date);


            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPLE02", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPLE03", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPLE04", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPLE05", date);

            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            //Revenue head – 1097, 1093, 1095, 1089, 1091, 1085, 1087, 1080, 1082, 1076, 1078, 
            //1072, 1073, 1070, 1071, 1056
            list2 = "1097, 1093, 1095, 1089, 1091, 1085, 1087, 1080, 1082, 1076, 1078, 1072, 1073, 1070, 1071, 1056";

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPLE06", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPLE06", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPLE06", date);


            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPLE07", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPLE08", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPLE09", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPLE10", date);

            //BOARD OF INTERNAL REVENUE SERVICE(MDA Id = 3)
            //Revenue head – 1103, 1104, 1105, 1098, 1099, 1100, 1101, 1102, 1055
            list2 = "1103, 1104, 1105, 1098, 1099, 1100, 1101, 1102, 1055";

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPLE11", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPLE11", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPLE11", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPLE12", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPLE13", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPLE14", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPLE15", date);

            //BOARD OF INTERNAL REVENUE SERVICE(MDA Id = 3)
            //Revenue head – 1054
            list2 = "1054";

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TEMPLE16", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TEMPLE16", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TEMPLE16", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TEMPLE17", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TEMPLE18", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TEMPLE19", date);

            DoMDAAndRevList("Nasarawa", 3, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TEMPLE20", date);

            //MINISTRY OF EDUCATION(MDA Id = 19)
            //Revenue head – 441, 442, 443, 444, 437, 438, 439, 440, 435, 434, 433, 436, 182, 430, 
            //431, 432

            list2 = "441, 442, 443, 444, 437, 438, 439, 440, 435, 434, 433, 436, 182, 430, 431, 432";

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "MNP01", date);

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "MNP01", date);

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MNP01", date);

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "MNP02", date);

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "MNP03", date);

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "MNP04", date);

            DoMDAAndRevList("Nasarawa", 19, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "MNP05", date);

            //MINISTRY OF HEALTH(MDA Id = 22)
            //Revenue head – 226
            list2 = "226";

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "MNP01", date);

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "MNP01", date);

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MNP01", date);

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "MNP02", date);

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "MNP03", date);

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "MNP04", date);

            DoMDAAndRevList("Nasarawa", 22, list2, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "MNP05", date);
        }


        private void DoSOSAP(DateTime? date = null)
        {
            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            //Revenue head – 1041, 1042
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "SOSAP01", date);
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "SOSAP01", date);
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "SOSAP01", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "SOSAP02", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "SOSAP03", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "SOSAP04", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "SOSAP05", date);

            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "SOSAP01", date);
            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "SOSAP01", date);
            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "SOSAP01", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "SOSAP02", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "SOSAP03");
            //Zenith
            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "SOSAP04", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "SOSAP05", date);


            //NASARAWA STATE UNIVERSITY, KEFFI(MDA Id = 7)
            DoMDAAndOneRev("Nasarawa", 7, 48, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NSUK03", date);
            DoMDAAndOneRev("Nasarawa", 7, 48, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NSUK04", date);
            DoMDAAndOneRev("Nasarawa", 7, 48, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NSUK04", date);

            DoMDAAndOneRev("Nasarawa", 7, 728, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NSUK03", date);
            DoMDAAndOneRev("Nasarawa", 7, 728, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NSUK04", date);
            DoMDAAndOneRev("Nasarawa", 7, 728, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NSUK04", date);
        }

        private void DoRSTVLSettlement(DateTime? date = null)
        {
            //BUREAU FOR PUBLIC PROCUREMENT (MDA Id = 69)
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NAS01", date);
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NAS01", date);
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NAS01", date);
            //GTB
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NAS02", date);
            //UBA
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NAS03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NAS04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", 69, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NAS05", date);

            //BOARD OF INTERNAL REVENUE SERVICE (MDA Id = 3)
            //Revenue head – 938
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "RTVLS01", date);
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "RTVLS01", date);
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "RTVLS01", date);
            //GTB
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "RTVLS02", date);
            //UBA
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "RTVLS03", date);
            //Zenith
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "RTVLS04", date);
            //FCMB
            DoMDAAndOneRev("Nasarawa", 3, 938, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "RTVLS05", date);

            //DoNigerSettlements(DateTime? date = null)
        }


        /// <summary>
        /// Do settlement for Niger
        /// </summary>
        private void DoNigerSettlements(DateTime? date = null)
        {
            //has higher priority
            DoCHITHUB(26, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NIG01", date);
            DoCHITHUB(26, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NIG02", date);
            DoCHITHUB(26, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NIG02", date);

            //Niger
            //NIGER STATE BOARD OF INTERNAL REVENUE(MDA Id = 1)
            DoMDAAndOneRev("Niger", 1, 1, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "RTVLS01", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRev("Niger", 1, 1, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "RTVLS02", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);
            DoMDAAndOneRev("Niger", 1, 1, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "RTVLS02", "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=", "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv", date);

            //
            DoNetPayNiger(date);
            DoReadycashNiger(PaymentChannel.MOB, date);
            DoReadycashNiger(PaymentChannel.AgencyBanking, date);

            //
            DoNigAllMDA("Niger", 29, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "IBBSH01", date);
            DoNigAllMDA("Niger", 29, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "IBBSH02", date);
            DoNigAllMDA("Niger", 29, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "IBBSH03", date);
        }



        private void DoCHITHUB(int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork("Niger");
                string code = "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=";
                string secret = "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedForChitHub(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueForChitHub(mdaId, provider, channel, yesterday, 5);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNasarawaInvestment(DateTime? startDate = null)
        {
            DoSingleSettlement("Nasarawa", PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NAS01", 63, 833, 63833, startDate);
            DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NAS01", 63, 833, 63833, startDate);
            DoSingleSettlement("Nasarawa", PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NAS01", 63, 833, 63833, startDate);
        }


        private void DoNasPoly(DateTime? startDate = null)
        {
            //NASPOLY CONSULT OND (MDA Id = 5)
            Do5ONDNas("Nasarawa", 5, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NASPOLY03", startDate);
            Do5ONDNas("Nasarawa", 5, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NASPOLY04", startDate);
            Do5ONDNas("Nasarawa", 5, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NASPOLY04", startDate);

            //NASPOLY CONSULT HND (MDA Id = 5)
            Do5HNDNas("Nasarawa", 5, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NASPOLY05", startDate);
            Do5HNDNas("Nasarawa", 5, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NASPOLY06", startDate);
            Do5HNDNas("Nasarawa", 5, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NASPOLY06", startDate);

            //Naspoly new portal
            DoNasPolyNewPortal("Nasarawa", 5, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NASPOLY07", startDate);
            DoNasPolyNewPortal("Nasarawa", 5, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NASPOLY08", startDate);
            DoNasPolyNewPortal("Nasarawa", 5, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NASPOLY08", startDate);

            DoMDAAndOneRev("Nasarawa", 5, 824, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "NASPOLY01", startDate);
            DoMDAAndOneRev("Nasarawa", 5, 824, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "NASPOLY02", startDate);
            DoMDAAndOneRev("Nasarawa", 5, 824, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "NASPOLY02", startDate);

            DoNetPayNasPoly("Nasarawa", startDate);
            DoReadycashNasPoly("Nasarawa", PaymentChannel.MOB, startDate);
            DoReadycashNasPoly("Nasarawa", PaymentChannel.AgencyBanking, startDate);
        }


        private void DoHighCourtOfJustice(DateTime? startDate = null)
        {
            List<int> collection = new List<int> { 848, 849, 850, 851, 567, 562, 563, 564, 565, 566, 561, 556, 557, 558, 559, 560, 552, 553, 554, 555, 136, 137, 138, 139 };
            foreach (var item in collection)
            {
                DoMDAAndOneRevLessSettlementFee("Nasarawa", 13, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MOJ01", startDate);
            }
        }


        public void DoShariaCourtOfAppeal(DateTime? startDate = null)
        {
            List<int> collection = new List<int> { 392, 853 };
            foreach (var item in collection)
            {
                DoMDAAndOneRevLessSettlementFee("Nasarawa", 47, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MOJ01", startDate);
            }
        }


        public void DoCustomaryCourt(DateTime? startDate = null)
        {
            List<int> collection = new List<int> { 852, 134 };
            foreach (var item in collection)
            {
                DoMDAAndOneRevLessSettlementFee("Nasarawa", 11, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MOJ01", startDate);
            }
        }

        public void DoAreaCourt(DateTime? startDate = null)
        {
            List<int> collection = new List<int> { 421, 422 };
            foreach (var item in collection)
            {
                DoMDAAndOneRevLessSettlementFee("Nasarawa", 53, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MOJ01", startDate);
            }
        }

        public void DoMagistrateCourt(DateTime? startDate = null)
        {
            List<int> collection = new List<int> { 423, 424 };
            foreach (var item in collection)
            {
                DoMDAAndOneRevLessSettlementFee("Nasarawa", 54, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MOJ01", startDate);
            }
        }


        public void DoHBMDrugs(DateTime date)
        {

            Dictionary<int, int> dic = new Dictionary<int, int>
            {
                { 74,1715}, { 80,1716}, { 85,1717}, { 79,1718}, { 81,1719}, { 83,1720}, { 77,1721}, { 87,1722}, { 73,1723}, { 89,1724}, { 86,1725}, { 82,1726}, { 84,1727}, { 75,1728}, { 78,1729}, { 76,1730}, { 88,1731}
            };

            int i = 13;

            foreach (var item in dic)
            {
                DoSettlementWithDictionary(new Dictionary<int, int> { { item.Key, item.Value } }, "HMB" + i, date);
                i++;
            }            
        }

        public void DoHBM(DateTime date)
        {
            //Revenue head – 1107, 1109, 1111, 1112 
            //Id MDA_Id
            //1107    73
            //1109    74
            //1111    75
            //1112    76
            var mr = new Dictionary<int, int>
            {
                { 73, 1107 },
                { 74, 1109 },
                { 75, 1111 },
                { 76, 1112 },
            };
            DoSettlementWithDictionary(mr, "HMB01", date);

            //Id MDA_Id
            //1113    73
            //1114    77
            //1115    78
            //1116    75
            //1117    74
            //1118    76
            //Revenue head – 1113, 1114, 1115, 1116, 1117, 1118
            mr = new Dictionary<int, int>
            {
                { 73, 1113 },
                { 77, 1114 },
                { 78, 1115 },
                { 75, 1116 },
                { 74, 1117 },
                { 76, 1118 },
            };
            DoSettlementWithDictionary(mr, "HMB02", date);


            //Id MDA_Id
            //1119    75
            //1120    76
            //1121    73
            //1122    77
            //1123    78
            //Revenue head – 1119, 1120, 1121, 1122, 1123
            mr = new Dictionary<int, int>
            {
                { 75, 1119 },
                { 76, 1120 },
                { 73, 1121 },
                { 77, 1122 },
                { 78, 1123 },
            };
            DoSettlementWithDictionary(mr, "HMB02", date);



            //Revenue head – 1124, 1129, 1125, 1126, 1127, 1128, 1133, 1136
            //Id MDA_Id
            //1124    79
            //1125    80
            //1126    81
            //1127    82
            //1128    83
            //1129    77
            //1133    78
            //1136    84
            mr = new Dictionary<int, int>
            {
                { 79, 1124 },
                { 80, 1125 },
                { 81, 1126 },
                { 82, 1127 },
                { 83, 1128 },
                { 77, 1129 },
                { 78, 1133 },
                { 84, 1136 },
            };
            DoSettlementWithDictionary(mr, "HMB03", date);

            ////Id MDA_Id
            //1142    81
            //1146    83
            //1149    84
            //Revenue head – 1142, 1146, 1149
            mr = new Dictionary<int, int>
            {
                { 81, 1142 },
                { 83, 1146 },
                { 84, 1149 }
            };
            DoSettlementWithDictionary(mr, "HMB04", date);

            //Id MDA_Id
            //1150    79
            //1151    80
            //1152    84
            //1153    81
            //Revenue head – 1150, 1151, 1152, 1153 
            mr = new Dictionary<int, int>
            {
                { 79, 1150 },
                { 80, 1151 },
                { 84, 1152 },
                { 81, 1153 }
            };

            DoSettlementWithDictionary(mr, "HMB05", date);

            //Id MDA_Id
            //1154    85
            //1155    86
            //Revenue head – 1154, 1155
            mr = new Dictionary<int, int>
            {
                { 85, 1154 },
                { 86, 1155 }
            };
            DoSettlementWithDictionary(mr, "HMB06", date);

            //Id	MDA_Id
            //1156    85
            //1157    86
            //Revenue head – 1156, 1157
            mr = new Dictionary<int, int>
            {
                { 85, 1156 },
                { 86, 1157 }
            };
            DoSettlementWithDictionary(mr, "HMB07", date);

            //Id MDA_Id
            //1158    85
            //1159    86
            //Revenue head – 1158, 1159
            mr = new Dictionary<int, int>
            {
                { 85, 1158 },
                { 86, 1159 }
            };
            DoSettlementWithDictionary(mr, "HMB08", date);

            //Id	MDA_Id
            //1160    87
            //1161    88
            //1162    89
            //Revenue head – 1160, 1161, 1162
            mr = new Dictionary<int, int>
            {
                { 87, 1160 },
                { 88, 1161 },
                { 89, 1162 }
            };
            DoSettlementWithDictionary(mr, "HMB09", date);

            //Id	MDA_Id
            //1163    87
            //1164    89
            //1165    88
            //Revenue head – 1163, 1164, 1165 
            mr = new Dictionary<int, int>
            {
                { 87, 1163 },
                { 89, 1164 },
                { 88, 1165 }
            };
            DoSettlementWithDictionary(mr, "HMB10", date);

            //Id	MDA_Id
            //1166    87
            //1167    88
            //1168    89
            //Revenue head – 1166, 1167, 1168 
            mr = new Dictionary<int, int>
            {
                { 87, 1166 },
                { 88, 1167 },
                { 89, 1168 }
            };
            DoSettlementWithDictionary(mr, "HMB011", date);

            //Id	MDA_Id
            //1169    80
            //1170    82
            //1171    79
            //Revenue head – 1169, 1170, 1171
            mr = new Dictionary<int, int>
            {
                { 80, 1169 },
                { 82, 1170 },
                { 79, 1171 }
            };
            DoSettlementWithDictionary(mr, "HMB012", date);

            //Id	MDA_Id
            //1172    83
            //1173    74
            //1174    82
            mr = new Dictionary<int, int>
            {
                { 83, 1172 },
                { 74, 1173 },
                { 82, 1174 }
            };
            DoSettlementWithDictionary(mr, "HMB013", date);
        }



        private void DoSettlementWithDictionary(Dictionary<int, int> dic, string code, DateTime date)
        {
            foreach (var item in dic)
            {
                DoMDAAndOneRev("Nasarawa", item.Key, item.Value, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", code, date);
                DoMDAAndOneRev("Nasarawa", item.Key, item.Value, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", code, date);
                DoMDAAndOneRev("Nasarawa", item.Key, item.Value, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", code, date);
            }
        }


        public void DoUpperAreaCourt(DateTime? startDate = null)
        {
            List<int> collection = new List<int> { 550, 854 };
            foreach (var item in collection)
            {
                DoMDAAndOneRevLessSettlementFee("Nasarawa", 58, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "MOJ01", startDate);
            }
        }


        private void DoTSC(DateTime date)
        {
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "TSC01", date);
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "TSC01", date);
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "TSC01", date);
            //GTB
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "TSC02", date);
            //UBA
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "TSC03", date);
            //Zenith
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "TSC04", date);
            //FCMB
            DoOnlyMDA("Nasarawa", 48, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "TSC05", date);

            DoOnlyMDAAllChannels("Nasarawa", 48, PaymentProvider.RemitaSingleProduct, "REMITA SP", "TSCR01", date);
        }


        private void DoCOE(DateTime? startDate = null)
        {
            //college of education
            DoCEA128("Nasarawa", 6, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE19", startDate);
            DoCEA128("Nasarawa", 6, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE20", startDate);
            DoCEA128("Nasarawa", 6, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE20", startDate);

            List<int> coel0 = new List<int> { 773 };
            foreach (var item in coel0)
            {
                DoMDAAndOneRev("Nasarawa", 6, 773, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE01", startDate);
                DoMDAAndOneRev("Nasarawa", 6, 773, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE02", startDate);
                DoMDAAndOneRev("Nasarawa", 6, 773, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE02", startDate);
            }

            List<int> coel1 = new List<int> { 774, 775 };
            foreach (var item in coel1)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE03", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE04", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE04", startDate);
            }

            List<int> coel2 = new List<int> { 776, 777, 778, 779 };
            foreach (var item in coel2)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE05", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE06", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE06", startDate);
            }

            List<int> coel3 = new List<int> { 780, 781, 782, 783, 787, 786, 785, 784, 792, 791, 790, 789, 788, 793 };
            foreach (var item in coel3)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE07", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE08", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE08", startDate);
            }

            List<int> coel4 = new List<int> { 795, 794, 800, 799 };
            foreach (var item in coel4)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE09", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE10", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE10", startDate);
            }

            List<int> coel5 = new List<int> { 797, 796, 802, 801, 798, 803 };
            foreach (var item in coel5)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE11", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE12", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE12", startDate);
            }

            List<int> coel6 = new List<int> { 123, 124 };
            foreach (var item in coel6)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE13", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE14", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE14", startDate);
            }

            List<int> coel7 = new List<int> { 808, 806, 804, 810 };
            foreach (var item in coel7)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE15", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE16", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE16", startDate);
            }


            List<int> coel8 = new List<int> { 807, 805, 811, 809 };
            foreach (var item in coel8)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE17", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE18", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE18", startDate);
            }

            List<int> coel9 = new List<int> { 814 };
            foreach (var item in coel9)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE21", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE22", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE22", startDate);
            }

            List<int> coel10 = new List<int> { 815 };
            foreach (var item in coel10)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE23", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE24", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE24", startDate);
            }

            List<int> coel11 = new List<int> { 816 };
            foreach (var item in coel11)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE25", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE26", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE26", startDate);
            }

            List<int> coel12 = new List<int> { 812 };
            foreach (var item in coel12)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE27", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE28", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE28", startDate);
            }

            List<int> coel13 = new List<int> { 813 };
            foreach (var item in coel13)
            {
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "COE29", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "COE30", startDate);
                DoMDAAndOneRev("Nasarawa", 6, item, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "COE30", startDate);
            }
        }


        private void DoRemitaDASH()
        {
            //foreach (var item in new List<int> { 736, 737, 769 })
            //{
            //    DoMDAAndOneRevAllChannels("Nasarawa", 12, item, PaymentProvider.Remita, "ALL", "DASH06");
            //}

            //foreach (var item in new List<int> { 735, 734, 733, 738, 739, 740, 770 })
            //{
            //    DoMDAAndOneRevAllChannels("Nasarawa", 12, item, PaymentProvider.Remita, "ALL", "DASH07");
            //}

            //foreach (var item in new List<int> { 741, 135, 742, 743, 744, 766, 767, 768 })
            //{
            //    DoMDAAndOneRevAllChannels("Nasarawa", 12, item, PaymentProvider.Remita, "ALL", "DASH08");
            //}

            //foreach (var item in new List<int> { 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755, 756, 757, 758, 772, 764, 765 })
            //{
            //    DoMDAAndOneRevAllChannels("Nasarawa", 12, item, PaymentProvider.Remita, "ALL", "DASH09");
            //}

            //foreach (var item in new List<int> { 759, 760, 761, 762, 763, 771 })
            //{
            //    DoMDAAndOneRevAllChannels("Nasarawa", 12, item, PaymentProvider.Remita, "ALL", "DASH10");
            //}
        }


        private void DoZenithPOS(DateTime? startDate = null)
        {
            List<int> zenithitex = new List<int> { 62, 61, 24, 59, 58, 57, 44, 22, 25, 4, 8, 9, 15, 16, 17, 20, 21, 27, 30, 34, 40, 37, 56, 55, 54, 53, 52, 51, 50, 49, 47, 46, 43, 42, 41, 39, 38, 36, 33, 31, 29, 28, 26, 19, 14, 13, 11, 2, 3, 68 };

            foreach (int val in zenithitex)
            {
                if (val == 3)
                {
                    //All revenue heads except 3 and  86
                    DoMDAAndOneRev("Nasarawa", 3, 86, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "CWG04", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "SOSAP04", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "SOSAP04", startDate);
                }
                DoOnlyMDA("Nasarawa", val, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NAS04", startDate);
            }

            //Revenue heads – Only revenue head 86
            DoMDAAndOneRev("Nasarawa", 60, 580, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NAS04", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 581, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NAS04", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 582, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NAS04", startDate);


            DoOnlyMDA("Nasarawa", 35, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NWB04", startDate);
            DoOnlyMDA("Nasarawa", 32, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "NBS04", startDate);





            DoMDAAndOneRev("Nasarawa", 60, 583, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "MMBN05", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 585, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "MMBK04", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 584, PaymentProvider.ITEXZENITH, PaymentChannel.POS, "POS", "MMBM04", startDate);
        }


        /// <summary>
        /// School of nursing
        /// https://anywhere.parkwayprojects.com/tfs/Parkway/CBS/_workitems/edit/336/
        /// </summary>
        private void DoSchoolOfNursing(DateTime? startDate = null)
        {
            DoMDAAndOneRev("Nasarawa", 46, 380, PaymentProvider.Bank3D, PaymentChannel.Web, "WEB", "SON01", startDate);
            DoMDAAndOneRev("Nasarawa", 46, 380, PaymentProvider.Readycash, PaymentChannel.MOB, "BANK TRANSFER", "SON01", startDate);
            DoMDAAndOneRev("Nasarawa", 46, 380, PaymentProvider.Readycash, PaymentChannel.AgencyBanking, "AGENCY BANKING", "SON01", startDate);
        }


        private void DoFCMBPOS(DateTime? startDate = null)
        {
            List<int> fcmbitex = new List<int> { 62, 61, 24, 59, 58, 57, 44, 22, 25, 4, 8, 9, 15, 16, 17, 20, 21, 27, 30, 34, 40, 37, 56, 55, 54, 53, 52, 51, 50, 49, 47, 46, 43, 42, 41, 39, 38, 36, 33, 31, 29, 28, 26, 19, 14, 13, 11, 3, 2, 68 };

            foreach (int val in fcmbitex)
            {
                if (val == 3)
                {
                    //All revenue heads except 3 and  86
                    DoMDAAndOneRev("Nasarawa", 3, 86, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "CWG05", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "SOSAP05", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "SOSAP05", startDate);
                }
                DoOnlyMDA("Nasarawa", val, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NAS05", startDate);
            }

            DoOnlyMDA("Nasarawa", 35, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NWB05", startDate);
            DoOnlyMDA("Nasarawa", 32, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NBS05", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 583, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "MMBN06", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 585, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "MMBK05", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 584, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "MMBM05", startDate);

            //Revenue heads – Only revenue head 86
            DoMDAAndOneRev("Nasarawa", 60, 580, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NAS05", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 581, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NAS05", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 582, PaymentProvider.FCMBPOS, PaymentChannel.POS, "POS", "NAS05", startDate);
        }


        private void DoCITISERVE(DateTime? startDate = null)
        {
            List<int> itexl = new List<int> { 62, 61, 24, 59, 58, 57, 44, 22, 25, 4, 8, 9, 15, 16, 17, 20, 21, 27, 30, 34, 40, 37, 56, 55, 54, 53, 52, 51, 50, 49, 47, 46, 43, 42, 41, 39, 38, 36, 33, 31, 29, 28, 26, 19, 14, 13, 11, 2, 3, 68 };

            foreach (int val in itexl)
            {
                if (val == 3)
                {
                    //All revenue heads except 3 and  86
                    DoMDAAndOneRev("Nasarawa", 3, 86, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "CWG03", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "SOSAP03", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "SOSAP03", startDate);
                }
                DoOnlyMDA("Nasarawa", val, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NAS03", startDate);
            }

            //Revenue heads – Only revenue head 86
            DoMDAAndOneRev("Nasarawa", 60, 580, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NAS03", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 581, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NAS03", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 582, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NAS03", startDate);


            DoOnlyMDA("Nasarawa", 35, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NWB03", startDate);
            DoOnlyMDA("Nasarawa", 32, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "NBS03", startDate);





            DoMDAAndOneRev("Nasarawa", 60, 583, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "MMBN04", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 585, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "MMBK03", startDate);

            DoMDAAndOneRev("Nasarawa", 60, 584, PaymentProvider.CITISERVE, PaymentChannel.POS, "POS", "MMBM03", startDate);
        }


        private void DoITEX(DateTime? startDate = null)
        {
            List<int> itexl = new List<int> { 62, 61, 24, 59, 58, 57, 44, 22, 25, 4, 8, 9, 15, 16, 17, 20, 21, 27, 30, 34, 40, 37, 56, 55, 54, 53, 52, 51, 50, 49, 47, 46, 43, 42, 41, 39, 38, 36, 33, 31, 29, 28, 26, 19, 14, 13, 11, 2, 3, 68 };
            foreach (int val in itexl)
            {
                if (val == 3)
                {
                    //All revenue heads except 3 and  86
                    DoMDAAndOneRev("Nasarawa", 3, 86, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "CWG02", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1041, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "SOSAP02", startDate);
                    DoMDAAndOneRev("Nasarawa", 3, 1042, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "SOSAP02", startDate);
                }
                DoOnlyMDA("Nasarawa", val, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NAS02", startDate);
            }

            DoOnlyMDA("Nasarawa", 35, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NWB02", startDate);
            DoOnlyMDA("Nasarawa", 32, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NBS02", startDate);

            //Revenue heads – Only revenue head 86
            DoMDAAndOneRev("Nasarawa", 60, 580, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NAS02", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 581, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NAS02", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 582, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "NAS02", startDate);



            DoMDAAndOneRev("Nasarawa", 60, 583, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "MMBN03", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 585, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "MMBK02", startDate);
            DoMDAAndOneRev("Nasarawa", 60, 584, PaymentProvider.ITEX, PaymentChannel.POS, "POS", "MMBM02", startDate);
        }


        private void DoReadycashAG2(string tenantName, PaymentChannel channel, DateTime? predate = null)
        {
            int mdaId = 10;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";

                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAG2(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueAG2(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 8);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " COA04 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "COA04",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-8", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "COA04")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }


        private void DoReadycashAG1(string tenantName, PaymentChannel channel, DateTime? predate = null)
        {
            int mdaId = 10;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";

                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAG1(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueAG1(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 6);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " COA02 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "COA02",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-6", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "COA02")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }


        private void DoReadycashPG(string tenantName, PaymentChannel channel, DateTime? predate = null)
        {
            int mdaId = 7;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";

                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedPG(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTruePG(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 4);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NSUK04 PG " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "NSUK04",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-4", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "NSUK04")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("PG {0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }


        private void DoReadycashNiger(PaymentChannel channel, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork("Niger");
                string code = "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=";
                string secret = "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedForNiger(0, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueNiger(0, 0, PaymentProvider.Readycash, channel, yesterday, 2);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NIG02 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "NIG02",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-NIG02", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "NIG02")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", 0, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }


        private void DoReadycash(string tenantName, PaymentChannel channel, DateTime? predate = null)
        {
            //string configVal = ConfigurationManager.AppSettings["DoReadycash"];

            //if (string.IsNullOrEmpty(configVal))
            //{
            //    return;
            //}

            int mdaId = 7;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceived(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrue(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 2);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NSUK02 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "NSUK02",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-2", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "NSUK02")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }

        private void DoReadycashSOH(string tenantName, PaymentChannel channel, DateTime? predate = null)
        {
            int mdaId = 45;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";

                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedSOH(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueSOH(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 10);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " SOHT02 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "SOHT02",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-10", yesterday.ToString("dd/MM/yyyy"), mdaId, PaymentProvider.Readycash, channel, "658-659")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }


        public void DoReadycashNasPoly(string tenantName, PaymentChannel channel, DateTime? predate = null)
        {
            int mdaId = 5;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasPoly(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueNasPoly(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 12);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NASPOLY02 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "NASPOLY02",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-12", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "NASPOLY02")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }



        public void DoReadycashIDEC(PaymentChannel channel, DateTime? predate = null)
        {
            int mdaId = 0;

            try
            {
                SetUnitofWork("IDEC");
                string code = "UaWG84X/q44J1oZ+OdbxyDRDUhKfP36xuz5FI1GudZ0=";
                string secret = "OuZ6lZgL1DIwEj25d8IIaZ3hBxbYBWF2L08OHfu8HuZn//1OL0eKMoF6g9zA";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedIDEC(mdaId, 0, PaymentProvider.Readycash, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueIDEC(mdaId, 0, PaymentProvider.Readycash, channel, yesterday, 14);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " IDEC03 " + channel.ToDescription().ToUpper() + " SETTLEMENT",
                    RuleCode = "IDEC03",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-14", PaymentProvider.Readycash, channel, yesterday.ToString("dd/MM/yyyy"), "IDEC03")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", 0, 0, PaymentProvider.Readycash, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }

        private void DoRemitaIDEC(DateTime? predate = null)
        {
            int mdaId = 0;

            try
            {
                SetUnitofWork("IDEC");
                string code = "UaWG84X/q44J1oZ+OdbxyDRDUhKfP36xuz5FI1GudZ0=";
                string secret = "OuZ6lZgL1DIwEj25d8IIaZ3hBxbYBWF2L08OHfu8HuZn//1OL0eKMoF6g9zA";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedIDECNIBSS(mdaId, 0, PaymentProvider.RemitaSingleProduct, PaymentChannel.None, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueIDECNIBSS(mdaId, 0, PaymentProvider.RemitaSingleProduct, PaymentChannel.None, yesterday, 12);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " IDECR01 REMITA SP",
                    RuleCode = "IDECR01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("IDEC{0}-{1}-{2}-{3}-15", PaymentProvider.RemitaSingleProduct, PaymentChannel.None, yesterday.ToString("dd/MM/yyyy"), "IDECR01")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.RemitaSingleProduct, PaymentChannel.None, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }



        public void DoEbillsIDEC(DateTime? predate = null)
        {
            int mdaId = 0;

            try
            {
                SetUnitofWork("IDEC");
                string code = "UaWG84X/q44J1oZ+OdbxyDRDUhKfP36xuz5FI1GudZ0=";
                string secret = "OuZ6lZgL1DIwEj25d8IIaZ3hBxbYBWF2L08OHfu8HuZn//1OL0eKMoF6g9zA";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedIDECNIBSS(mdaId, 0, PaymentProvider.NIBSS, PaymentChannel.None, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];

                TransactionLogDAO.SetSettledTransactionsToTrueIDECNIBSS(mdaId, 0, PaymentProvider.NIBSS, PaymentChannel.None, yesterday, 12);

                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " IDEC02 BANK TRANSFER SETTLEMENT",
                    RuleCode = "IDEC02",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("IDEC{0}-{1}-{2}-{3}-15", PaymentProvider.NIBSS, PaymentChannel.None, yesterday.ToString("dd/MM/yyyy"), "IDEC02")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                string settlementResult = string.Empty;

                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.NIBSS, PaymentChannel.None, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;

            }
        }


        private void DoNetPayPG(string tenantName, DateTime? predate = null)
        {
            int mdaId = 7;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedPG(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTruePG(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 3);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NSUK03 PG WEB PAYMENT SETTLEMENT",
                    RuleCode = "NSUK03",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-3", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "NSUK03")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("PG {0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        private void DoNetPayAG1(string tenantName, DateTime? predate = null)
        {
            int mdaId = 10;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";

                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAG1(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueAG1(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 5);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " COA01 WEB PAYMENT SETTLEMENT",
                    RuleCode = "COA01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-5", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "COA01")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNetPayAG2(string tenantName, DateTime? predate = null)
        {
            int mdaId = 10;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";

                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAG2(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueAG2(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 7);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " COA03 WEB PAYMENT SETTLEMENT",
                    RuleCode = "COA03",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-7", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "COA03")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        private void DoNetPayNiger(DateTime? predate = null)
        {
            try
            {
                SetUnitofWork("Niger");
                string code = "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=";
                string secret = "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedForNiger(0, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueNiger(0, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 1);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NIG01 WEB PAYMENT SETTLEMENT",
                    RuleCode = "NIG01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-NIG", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "NIG01")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", 0, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNetPay(string tenantName, DateTime? predate = null)
        {
            int mdaId = 7;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceived(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrue(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 1);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NSUK01 WEB PAYMENT SETTLEMENT",
                    RuleCode = "NSUK01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-1", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "NSUK01")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel + "|" + retVal.QueryString,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        public void DoNetPaySOH(string tenantName, DateTime? predate = null)
        {
            int mdaId = 45;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedSOH(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueSOH(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 9);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " SOHT01 WEB PAYMENT SETTLEMENT",
                    RuleCode = "SOHT01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-9", yesterday.ToString("dd/MM/yyyy"), mdaId, PaymentProvider.Bank3D, PaymentChannel.Web, "658-659")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        public void DoNetPayNasPoly(string tenantName, DateTime? predate = null)
        {
            int mdaId = 5;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasPoly(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueNasPoly(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 11);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " NASPOLY01 WEB PAYMENT SETTLEMENT",
                    RuleCode = "NASPOLY01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-11", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "NASPOLY01")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNasarawaSOHT(string tenantName, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            int mdaId = 45;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedSOHT(mdaId, 0, provider, channel, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueSOHT(mdaId, 0, provider, channel, yesterday, 657659378);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId, 657659378)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 657659378, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNigAllMDA(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "dDfp+qS8BSIHRhwVzforUBevpSRisnaBG67XqI0AtI4=";
                string secret = "Qhlq0fP5qvCB7CKboK1BAYIDUh+lV70bEWFJk2NdXdpgyHm99T2+tcpd/bXv";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAllMDA(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueAllMDA(mdaId, provider, channel, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoDASH01(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedDASH01(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueDASH01(mdaId, provider, channel, yesterday, 736737);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoDASH02(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedDASH02(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueDASH02(mdaId, provider, channel, yesterday, 735734);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        private void DoDASH03(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedDASH03(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueDASH03(mdaId, provider, channel, yesterday, 741135);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoDASH04(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedDASH04(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueDASH04(mdaId, provider, channel, yesterday, 745746);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        public void DoMDAAndOneRevAllChannels(string tenantName, int mdaId, int revId, PaymentProvider provider, string narr, string scode, DateTime yesterday, string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=", string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP")
        {
            try
            {
                SetUnitofWork(tenantName);
                //string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                //string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //

                //DateTime now = DateTime.Now.ToLocalTime();
                //DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                //if (predate != null)
                //{
                //    yesterday = predate.Value;
                //}


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedMDAAndOneRevAllChannel(mdaId, revId, provider, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueMDAAndOneRevAllChannel(mdaId, revId, provider, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, "All", yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revId, provider, "ALL", yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        public void DoMDAAndOneRevLessSettlementFee(string tenantName, int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedMDAAndOneRevLessSettlementFee(mdaId, revId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueMDAAndOneRev(mdaId, revId, provider, channel, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revId, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }

        public void DoMDAAndOneRev(string tenantName, int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, string settlementId, string settlementSecret, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = settlementId;
                string secret = settlementSecret;
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedMDAAndOneRev(mdaId, revId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueMDAAndOneRev(mdaId, revId, provider, channel, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revId, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }

        private void DoRevListAllChannels(string tenantName, string revList, PaymentProvider provider, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedRevListAllChannels(revList, provider, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueRevListAllChannels(revList, provider, yesterday);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                string settlemntEngineRef = string.Format("{0}-{1}-{2}-{3}-{4}", provider, "ALL CHANNELS", yesterday.ToString("dd/MM/yyyy"), scode, revList);
                settlemntEngineRef = settlemntEngineRef.Length > 100 ? settlemntEngineRef.Substring(0, 100) : settlemntEngineRef;

                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = settlemntEngineRef
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr
                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}", revList.Length > 100 ? revList.Substring(0, 100) : revList, provider, "ALL CHANNELS", yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                    LongerParams = string.Format("{0}-{1}-{2}-{3}-{4}", "RevMDA", revList, provider, "ALL CHANNELS", yesterday)
                });
                UoW.Commit();
                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        public void DoMDAAndRevListAllChannels(string tenantName, int mdaId, string revList, PaymentProvider provider, string narr, string scode, DateTime? predate = null, string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=", string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP")
        {
            try
            {
                SetUnitofWork(tenantName);
                //string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                //string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedMDAAndRevList(mdaId, revList, provider, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueMDAAndRevList(mdaId, revList, provider, yesterday);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                string settlemntEngineRef = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, "All Channels", yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revList);
                settlemntEngineRef = settlemntEngineRef.Length > 100 ? settlemntEngineRef.Substring(0, 100) : settlemntEngineRef;

                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = settlemntEngineRef
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr
                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revList.Length > 100 ? revList.Substring(0, 100) : revList, provider, "All Channels", yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                    LongerParams = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revList, provider, "ALL CHANNELS", yesterday)
                });
                UoW.Commit();
                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }

        private void DoMDAAndRevList(string tenantName, int mdaId, string revList, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null, string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=", string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP")
        {
            try
            {
                SetUnitofWork(tenantName);
                //string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                //string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedMDAAndRevList(mdaId, revList, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueMDAAndRevList(mdaId, revList, provider, channel, yesterday);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                string settlemntEngineRef = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revList);
                settlemntEngineRef = settlemntEngineRef.Length > 100 ? settlemntEngineRef.Substring(0, 100) : settlemntEngineRef;

                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = settlemntEngineRef
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr
                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revList.Length > 100 ? revList.Substring(0, 100) : revList, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                    LongerParams = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revList, provider, channel, yesterday)
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }

        public void DoMDAAndOneRev(string tenantName, int mdaId, int revId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedMDAAndOneRev(mdaId, revId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueMDAAndOneRev(mdaId, revId, provider, channel, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revId, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }

        public void DoOnlyMDAAllChannels(string tenantName, int mdaId, PaymentProvider provider, string narr, string scode, DateTime predate, string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=", string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP")
        {
            try
            {
                SetUnitofWork(tenantName);
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = predate;
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAllMDA(mdaId, provider, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueAllMDA(mdaId, provider, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, "ALLCHANNELS", yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, "ALLCHANNELS", yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        public void DoOnlyMDA(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null, string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=", string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP")
        {
            try
            {
                SetUnitofWork(tenantName);
                //string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                //string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";

                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedAllMDA(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueAllMDA(mdaId, provider, channel, yesterday);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoCEA128(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }

                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedCEA128(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueCEA128(mdaId, provider, channel, yesterday, 128);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 128, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        private void DoDASH05(string tenantName, int mdaId, PaymentProvider provider, PaymentChannel channel, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedDASH05(mdaId, provider, channel, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueDASH05(mdaId, provider, channel, yesterday, 759760);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", provider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, provider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNasAllMDA(string tenantName, int mdaId, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasarawaAllMDA(mdaId, p, c, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawaAllMDA(mdaId, p, c, yesterday, mdaId);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", p, c, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoCollAgr(string tenantName, int mdaId, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasarawaCollAgr(mdaId, p, c, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawaCollAgr(mdaId, p, c, yesterday, mdaId);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", p, c, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoNasPolyNewPortal(string tenantName, int mdaId, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasPolyNew(mdaId, p, c, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueNasPolyNew(mdaId, p, c, yesterday, 20012021);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", p, c, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        private void Do5HNDNas(string tenantName, int mdaId, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasarawa5(mdaId, p, c, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawa5(mdaId, p, c, yesterday, 11115);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", p, c, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void Do5ONDNas(string tenantName, int mdaId, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasarawa15(mdaId, p, c, yesterday);

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawa15(mdaId, p, c, yesterday, mdaId);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}", p, c, yesterday.ToString("dd/MM/yyyy"), scode, mdaId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void Do18Nas(string tenantName, int mdaId, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasarawa18(mdaId, p, c, yesterday);
                //throw new Exception { };

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawa18(mdaId, p, c, yesterday, mdaId);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-18", p, c, yesterday.ToString("dd/MM/yyyy"), scode)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        private void DoNasarawa(string tenantName, PaymentProvider p, PaymentChannel c, string narr, string scode, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }


                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedNasarawa(0, 0, p, c, yesterday);
                dynamic retVal1 = TransactionLogDAO.GetTotalAmountReceivedNasarawaOnly(0, 0, p, c, yesterday);
                dynamic retVal2 = TransactionLogDAO.GetTotalAmountReceivedNasarawaEx(0, 0, p, c, yesterday);
                //throw new Exception { };
                //totalAmount += 
                //string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                decimal tots = retVal.TotalAmount + retVal1.TotalAmount + retVal2.TotalAmount;
                int cots = retVal.TotalCount + retVal1.TotalCount + retVal2.TotalCount;

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawa(0, 0, p, c, yesterday, 1504);

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawaOnly(0, 0, p, c, yesterday, 1504);

                TransactionLogDAO.SetSettledTransactionsToTrueNasarawaEx(0, 0, p, c, yesterday, 1504);

                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                //decimal tots = retVal.TotalAmount + retVal1.TotalAmount + retVal2.TotalAmount;
                //int cots = retVal.TotalCount + retVal1.TotalCount + retVal2.TotalCount;

                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = tots,
                    NumberOfTransactions = cots,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-1504", p, c, yesterday.ToString("dd/MM/yyyy"), scode)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", 0, 0, p, c, yesterday),
                    SettlementId = 1,
                    TransactionCount = cots,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private void DoSingleSettlement(string tenantName, PaymentProvider paymentProvider, PaymentChannel channel, string narr, string scode, int mdaId, int revId, int who, DateTime? predate = null)
        {
            try
            {
                SetUnitofWork(tenantName);
                string code = "AOcI22ifEZ2evEL0hZmmc+DMDnTfV1Cz+CShi9VXUeE=";
                string secret = "nTPzZkQ8NBdnnV5StgMsYB65q1Hrnt0xfdQiDId1WwqbkitzQNYLU3JRKIfP";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedGeneric(mdaId, revId, paymentProvider, channel, yesterday);
                TransactionLogDAO.SetSettledTransactionsToTrueGeneric(mdaId, revId, paymentProvider, channel, yesterday, who);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " " + scode + " " + narr + " PAYMENT SETTLEMENT ",
                    RuleCode = scode,//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", paymentProvider, channel, yesterday.ToString("dd/MM/yyyy"), scode, mdaId, revId)
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", mdaId, revId, paymentProvider, channel, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();
            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }



        public void DoNetPayIDEC(DateTime? predate = null)
        {
            int mdaId = 5;
            decimal totalAmount = 0.00m;

            try
            {
                SetUnitofWork("IDEC");
                string code = "UaWG84X/q44J1oZ+OdbxyDRDUhKfP36xuz5FI1GudZ0=";
                string secret = "OuZ6lZgL1DIwEj25d8IIaZ3hBxbYBWF2L08OHfu8HuZn//1OL0eKMoF6g9zA";
                //string code = "2RiME+l9n2l3DZwER2CjIOpP4Ao0qBH+y9uVzgwYwec=";
                //string secret = "d1gybibEOpgynTocSQtavSrLLlqh2Wqj8pvvi22P578=";
                string auth = "https://settlement.cbs.ng/settlementapi/api/v1/auth/gettoken";
                //string setl = "https://services.bank3d.ng/settlementapi/api/v1/rule/computewithtransactionnumber";
                string setl = "https://settlement.cbs.ng/settlementapi/api/v1/rule/compute";
                //string auth = "https://api.settlement.uat.bank3d.ng/api/v1/auth/gettoken";
                //string setl = "https://api.settlement.uat.bank3d.ng/api/v1/rule/computewithtransactionnumber";
                SetTransactionLogDAO();
                //
                DateTime now = DateTime.Now.ToLocalTime();
                DateTime yesterday = new DateTime(now.Year, now.Month, now.Day).AddMilliseconds(-1);
                if (predate != null)
                {
                    yesterday = predate.Value;
                }
                //
                dynamic retVal = TransactionLogDAO.GetTotalAmountReceivedIDEC(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday);
                //totalAmount += 
                string configValx = ConfigurationManager.AppSettings["DoWorkForJob"];
                TransactionLogDAO.SetSettledTransactionsToTrueIDEC(mdaId, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday, 13);
                //
                IClient _remoteClient = new Client();
                SEngineAuth se = new SEngineAuth { ClientCode = code, hmac = GetHmac(code, secret) };
                string model = JsonConvert.SerializeObject(se);
                string stoken = _remoteClient.SendRequest(model, auth, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                AuthToken authtoken = JsonConvert.DeserializeObject<AuthToken>(stoken);
                //
                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = retVal.TotalAmount,
                    NumberOfTransactions = retVal.TotalCount,
                    Narration = yesterday.ToString("dd/MM/yyyy") + " IDEC01 WEB PAYMENT SETTLEMENT",
                    RuleCode = "IDEC01",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    //RuleCode = "CBS001-Test",//"Web payment Settlement" + DateTime.Now.ToLocalTime().ToString(),CBS001-Test"
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-13", PaymentProvider.Bank3D, PaymentChannel.Web, yesterday.ToString("dd/MM/yyyy"), "IDEC01")
                };

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                //_apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue();
                string settlementResult = string.Empty;
                SettlementEngineResponse responseModel = null;
                //if (!string.IsNullOrEmpty(configValx))
                string errmsg = string.Empty;
                {
                    try
                    {
                        settlementResult = _remoteClient.SendRequest(ssttlmtmodel, setl, HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                        responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(settlementResult);
                    }
                    catch (Exception exception)
                    {
                        errmsg = exception.Message + exception.StackTrace;
                        responseModel = new SettlementEngineResponse { Error = true };
                    }

                }
                //else
                //{
                //    settlementResult = "";
                //    responseModel = new SettlementEngineResponse { };
                //}
                //desr

                SetSettlementEngineDetailsDAO();
                UoW.BeginTransaction();

                SettlementEngineDetailsDAO.Add(new Core.Models.SettlementEngineDetails
                {
                    Amount = sttlmtmodel.Amount,
                    Error = responseModel.Error,
                    TimeFired = yesterday,
                    SettlementEngineResponseJSON = settlementResult + " | " + errmsg,
                    Params = string.Format("{0}-{1}-{2}-{3}-{4}", 0, 0, PaymentProvider.Bank3D, PaymentChannel.Web, yesterday),
                    SettlementId = 1,
                    TransactionCount = retVal.TotalCount,
                    JSONModel = ssttlmtmodel,
                });
                UoW.Commit();

                //if (responseModel.Error) { throw new Exception(responseModel.ErrorMessage); }

            }
            catch (Exception exception)
            {
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                TransactionLogDAO = null;
                SettlementEngineDetailsDAO = null;
            }
        }


        private string GetHmac(string code, string secret)
        {
            return Core.Utilities.Util.HMACHash256(code, secret);
        }

        private void SetSettlementEngineDetailsDAO()
        {
            if (SettlementEngineDetailsDAO == null) { SettlementEngineDetailsDAO = new SettlementEngineDetailsDAOManager(UoW); }
        }

        private void SetTransactionLogDAO()
        {
            if (TransactionLogDAO == null) { TransactionLogDAO = new TransactionLogDAOManager(UoW); }
        }

    }
}
