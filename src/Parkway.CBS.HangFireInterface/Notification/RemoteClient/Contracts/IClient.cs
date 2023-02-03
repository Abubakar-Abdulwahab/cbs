using System.Net.Http;
using System.Collections.Generic;

namespace Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts
{
    public interface IClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callBackURL"></param>
        /// <param name="method"></param>
        /// <param name="queryStringParameters"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        string SendRequest(string model, string callBackURL, HttpMethod method, Dictionary<string, string> queryStringParameters, Dictionary<string, string> headerParams);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callBackURL"></param>
        /// <param name="method"></param>
        /// <param name="queryStringParameters"></param>
        /// <param name="headerParams"></param>
        /// <returns></returns>
        string SendRequest(string model, string callBackURL, HttpMethod method, Dictionary<string, string> queryStringParameters);
    }
}
