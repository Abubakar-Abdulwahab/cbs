using Newtonsoft.Json;
using Parkway.Scheduler.Interface.Remote.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.Remote
{
    internal class RemoteClient : IRemoteClient
    {
        private static readonly Lazy<HttpClientHandler> sharedHandler = new Lazy<HttpClientHandler>(() => new HttpClientHandler());

        public RemoteClientResponse SendRequest(string URL, string verb, Dictionary<string, dynamic> headers, dynamic bodyParams = null, string headerContentType = "application/json", string bodyContentType = "application/json")
        {
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(verb), URL);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(headerContentType));
            SetAuthorizationHeaderValues(request, headers);
            BuildRequest(request, bodyParams, bodyContentType);
            return ExecuteRequest(request);
        }

        private static void SetAuthorizationHeaderValues(HttpRequestMessage request, Dictionary<string, dynamic> headers)
        {
            foreach (var headerItem in headers)
            {
                request.Headers.Add(headerItem.Key, headerItem.Value.ToString());
            }
        }

        private static void BuildRequest(HttpRequestMessage request, dynamic bodyParams, string bodyContentType)
        {
            if (request.Method != HttpMethod.Post || bodyParams == null) { return; }
            var sd = JsonConvert.SerializeObject(bodyParams);
            request.Content = new StringContent(JsonConvert.SerializeObject(bodyParams), Encoding.UTF8, bodyContentType);//content type header for input data
        }

        private static RemoteClientResponse ExecuteRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            RemoteClientResponse result = new RemoteClientResponse();
            try
            {
                using (var client = new HttpClient(sharedHandler.Value, false))
                {
                    response = client.SendAsync(request).Result;
                    result.StatusCode = response.StatusCode;
                    result.Response = response.Content.ReadAsStringAsync().Result;
                    return result;
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
                throw new Exception(exception.Message);
            }
            #endregion
        }
    }

    internal class RemoteClientResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Response { get; set; }
    }
}
