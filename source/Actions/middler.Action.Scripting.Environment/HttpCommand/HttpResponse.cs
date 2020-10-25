using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using middler.Scripting.ExtensionMethods;

namespace middler.Scripting.HttpCommand
{
    public class HttpResponse : IDisposable
    {

        private readonly HttpResponseMessage _httpResponseMessage;

        public Version Version => _httpResponseMessage.Version;

        private GenericHttpContent _content;
        public GenericHttpContent Content => _content ??= new GenericHttpContent(_httpResponseMessage.Content);

        private ExpandoObject _contentHeaders;
        public ExpandoObject ContentHeaders => _contentHeaders ??= _httpResponseMessage.Content?.Headers?.ToExpandoObject();

        public HttpStatusCode StatusCode => _httpResponseMessage.StatusCode;
        public string ReasonPhrase => _httpResponseMessage.ReasonPhrase;

        private ExpandoObject _headers;
        public ExpandoObject Headers => _headers ??= _httpResponseMessage.Headers.ToExpandoObject();

        private ExpandoObject _trailingheaders;
        public ExpandoObject TrailingHeaders => _trailingheaders ??= _httpResponseMessage.TrailingHeaders.ToExpandoObject();

        public bool IsSuccessStatusCode => _httpResponseMessage.IsSuccessStatusCode;
        public HttpResponse EnsureSuccessStatusCode()
        {
            _httpResponseMessage.EnsureSuccessStatusCode();
            return this;
        }

        public override string ToString() => _httpResponseMessage.ToString();


        internal HttpResponse(HttpResponseMessage httpResponseMessage)
        {
            _httpResponseMessage = httpResponseMessage;
            //Content = new GenericHttpContent(_httpResponseMessage.Content);
        }


        public void Dispose()
        {
            _httpResponseMessage?.Dispose();
        }

        
    }
}