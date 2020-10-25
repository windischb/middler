using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using middler.Common.SharedModels.Models;

namespace middler.Common
{
    public interface IMiddlerRequestContext
    {
        //PathString Path { get; set; }
        ClaimsPrincipal Principal { get; set; }
        IPAddress SourceIPAddress { get; set; }
        List<IPAddress> ProxyServers { get; set; }
        MiddlerRouteData RouteData { get; set; }
        SimpleDictionary<string> Headers { get; set; }
        MiddlerRouteQueryParameters QueryParameters { get; set; }
        Uri Uri { get; set; }
        //Uri EncodedUri { get; set; }
        string HttpMethod { get; set; }
        CancellationToken RequestAborted { get; }

        string ContentType { get; set; }

        string GetBodyAsString();

        void SetBody(object body);
    }
}
