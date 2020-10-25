using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using middler.Common;
using middler.Core.Context;

namespace middler.Core
{
    public class FakeHttpContext: HttpContext
    {



        public override ConnectionInfo Connection => _httpContext.Connection;
        public override IFeatureCollection Features => _middlerContext.Features;

        public override IDictionary<object, object> Items
        {
            get => _httpContext.Items;
            set => _httpContext.Items = value;
        }
        public override HttpRequest Request { get; }

        public override CancellationToken RequestAborted
        {
            get => _httpContext.RequestAborted;
            set => _httpContext.RequestAborted = value;
        }

        public override IServiceProvider RequestServices
        {
            get => _httpContext.RequestServices;
            set => _httpContext.RequestServices = value;
        }

        public override HttpResponse Response { get; }
        public override ISession Session
        {
            get => _httpContext.Session;
            set => _httpContext.Session = value;
        }
        public override string TraceIdentifier
        {
            get => _httpContext.TraceIdentifier;
            set => _httpContext.TraceIdentifier = value;
        }
        public override ClaimsPrincipal User
        {
            get => _httpContext.User;
            set => _httpContext.User = value;
        }

        public override WebSocketManager WebSockets => _httpContext.WebSockets;


        private readonly MiddlerContext _middlerContext;
        private HttpContext _httpContext;
        public FakeHttpContext(MiddlerContext middlerContext, HttpContext httpContext)
        {
            _middlerContext = middlerContext;
            _httpContext = httpContext;
            Request = new FakeHttpRequest(middlerContext.MiddlerRequestContext, httpContext);
            Response = new FakeHttpResponse(middlerContext.MiddlerResponseContext, httpContext);
        }



        public override void Abort()
        {
            throw new NotImplementedException();
        }

    }

    public class FakeHttpRequest: HttpRequest
    {
        private MiddlerRequestContext request;
        private HttpRequest _httpRequest;
        public FakeHttpRequest(MiddlerRequestContext request, HttpContext httpContext)
        {
            this.request = request;
            HttpContext = httpContext;
            _httpRequest = httpContext.Request;
        }

        public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _httpRequest.ReadFormAsync(cancellationToken);
        }

        public override Stream Body
        {
            get => _httpRequest.Body;
            set => _httpRequest.Body = value;
        }

        public override long? ContentLength
        {
            get => _httpRequest.ContentLength;
            set => _httpRequest.ContentLength = value;
        }

        public override string ContentType
        {
            get => _httpRequest.ContentType;
            set => _httpRequest.ContentType = value;
        }

        public override IRequestCookieCollection Cookies
        {
            get => _httpRequest.Cookies;
            set => _httpRequest.Cookies = value;
        }
        public override IFormCollection Form
        {
            get => _httpRequest.Form;
            set => _httpRequest.Form = value;
        }

        public override bool HasFormContentType => _httpRequest.HasFormContentType;
        public override IHeaderDictionary Headers => _httpRequest.Headers;
        public override HostString Host
        {
            get => _httpRequest.Host;
            set => _httpRequest.Host = value;
        }
        public override HttpContext HttpContext { get; }
        public override bool IsHttps
        {
            get => _httpRequest.IsHttps;
            set => _httpRequest.IsHttps = value;
        }
        public override string Method 
        {
            get => _httpRequest.Method;
            set => _httpRequest.Method = value;
        }
        public override PathString Path
        {
            get => _httpRequest.Path;
            set => _httpRequest.Path = value;
        }
        public override PathString PathBase
        {
            get => _httpRequest.PathBase;
            set => _httpRequest.PathBase = value;
        }
        public override string Protocol
        {
            get => _httpRequest.Protocol;
            set => _httpRequest.Protocol = value;
        }
        public override IQueryCollection Query
        {
            get => _httpRequest.Query;
            set => _httpRequest.Query = value;
        }
        public override QueryString QueryString
        {
            get => _httpRequest.QueryString;
            set => _httpRequest.QueryString = value;
        }
        public override string Scheme
        {
            get => _httpRequest.Scheme;
            set => _httpRequest.Scheme = value;
        }

        public override RouteValueDictionary RouteValues
        {
            get => _httpRequest.RouteValues;
            set => _httpRequest.RouteValues = value;
        }
    }

    public class FakeHttpResponse: HttpResponse
    {
        

        public override Stream Body {
            get => _middlerResponseContext.Body;
            set {}
        }
        public override long? ContentLength
        {
            get => _httpResponse.ContentLength;
            set => _httpResponse.ContentLength = value;
        }
        public override string ContentType
        {
            get => _httpResponse.ContentType;
            set => _httpResponse.ContentType = value;
        }
        public override IResponseCookies Cookies
        {
            get => _httpResponse.Cookies;
        }
        public override bool HasStarted
        {
            get => false;
        }
        public override IHeaderDictionary Headers
        {
            get => _httpResponse.Headers;
        }
        public override HttpContext HttpContext { get; }
        public override int StatusCode
        {
            get => _middlerResponseContext.StatusCode;
            set => _middlerResponseContext.StatusCode = value;
        }


        private MiddlerResponseContext _middlerResponseContext;
        private HttpResponse _httpResponse;
        public FakeHttpResponse(MiddlerResponseContext responseContext, HttpContext httpContext)
        {
            _middlerResponseContext = responseContext;
            HttpContext = httpContext;
            _httpResponse = httpContext.Response;
            Body = responseContext.Body;
        }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }

        
    }
}
