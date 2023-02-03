using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Parkway.CBS.RemoteClient
{
    public class RemoteClient : IRemoteClient
    {
        private static readonly Lazy<HttpClientHandler> sharedHandler = new Lazy<HttpClientHandler>(() => new HttpClientHandler());

        public RemoteClient() { }

        public string SendRequest(RequestModel model, HttpMethod method, Dictionary<string, string> queryStringParameters, bool isFormData = false)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpRequestMessage request = BuildRequest(model, method, queryStringParameters, isFormData);

            if (isFormData)
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            }
            else
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

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
                using (var client = new HttpClient(sharedHandler.Value, false))
                {
                    response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"{response.Content.ReadAsStringAsync().Result}, {request.RequestUri}");
                    }
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            #region catch clauses
            catch (HttpRequestException exception)
            {
                throw new Exception($"{exception.InnerException.Message} {JsonConvert.SerializeObject(request)}");
            }
            catch (AggregateException exception)
            {
                throw new Exception($"{exception.InnerExceptions.First().InnerException.Message}, {JsonConvert.SerializeObject(request)} ");
            }
            catch (Exception exception)
            {
                throw new Exception($"{exception.Message}, {JsonConvert.SerializeObject(request)} ");
            }
            #endregion
        }

        private HttpRequestMessage BuildRequest(RequestModel model, HttpMethod method, Dictionary<string, string> queryStringParameters, bool isFormData)
        {
            try
            {
                UriBuilder uri = new UriBuilder(model.URL);
                var query = HttpUtility.ParseQueryString(uri.Query);
                foreach (var item in queryStringParameters) { query[item.Key] = item.Value; }
                uri.Query = query.ToString();
                HttpRequestMessage request = new HttpRequestMessage(method, uri.ToString());
                if (method != HttpMethod.Post) { return request; }
                if (isFormData)
                {
                    request.Content = new FormUrlEncodedContent(model.Model);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(model.Model), Encoding.UTF8, "application/json");

                }
                return request;
            }
            catch (Exception exception)
            {
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
            if (model.Headers == null) { return; }
            foreach (var headerItem in model.Headers)
            {
                request.Headers.Add(headerItem.Key, headerItem.Value.ToString());
            }
        }


    }
}
