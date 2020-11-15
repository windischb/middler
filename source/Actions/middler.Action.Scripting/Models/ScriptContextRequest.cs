using System;
using System.Linq;
using middler.Common;
using middler.Common.SharedModels.Models;

namespace middler.Action.Scripting.Models
{
    public class ScriptContextRequest
    {
        public string HttpMethod => _middlerRequestContext.HttpMethod;
        public Uri Uri => _middlerRequestContext.Uri;
        public MiddlerRouteData RouteData => _middlerRequestContext.RouteData;
        public SimpleDictionary<string> Headers => _middlerRequestContext.Headers;
        public MiddlerRouteQueryParameters QueryParameters => _middlerRequestContext.QueryParameters;
        public string User => Authenticated ? _middlerRequestContext.Principal.Identity?.Name : "Anonymous";

        public string Client =>
            _middlerRequestContext.Principal.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

        public bool Authenticated => _middlerRequestContext.Principal.Identity?.IsAuthenticated ?? false;

        public string ClientIp=> _middlerRequestContext.SourceIPAddress.ToString();
        public string[] ProxyServers => _middlerRequestContext.ProxyServers.Select(ip => ip.ToString()).ToArray();

        private readonly IMiddlerRequestContext _middlerRequestContext;
        public ScriptContextRequest(IMiddlerRequestContext middlerRequestContext)
        {
            _middlerRequestContext = middlerRequestContext;
        }

        public string GetBodyAsString()
        {
            return _middlerRequestContext.GetBodyAsString();
        }

        public void SetBody(object body)
        {
            _middlerRequestContext.SetBody(body);
        }
    }
}