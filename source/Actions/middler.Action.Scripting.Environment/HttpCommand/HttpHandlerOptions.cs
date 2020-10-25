using System;
using System.Net;

namespace middler.Scripting.HttpCommand
{
    public class HttpHandlerOptions
    {

        public Uri RequestUri { get; }
        public WebProxy Proxy { get; set; }
        public bool IgnoreProxy { get; set; }

        public HttpHandlerOptions(Uri uri)
        {
            RequestUri = uri;
        }

    }
}