using middler.Action.Scripting.Shared;
using middler.Common;
using middler.Common.SharedModels.Models;

namespace middler.Action.Scripting.Models
{
    public class ScriptContextResponse: IScriptContextResponse
    {
        public int StatusCode
        {
            get => _middlerResponseContext.StatusCode;
            set => _middlerResponseContext.StatusCode = value;
        }
        public SimpleDictionary<string> Headers
        {
            get => _middlerResponseContext.Headers;
            set => _middlerResponseContext.Headers = value;
        }

        private IMiddlerResponseContext _middlerResponseContext;

        public ScriptContextResponse(IMiddlerResponseContext middlerResponseContext)
        {
            _middlerResponseContext = middlerResponseContext;
        }

        public string GetBodyAsString()
        {
            return _middlerResponseContext.GetBodyAsString();
        }
        public void SetBody(object body)
        {
            _middlerResponseContext.SetBody(body);
        }


    }
}