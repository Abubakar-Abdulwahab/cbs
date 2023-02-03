using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Parkway.CBS.HangFireInterface.Notification.RemoteClient
{
    public class Client : IClient
    {
        [ProlongExpirationTime]
        public string SendRequest(string model, string callBackURL, HttpMethod method, Dictionary<string, string> queryStringParameters)
        {
            Dictionary<string, string> headerParams = new Dictionary<string, string> { };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpRequestMessage request = BuildRequest(model, callBackURL, method, queryStringParameters);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SetAuthorizationHeaderValues(request, headerParams);
            string uj = null;
            if (headerParams != null && headerParams.TryGetValue("Bearer", out uj))
            {
                return ExceuteRequest(request, uj);
            }
            return ExceuteRequest(request);
        }


        [ProlongExpirationTime]
        public string SendRequest(string model, string callBackURL, HttpMethod method, Dictionary<string, string> queryStringParameters, Dictionary<string, string> headerParams)
        {
            ILogger log = new Log4netLogger();
            log.Info($"About to fire request for URL {callBackURL} . Dump= > {model}");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpRequestMessage request = BuildRequest(model, callBackURL, method, queryStringParameters);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SetAuthorizationHeaderValues(request, headerParams);
            string uj = null;
            if(headerParams != null && headerParams.TryGetValue("Bearer", out uj))
            {
                return ExceuteRequest(request, uj);
            }
            return ExceuteRequest(request);
        }


        /// <summary>
        /// Make request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        private string ExceuteRequest(HttpRequestMessage request, string token = null)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error occurred while sending request. Error::: {response.Content.ReadAsStringAsync().Result}");
                    }

                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            #region catch clauses
            catch (HttpRequestException exception)
            {
                throw new Exception(exception.InnerException.Message);
            }
            catch (AggregateException exception)
            {
                throw new Exception(exception.InnerExceptions.First().InnerException.Message);
            }
            catch (Exception exception)
            {
                throw new Exception($"{exception}");
            }
            #endregion
        }

        private HttpRequestMessage BuildRequest(string model, string callBackURL, HttpMethod method, Dictionary<string, string> queryStringParameters)
        {
            try
            {
                UriBuilder uri = new UriBuilder(callBackURL);
                var query = HttpUtility.ParseQueryString(uri.Query);
                foreach (var item in queryStringParameters) { query[item.Key] = item.Value; }
                uri.Query = query.ToString();
                HttpRequestMessage request = new HttpRequestMessage(method, uri.ToString());
                if (method != HttpMethod.Post) { return request; }
                request.Content = new StringContent(model, Encoding.UTF8, "application/json");
                return request;
            }
            catch (Exception exception)
            {
                throw new Exception($"Error in BuildRequest {model}. URI {callBackURL} Exception {exception}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headerParams"></param>
        private static void SetAuthorizationHeaderValues(HttpRequestMessage request, Dictionary<string, string> headerParams)
        {
            if (headerParams == null) { return; }
            foreach (var headerItem in headerParams)
            {
                request.Headers.Add(headerItem.Key, headerItem.Value.ToString());
            }
        }
    }
}
