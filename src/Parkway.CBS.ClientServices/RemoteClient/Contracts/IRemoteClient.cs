using System.Net.Http;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Entities.VMs;

namespace Parkway.CBS.ClientServices.RemoteClient.Contracts
{
    public interface IRemoteClient
    {
        /// <summary>
        /// Get the json string from http request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="method"></param>
        /// <returns>string</returns>
        string SendRequest(RemoteClientRequestModel model, HttpMethod method, Dictionary<string, string> queryParameters);
    }
}
