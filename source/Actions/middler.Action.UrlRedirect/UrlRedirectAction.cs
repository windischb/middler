using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Common.Actions.UrlRedirect
{
    public class UrlRedirectAction: MiddlerAction<UrlRedirectOptions>
    {
        internal static string DefaultActionType => "UrlRedirect";
        public override bool Terminating => true;

        public void ExecuteRequest(IMiddlerContext middlerContext, IActionHelper actionHelper)
        {
            var uri = new Uri(actionHelper.BuildPathFromRoutData(Parameters.RedirectTo));
            if (Parameters.PreserveMethod)
            {
                middlerContext.Response.StatusCode = Parameters.Permanent ? StatusCodes.Status308PermanentRedirect : StatusCodes.Status307TemporaryRedirect;
            }
            else
            {
                middlerContext.Response.StatusCode =  Parameters.Permanent ? StatusCodes.Status301MovedPermanently : StatusCodes.Status302Found;
            }

            middlerContext.Response.Headers["Location"] = uri.AbsoluteUri;
           
        }

        public override string ActionType => DefaultActionType;
    }
}
