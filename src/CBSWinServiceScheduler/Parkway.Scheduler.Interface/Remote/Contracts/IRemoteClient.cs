using System.Collections.Generic;

namespace Parkway.Scheduler.Interface.Remote.Contracts
{
    internal interface IRemoteClient
    {
        RemoteClientResponse SendRequest(string URL, string verb, Dictionary<string, dynamic> headers, dynamic bodyParams = null, string headerContentType = "application/json", string bodyContentType = "application/json");
    }
}
