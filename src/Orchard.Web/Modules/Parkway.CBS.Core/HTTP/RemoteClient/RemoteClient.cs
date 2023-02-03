using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Parkway.CBS.Core.HTTP.RemoteClient
{
    public class RemoteClient : IRemoteClient
    {
        //private static readonly Lazy<HttpClientHandler> sharedHandler = new Lazy<HttpClientHandler>(() => new HttpClientHandler());
        public ILogger Logger { get; set; }

        public RemoteClient()
        {
            Logger = NullLogger.Instance;
        }

        public string SendRequest(RequestModel model, HttpMethod method, Dictionary<string, string> queryStringParameters)
        {
            HttpRequestMessage request = BuildRequest(model, method, queryStringParameters);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SetAuthorizationHeaderValues(request, model);
            return ExceuteRequest(request);
        }

        /// <summary>
        /// Make request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        private string ExceuteRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.Error(string.Format("Error making {0}. Response: {1}", request.RequestUri, response.Content.ReadAsStringAsync().Result));
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                    Logger.Information(string.Format("Request for {0} sent", request.RequestUri));
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            #region catch clauses
            catch (HttpRequestException exception)
            {
                Logger.Error(exception, string.Format("HttpRequestException in ExceuteRequest {0}", JsonConvert.SerializeObject(request)));
                throw new Exception(exception.InnerException.Message);
            }
            catch (AggregateException exception)
            {
                Logger.Error(exception, string.Format("AggregateException in ExceuteRequest {0}", JsonConvert.SerializeObject(request)));
                throw new Exception(exception.InnerExceptions.First().InnerException.Message);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ExceuteRequest {0}", JsonConvert.SerializeObject(request)));
                throw new Exception(exception.Message);
            }
            #endregion
        }

        private HttpRequestMessage BuildRequest(RequestModel model, HttpMethod method, Dictionary<string, string> queryStringParameters)
        {
            try
            {
                UriBuilder uri = new UriBuilder(model.URL);
                var query = HttpUtility.ParseQueryString(uri.Query);
                foreach (var item in queryStringParameters) { query[item.Key] = item.Value; }
                uri.Query = query.ToString();
                HttpRequestMessage request = new HttpRequestMessage(method, uri.ToString());
                if (method != HttpMethod.Post) { return request; }
                request.Content = new StringContent(JsonConvert.SerializeObject(model.Model), Encoding.UTF8, "application/json");
                return request;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error in BuildRequest {0}. URI {1}", JsonConvert.SerializeObject(model), model.URL));
                throw;
            }
        }

        /// <summary>
        /// Set authorization headers
        /// </summary>
        /// <param name="request">HttpRequestMessage</param>
        /// <param name="context"><see cref="CashFlowRequestContext"/></param>
        private static void SetAuthorizationHeaderValues(HttpRequestMessage request, RequestModel model)
        {
            if(model.Headers == null) { return; }
            foreach (var headerItem in model.Headers)
            {
                request.Headers.Add(headerItem.Key, headerItem.Value.ToString());
            }
        }
    }
}