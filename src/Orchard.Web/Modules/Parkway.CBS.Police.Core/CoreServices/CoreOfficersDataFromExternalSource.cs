using Newtonsoft.Json;
using Orchard;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreOfficersDataFromExternalSource : ICoreOfficersDataFromExternalSource
    {
        private readonly IOfficersDataFromExternalSourceManager<OfficersDataFromExternalSource> _officersDataFromExternalSourceRepo;
        private readonly IOfficersDataFromExternalSourceStagingManager<OfficersDataFromExternalSourceStaging> _officersDataFromExternalSourceStagingRepo;
        private readonly IRemoteClient _remoteClient;
        private readonly IOrchardServices _orchardServices;


        public CoreOfficersDataFromExternalSource(IOfficersDataFromExternalSourceStagingManager<OfficersDataFromExternalSourceStaging> officersDataFromExternalSourceStagingRepo, IOfficersDataFromExternalSourceManager<OfficersDataFromExternalSource> officersDataFromExternalSourceRepo, IRemoteClient remoteClient, IOrchardServices orchardServices)
        {
            _officersDataFromExternalSourceRepo = officersDataFromExternalSourceRepo;
            _remoteClient = remoteClient;
            _orchardServices = orchardServices;
            _officersDataFromExternalSourceStagingRepo = officersDataFromExternalSourceStagingRepo;
        }


        /// <summary>
        /// We have the take and size,
        /// We need to check with the request Identifier if we have records for this particular
        /// chunk
        /// </summary>
        /// <param name="requestIdentifier"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public bool CheckIfChunkExists(string requestIdentifier, int take, int skip)
        {
            //chunk size is take
            //skip is offset
            //what is the range we are looking for in the SN
            dynamic startAndEndRange = GetSNRange(take, skip);
            return _officersDataFromExternalSourceRepo.GetCountWithinRange(startAndEndRange.Start, startAndEndRange.End) == take ? true : false;
        }



        public OfficersRequestResponseModel GetOfficersDataFromExternalSource(PoliceOfficerSearchParams searchParams, string requestIdentifier)
        {
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.ExternalDataSourceURL.ToString()).FirstOrDefault();

            RequestModel request = new RequestModel { URL = node.Value, Model = new OfficersRequestModel
            {
                StateCode = searchParams.StateCode,
                LGACode = searchParams.LGACode,
                CommandCode = searchParams.CommandCode,
                RankCode = searchParams.RankCode,
                GenderCode = searchParams.GenderCode,
                ServiceNumber = searchParams.ServiceNumber,
                Name = searchParams.OfficerName,
                IPPISNumber = searchParams.IPPISNo,
                Page = 1,
                PageSize = searchParams.Take

            } };
            var result = _remoteClient.SendRequest(request, HttpMethod.Post, new Dictionary<string, string> { });
            OfficersRequestResponseModel resultFromExternalSource = JsonConvert.DeserializeObject<OfficersRequestResponseModel>(result);
            //save the results 
            SaveExternalSourceDataIntoStaging(resultFromExternalSource, requestIdentifier);

            return resultFromExternalSource;
        }


        private void SaveExternalSourceDataIntoStaging(OfficersRequestResponseModel resultFromExternalSource, string requestIdentifier)
        {
            int counter = 0;
            string sourceref = $"{requestIdentifier}-{DateTime.Now.Ticks}";
            ICollection<OfficersDataFromExternalSourceStaging> dataModels = new List<OfficersDataFromExternalSourceStaging> { };
            foreach (var r in resultFromExternalSource.ReportRecords)
            {
                CheckIsNullOrEmpty(r.StateCode);
                CheckIsNullOrEmpty(r.StateName);
                CheckIsNullOrEmpty(r.AccountNumber);
                CheckIsNullOrEmpty(r.BankCode);
                CheckIsNullOrEmpty(r.CommandName);
                CheckIsNullOrEmpty(r.CommandCode);
                CheckIsNullOrEmpty(r.DateOfBirth);
                CheckIsNullOrEmpty(r.Gender);
                CheckIsNullOrEmpty(r.GenderCode);
                CheckIsNullOrEmpty(r.IPPISNumber);
                CheckIsNullOrEmpty(r.LGACode);
                CheckIsNullOrEmpty(r.LGAName);
                CheckIsNullOrEmpty(r.Name);
                CheckIsNullOrEmpty(r.PhoneNumber);
                CheckIsNullOrEmpty(r.RankCode);
                CheckIsNullOrEmpty(r.RankName);
                CheckIsNullOrEmpty(r.ServiceNumber);
                CheckIsNullOrEmpty(r.StateOfOrigin);
            }

            _officersDataFromExternalSourceStagingRepo.SaveBundleUnCommitStateless(dataModels);
        }


        private void CheckIsNullOrEmpty(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue)) { throw new DirtyFormDataException(); }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the start and end SN for the given take and skip param
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public dynamic GetSNRange(int take, int skip)
        {
            return new { Start = ((skip * take) + 1), End = skip + take };
        }


    }
}