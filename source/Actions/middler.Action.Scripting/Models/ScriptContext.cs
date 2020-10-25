using Microsoft.AspNetCore.Http;
using middler.Action.Scripting.Shared;
using middler.Common;
using middler.Common.SharedModels.Models;

namespace middler.Action.Scripting.Models
{
    public class ScriptContext
    {
        private readonly ScriptContextMethods _scriptContextMethods;

        public IScriptContextResponse Response { get; }
        public IScriptContextRequest Request { get; }
        public SimpleDictionary<object> PropertyBag => _middlerContext.PropertyBag;

        public bool Terminating { get; set; }

        private IMiddlerContext _middlerContext;
        public ScriptContext(IMiddlerContext middlerContext, ScriptContextMethods scriptContextMethods)
        {
            _middlerContext = middlerContext;
            _scriptContextMethods = scriptContextMethods;
            Request = new ScriptContextRequest(middlerContext.Request);
            Response = new ScriptContextResponse(middlerContext.Response);
        }

        public void SendResponse()
        {
            _scriptContextMethods.SendResponse?.Invoke();
        }

        //public void ForwardBody()
        //{
        //    _middlerContext.ForwardBody();
        //}
    }
}
