using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Utf8Json;

namespace middler.Scripting.HttpCommand
{
    public class HttpRequestData
    {
        public ConcurrentDictionary<string, StringValues> QueryParameters { get; } = new ConcurrentDictionary<string, StringValues>();
        public ConcurrentDictionary<string, List<string>> Headers { get; } = new ConcurrentDictionary<string, List<string>>();

        public string ContentType { get; set; }

        public string Path { get; set; }

        public async Task<HttpRequestMessage> BuildHttpRequestMessage(HttpHandlerOptions httpHandlerOptions, HttpMethod httpMethod, object content)
        {


            var requestUri = String.IsNullOrWhiteSpace(Path)
                ? httpHandlerOptions.RequestUri
                : new Uri(httpHandlerOptions.RequestUri, Path);

            

            if(QueryParameters.Any())
            {
                
                var qb = new QueryBuilder();
                if (!String.IsNullOrWhiteSpace(requestUri.Query))
                {
                    var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(requestUri.Query);
                    foreach (var (key, value) in query)
                    {
                        qb.Add(key, value.ToArray());
                    }
                }
                foreach (var (key, value) in QueryParameters)
                {
                    qb.Add(key, value.ToArray());
                }

                var uriBuilder = new UriBuilder(requestUri);
                uriBuilder.Query = qb.ToQueryString().ToString();

                requestUri = uriBuilder.Uri;

            }

           

            var message = new HttpRequestMessage(httpMethod, requestUri);

            if (content != null)
            {
                message.Content = await CreateHttpContent(content);
            }

            if (Headers.Any())
            {
                foreach (var (key, value) in Headers)
                {
                    if (message.Headers.Contains(key))
                    {
                        message.Headers.Remove(key);
                    }
                    message.Headers.Add(key, value);

                }
            }


            if (!String.IsNullOrWhiteSpace(ContentType))
            {
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            return message;
        }


        private async Task<HttpContent> CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content == null)
                return null;

            var ms = new MemoryStream();

            if (content is string str)
            {
                httpContent = new StringContent(str, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                return httpContent;
            }

            await CreateJsonHttpContent(content, ms);
            ms.Seek(0, SeekOrigin.Begin);
            httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpContent;
        }

        private async Task CreateJsonHttpContent(object content, Stream stream)
        {
            await using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);

            await JsonSerializer.SerializeAsync(stream, content);
            
        }
    }
}