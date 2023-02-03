using Orchard;
using System.Net.Http;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.RemoteClient.Contracts
{
    public interface IRemoteClient : IDependency
    {
        /// <summary>
        /// Get the json string from http request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="method"></param>
        /// <returns>string</returns>
        string SendRequest(RequestModel model, HttpMethod method, Dictionary<string, string> queryParameters);
    }
}
