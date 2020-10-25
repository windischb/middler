using System;
using middler.Common;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Action.UrlRewrite
{
    public class UrlRewriteAction: MiddlerAction<UrlRewriteOptions>
    {
        internal static string DefaultActionType => "UrlRewrite";

        public override bool Terminating => false;

        public override bool WriteStreamDirect => false;
        public override string ActionType => DefaultActionType;


        public void ExecuteRequest(IMiddlerContext middlerContext, IActionHelper actionHelper)
        {

            var rewriteTo = actionHelper.BuildPathFromRoutData(Parameters.RewriteTo);
            var isAbsolute = Uri.IsWellFormedUriString(rewriteTo, UriKind.Absolute);
            if (isAbsolute)
            {
                middlerContext.Request.Uri = new Uri(rewriteTo);
            }
            else
            {
                var builder = new UriBuilder(middlerContext.Request.Uri);
                builder.Query = null;
                if (rewriteTo.Contains("?"))
                {
                    builder.Path = rewriteTo.Split("?")[0];
                    builder.Query = rewriteTo.Split("?")[1];
                }
                else
                {
                    builder.Path = rewriteTo;
                }
                
                
                middlerContext.Request.Uri = builder.Uri;
            }



            //middlerContext.ForwardBody();

            //var nUrl = new UriBuilder(Parameters.RewriteTo);
            //builder.Path = Parameters.RewriteTo;

        }

        public void ExecuteResponse(IMiddlerContext middlerContext)
        {
            middlerContext.Response.Headers["TestHeader"] = "irgendwas";
           
        }
    }
}
