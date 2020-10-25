using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nito.AsyncEx.Synchronous;
using Reflectensions.ExtensionMethods;
using Reflectensions.Helper;

namespace middler.Scripting.HttpCommand
{
    public class HttpRequestBuilder : IHttpRequestBuilder
    {

        private readonly HttpHandlerOptions _httpHandlerOptions;

        private readonly HttpRequestData _requestData = new HttpRequestData();


        public HttpRequestBuilder(HttpHandlerOptions httpHandlerOptions)
        {
            _httpHandlerOptions = httpHandlerOptions;
        }

        public HttpRequestBuilder UsePath(string url)
        {
            _requestData.Path = url;
            return this;
        }


        public HttpRequestBuilder AddHeader(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();

            _requestData.Headers.AddOrUpdate(key, 
                _=> value.ToList(),
                (s, list) =>
                {
                    list.AddRange(value);
                    return list.Distinct().ToList();
                });

            return this;

        }

        public HttpRequestBuilder SetHeader(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();
            _requestData.Headers.Remove(key, out var _);
            _requestData.Headers.TryAdd(key, value.ToList());

            return this;
        }

        public HttpRequestBuilder SetContentType(string contentType)
        {
            _requestData.ContentType = contentType;
            return this;
        }

        public HttpRequestBuilder AddQueryParam(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();

            _requestData.QueryParameters.AddOrUpdate(key, s => {
                return new StringValues(value);
            }, (s, sv) => {
                var temp = sv.ToList();
                temp.AddRange(value);
                return new StringValues(temp.ToArray());

            });

            return this;
        }

        public HttpRequestBuilder SetQueryParam(string key, params string[] value)
        {
            if (String.IsNullOrWhiteSpace(key))
                return this;

            key = key.ToLower();

            _requestData.QueryParameters.TryRemove(key, out var _);
            _requestData.QueryParameters.TryAdd(key, new StringValues(value));
            
            return this;
        }

        public HttpRequestBuilder SetBearerToken(string token)
        {
            return SetHeader("authorization", $"Bearer {token}");
        }

        public HttpRequestBuilder SetBasicAuthentication(string username, string password)
        {
            var cred = $"{username}:{password}".EncodeToBase64();
            return SetHeader("authorization", $"Basic {cred}");
        }


        public async Task<HttpResponse> SendRequestMessageAsync(HttpRequestMessage httpRequestMessage)
        {
            var cl = new HttpClient(HttpHandlerFactory.Build(_httpHandlerOptions));
            var respMsg = await cl.SendAsync(httpRequestMessage);
            return new HttpResponse(respMsg);
        }

        public Task<HttpResponse> SendAsync(string httpMethod, object content = null)
        {
            return SendAsync(new HttpMethod(httpMethod), content);
        }

        public async Task<HttpResponse> SendAsync(HttpMethod httpMethod, object content = null)
        {
            var httpRequestMessage = await _requestData.BuildHttpRequestMessage(_httpHandlerOptions, httpMethod, content);
            return await SendRequestMessageAsync(httpRequestMessage);
        }

        public Task<HttpResponse> GetAsync()
        {
            return SendAsync(HttpMethod.Get);
        }

        public Task<HttpResponse> PostAsync(object content)
        {
            return SendAsync(HttpMethod.Post, content);
        }

        public Task<HttpResponse> PutAsync(object content)
        {
            return SendAsync(HttpMethod.Put, content);
        }

        public Task<HttpResponse> PatchAsync(object content)
        {
            return SendAsync(HttpMethod.Patch, content);
        }

        
        public Task<HttpResponse> DeleteAsync(object content)
        {
            return SendAsync(HttpMethod.Delete, content);
        }

        public Task<HttpResponse> DeleteAsync()
        {
            return DeleteAsync(null);
        }

        public HttpResponse Send(string httpMethod, object content = null)
        {
            return SendAsync(httpMethod, content).WaitAndUnwrapException();
        }

        public HttpResponse Send(HttpMethod httpMethod, object content = null)
        {
            return SendAsync(httpMethod, content).WaitAndUnwrapException();
        }

        public HttpResponse SendRequestMessage(HttpRequestMessage httpRequestMessage)
        {
            return SendRequestMessageAsync(httpRequestMessage).WaitAndUnwrapException();
        }

        public HttpResponse Get()
        {
            return GetAsync().WaitAndUnwrapException();
        }

        public HttpResponse Post(object content)
        {
            return PostAsync(content).WaitAndUnwrapException();
        }

        public HttpResponse Put(object content)
        {
            return PutAsync(content).WaitAndUnwrapException();
        }

        public HttpResponse Patch(object content)
        {
            return PatchAsync(content).WaitAndUnwrapException();
        }

        public HttpResponse Delete(object content)
        {
            return DeleteAsync(content).WaitAndUnwrapException();
        }
        public HttpResponse Delete()
        {
            return Delete(null);
        }

    }

    
}