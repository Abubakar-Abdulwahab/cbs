using System.Net.Http;
using System.Collections.Generic;

namespace Parkway.CBS.RemoteClient
{
    public interface IRemoteClient
    {
        /// <summary>
        /// Get the json string from http request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="method"></param>
        /// <returns>string</returns>
        string SendRequest(RequestModel model, HttpMethod method, Dictionary<string, string> queryParameters, bool isFormData = false);
    }
}