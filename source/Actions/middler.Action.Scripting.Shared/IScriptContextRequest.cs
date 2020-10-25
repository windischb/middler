using middler.Common.SharedModels.Models;
using System;
using System.Collections.Generic;

namespace middler.Action.Scripting.Shared
{
    public interface IScriptContextRequest
    {
        string HttpMethod { get; }

        Uri Uri { get; }

        MiddlerRouteData RouteData { get; }
        SimpleDictionary<string> Headers { get; }
        MiddlerRouteQueryParameters QueryParameters { get; }

        string ClientIp { get; }
        string[] ProxyServers { get; }

        string GetBodyAsString();
        void SetBody(object body);
    }
}