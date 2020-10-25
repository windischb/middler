using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using middler.Common;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using middler.Common.StreamHelper;
using middler.Core.Context;
using middler.Core.Models;

namespace middler.Core
{
    public class MiddlerContext: IMiddlerContext
    {
        internal MiddlerRequestContext MiddlerRequestContext { get; }
        public IMiddlerRequestContext Request => MiddlerRequestContext;

        internal MiddlerResponseContext MiddlerResponseContext { get; }
        public IMiddlerResponseContext Response => MiddlerResponseContext;
        public SimpleDictionary<object> PropertyBag { get; } = new SimpleDictionary<object>();

        public IFeatureCollection Features => HttpContext.Features;
        public IServiceProvider RequestServices => HttpContext.RequestServices;
        public void ForwardBody()
        {
            MiddlerResponseContext.Body = MiddlerRequestContext.Body;
        }


        private HttpContext HttpContext { get; }
        private IMiddlerOptions MiddlerOptions { get; }

        //private FakeHttpContext FakeHttpContext { get; }
        public MiddlerContext(HttpContext httpContext, IMiddlerOptions middlerOptions)
        {
            HttpContext = httpContext;
            MiddlerOptions = middlerOptions;
            MiddlerRequestContext = new MiddlerRequestContext(httpContext, middlerOptions);
            MiddlerResponseContext = new MiddlerResponseContext();

            MiddlerResponseContext.Body = new AutoStream(opts => 
                opts
                    .WithMemoryThreshold(middlerOptions.AutoStreamDefaultMemoryThreshold)
                    .WithFilePrefix("middler"), Request.RequestAborted);
        }


        public void SetRouteData(IDictionary<string, IRouteConstraint> constraints)
        {
            MiddlerRequestContext.SetRouteData(constraints);
        }


        //public void PrepareNext()
        //{

            
        //    //MiddlerRequestContext.SetNextBody(MiddlerResponseContext.Body);
        //    if (MiddlerRequestContext.Body.CanSeek)
        //    {
        //        MiddlerRequestContext.Body.Seek(0, SeekOrigin.Begin);
        //    }
           

        //    MiddlerResponseContext.Body = new AutoStream(opts => 
        //        opts
        //            .WithMemoryThreshold(MiddlerOptions.AutoStreamDefaultMemoryThreshold)
        //            .WithFilePrefix("middler"), Request.RequestAborted);

        //}
    }
}