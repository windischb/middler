using System;
using System.Net;
using middler.Common.SharedModels.Models;


namespace middler.Scripting.HttpCommand
{
    public class HttpOptionsBuilder
    {
        private readonly HttpHandlerOptions _httpHandlerOptions;

        public HttpOptionsBuilder(string url)
        {
            _httpHandlerOptions = new HttpHandlerOptions(UriHelper.BuildUri(url));
        }
        

        public HttpOptionsBuilder UseProxy(WebProxy proxy)
        {
            _httpHandlerOptions.IgnoreProxy = false;
            _httpHandlerOptions.Proxy = proxy;
            return this;
        }

        public HttpOptionsBuilder UseProxy(Uri proxy)
        {
            return UseProxy(new WebProxy(proxy));
        }

        public HttpOptionsBuilder UseProxy(Uri proxy, ICredentials credentials)
        {
            var webProxy = new WebProxy(proxy)
            {
                Credentials = credentials
            };
            return UseProxy(webProxy);
        }

        public HttpOptionsBuilder UseProxy(Uri proxy, SimpleCredentials credentials)
        {
            return UseProxy(proxy, (NetworkCredential)credentials);
        }


        public HttpOptionsBuilder UseProxy(string proxy)
        {
            var uri = UriHelper.BuildUri(proxy);
            return UseProxy(uri);
        }

        public HttpOptionsBuilder UseProxy(string proxy, ICredentials credentials)
        {
            var uri = UriHelper.BuildUri(proxy);
            return UseProxy(uri, credentials);
        }

        public HttpOptionsBuilder UseProxy(string proxy, SimpleCredentials credentials)
        {
            var uri = UriHelper.BuildUri(proxy);
            return UseProxy(uri, (NetworkCredential)credentials);
        }

        public HttpOptionsBuilder IgnoreProxy()
        {
            return IgnoreProxy(true);
        }

        public HttpOptionsBuilder IgnoreProxy(bool value)
        {
            _httpHandlerOptions.IgnoreProxy = value;
            return this;
        }


        public static implicit operator HttpHandlerOptions(HttpOptionsBuilder optionsOptionsBuilder)
        {
            return optionsOptionsBuilder._httpHandlerOptions;
        }
    }
}