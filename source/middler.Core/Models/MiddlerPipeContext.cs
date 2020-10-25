using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace middler.Core.Models
{
    public class MiddlerPipeContext
    {
        
       public PipeRequest Request { get; set; }

       
    }

    public class PipeRequest
    {
        public string HttpMethod { get; }
        public Uri Uri { get; }
        public IDictionary<string, object> RouteData { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public IDictionary<string, string> QueryParameters { get; set; }
        public string UserAgent { get; }
        public string ClientIp { get; }
        public string[] ProxyServers { get; }
    }

    public class PipeResponse
    {

    }
}
