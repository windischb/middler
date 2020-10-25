using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using middler.Common;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using middler.Common.StreamHelper;
using middler.Core.ExtensionMethods;
using Newtonsoft.Json;
using Reflectensions.ExtensionMethods;

namespace middler.Core
{
    public class MiddlerRequestContext:  IMiddlerRequestContext
    {
        //public PathString Path { get; set; }
        public ClaimsPrincipal Principal { get; set; } 
        public IPAddress SourceIPAddress { get; set; }
        public List<IPAddress> ProxyServers { get; set; }
        public MiddlerRouteData RouteData { get; set; } = new MiddlerRouteData();
        public SimpleDictionary<string> Headers { get; set; }
        public MiddlerRouteQueryParameters QueryParameters { get; set; }

        public Uri Uri { get; set; }


        public string HttpMethod { get; set; }

        public CancellationToken RequestAborted => HttpContext.RequestAborted;
        public string ContentType { get; set; }
       
        public Stream Body { get; private set; }

        private HttpContext HttpContext { get; }
        private IMiddlerOptions MiddlerOptions { get; }


        public MiddlerRequestContext(HttpContext httpContext, IMiddlerOptions middlerOptions)
        {
            HttpContext = httpContext;
            //Path = httpContext.Request.Path;
            Principal = httpContext.User;
            SourceIPAddress = httpContext.Request.FindSourceIp().FirstOrDefault();
            ProxyServers = httpContext.Request.FindSourceIp().Skip(1).ToList();
            Headers = new SimpleDictionary<string>(httpContext.Request.GetHeaders());
            QueryParameters = new MiddlerRouteQueryParameters(httpContext.Request.GetQueryParameters());
            Uri = new Uri(httpContext.Request.GetDisplayUrl());
            HttpMethod = httpContext.Request.Method;
            ContentType = httpContext.Request.ContentType;
            MiddlerOptions = middlerOptions;
            
            Body = httpContext.Request.Body;
        }

        public RouteData GetRouteData()
        {
            return HttpContext.GetRouteData();
        }

        internal void SetRouteData(IDictionary<string, IRouteConstraint> constraints)
        {
            var middlerRouteData = new MiddlerRouteData();

            //middlerRouteData["@HOST"] = Uri.Host;
            //middlerRouteData["@SCHEME"] = Uri.Scheme;
            //middlerRouteData["@PORT"] = Uri.Port;
            //middlerRouteData["@PATH"] = Uri.AbsolutePath;

            var rd = GetRouteData();

            foreach (var key in rd.Values.Keys)
            {
                var val = rd.Values[key]?.ToString();

                if (val == null)
                {
                    middlerRouteData.Add(key.ToLower(), null);

                    continue;
                }

                if (constraints.ContainsKey(key))
                {

                    var constraint = constraints[key];
                    IRouteConstraint ic;
                    if (constraint is OptionalRouteConstraint optionalRouteConstraint)
                    {
                        ic = optionalRouteConstraint.InnerConstraint;
                    }
                    else
                    {
                        ic = constraint;
                    }

                    object value;
                    if (ic is IntRouteConstraint)
                    {
                        value = val.ToInt();
                    }
                    else if (ic is BoolRouteConstraint)
                    {
                        value = val.ToBoolean();
                    }
                    else if (ic is DateTimeRouteConstraint)
                    {
                        value = val.ToDateTime();
                    }
                    else if (ic is DecimalRouteConstraint)
                    {
                        value = val.ToDecimal();
                    }
                    else if (ic is DoubleRouteConstraint)
                    {
                        value = val.ToDouble();
                    }
                    else if (ic is FloatRouteConstraint)
                    {
                        value = val.ToFloat();
                    }
                    else if (ic is GuidRouteConstraint)
                    {
                        value = new Guid(val);
                    }
                    else if (ic is LongRouteConstraint)
                    {
                        value = val.ToLong();
                    }
                    else
                    {
                        value = val;
                    }

                    middlerRouteData.Add(key.ToLower(), value);
                }
                else
                {
                    middlerRouteData.Add(key.ToLower(), val);
                }

            }

            RouteData = middlerRouteData;

        }
        
        public string GetBodyAsString()
        {
            if (Body is AutoStream)
            {
                using var sr = new StreamReader(Body);
                Body.Seek(0, SeekOrigin.Begin);
                return sr.ReadToEnd();
            }
            else
            {
                var aStream = new AutoStream(opts =>
                    opts
                        .WithMemoryThreshold(MiddlerOptions.AutoStreamDefaultMemoryThreshold)
                        .WithFilePrefix("middler"), RequestAborted);

                Body.CopyToAsync(aStream, 131072, RequestAborted).GetAwaiter().GetResult();
                Body = aStream;
                return GetBodyAsString();
            }
        }

        public void SetBody(object body)
        {

            if (Body is AutoStream)
            {
                Body.Seek(0, SeekOrigin.Begin);
                Body.SetLength(0);
                switch (body)
                {
                    case string str:
                    {
                        SetStringBody(str);
                        return;
                    }
                    case Stream stream:
                    {
                        SetStreamBody(stream);
                        return;
                    }
                    default:
                    {
                        SetObjectBody(body);
                        break;
                    }
                }
            }
            else
            {
                Body = new AutoStream(opts =>
                    opts
                        .WithMemoryThreshold(MiddlerOptions.AutoStreamDefaultMemoryThreshold)
                        .WithFilePrefix("middler"), RequestAborted);
                SetBody(body);
            }
            


        }

        private void SetObjectBody(object @object)
        {
            using var sw = new StreamWriter(Body, new UTF8Encoding(false), 8192, true);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(sw, @object);

        }

        private void SetStringBody(string content)
        {
            using var sw = new StreamWriter(Body, new UTF8Encoding(false), 8192, true);
            sw.Write(content);
        }

        private void SetStreamBody(Stream stream)
        {
            Body = stream;
        }

    }
}
