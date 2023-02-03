using System;
using System.Linq;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem;
using Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Utilities;
using Newtonsoft.Json;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.CoreServices.Contracts;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers
{
    public class PoliceOfficerSchedulerReportAJAXHandler : IPoliceOfficerSchedulerReportAJAXHandler
    {
        private readonly ICoreOfficersDataFromExternalSource _corePoliceOfficerDataFromExternalSource;

        public PoliceOfficerSchedulerReportAJAXHandler(ICoreOfficersDataFromExternalSource corePoliceOfficerDataFromExternalSource)
        {
            _corePoliceOfficerDataFromExternalSource = corePoliceOfficerDataFromExternalSource;
        }


        /// <summary>
        /// Here we deserialize the searchParams value, 
        /// check that the request Identifier matches the serialized value
        /// then check if the search params have their page offset cached, if no cache found we return false, else true.
        /// </summary>
        /// <param name="searchParamsToken"></param>
        /// <param name="requestIdentifier"></param>
        /// <returns>APIResponse</returns>
        public APIResponse CheckSearchParamsConstraints(string searchParamsToken, string requestIdentifier)
        {
            PoliceOfficerSearchParams searchParams = GetSearchParamsOBJ(searchParamsToken);
            if (!CheckForRequestMismatch(searchParams, requestIdentifier)) return new APIResponse { Error = true, ResponseObject = "Token mismatch. Invalid request." };
            //now that we have confirmed that the search params are the same
            //lets check if the page chunk exists
            return new APIResponse { ResponseObject = _corePoliceOfficerDataFromExternalSource.CheckIfChunkExists(requestIdentifier, searchParams.Take, searchParams.Skip) };
        }



        private bool CheckForRequestMismatch(PoliceOfficerSearchParams searchParams, string requestIdentifier)
        {
            return GenerateRequestIdentifierString(searchParams) != requestIdentifier;
        }

        public OfficersRequestResponseModel GetOfficers(int page)
        {
            try
            {
                int skip = (page == 1) ? 0 : (page-1) * 10;
                int take = 10;
                return new OfficersRequestResponseModel
                {
                    StateCode = "",
                    LGACode = "",
                    CommandCode = "",
                    RankCode = "",
                    GenderCode = "",
                    ServiceNumber = "",
                    Name = "",
                    IPPISNumber = "",
                    Page = page,
                    PageSize = 10,
                    TotalNumberOfOfficers = 37,
                    ReportRecords = new List<OfficersItems>
                    {
                        new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        }, new OfficersItems
                        {
                            Name = "John Mark",
                            IPPISNumber = "2748462847283",
                            ServiceNumber = "PSS_73823233",
                            PhoneNumber = "09099384938",
                            Gender = "Male",
                            GenderCode = "M",
                            RankName = "Corporal",
                            RankCode = "Corporal",
                            StateName = "Lagos",
                            StateCode = "Lagos",
                            CommandName = "SABO AREA 1 DIVISIONAL HQ",
                            CommandCode = "SABO",
                            LGAName = "Yaba",
                            LGACode = "Yaba",
                            DateOfBirth = "23/04/1990",
                            StateOfOrigin = "Kogi",
                            AccountNumber = "0007876765",
                            BankCode = "ACCESS"
                        },
                    }.Skip(skip).Take(take).ToList()
                };

            }catch(Exception) { throw; }
        }


        /// <summary>
        /// Get the request identifier for the given search params token
        /// </summary>
        /// <param name="searchParametersToken"></param>
        /// <returns>string</returns>
        public string GetRequestIdentifier(string searchParametersToken)
        {
            return GenerateRequestIdentifierString(GetSearchParamsOBJ(searchParametersToken));
        }


        /// <summary>
        /// Get the search param Obj
        /// </summary>
        /// <param name="searchParametersToken"></param>
        /// <returns></returns>
        private PoliceOfficerSearchParams GetSearchParamsOBJ(string searchParametersToken)
        {
            return JsonConvert.DeserializeObject<PoliceOfficerSearchParams>(Util.LetsDecrypt(searchParametersToken, AppSettingsConfigurations.EncryptionSecret));
        }


        /// <summary>
        /// Get the string value that identifies the search
        /// parameters
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>string</returns>
        private string GenerateRequestIdentifierString(PoliceOfficerSearchParams searchParams)
        {
            return Util.OneWaySHA512Hash($"{searchParams.SelectedCommand}-{searchParams.State}-{searchParams.LGA}-{searchParams.IPPISNo}-{searchParams.IdNumber}-{searchParams.Rank}-{searchParams.OfficerName}", AppSettingsConfigurations.EncryptionSecret);
        }


    }
}