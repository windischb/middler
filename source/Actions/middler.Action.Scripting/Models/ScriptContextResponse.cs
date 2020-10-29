using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using middler.Action.Scripting.Shared;
using middler.Common;
using middler.Common.SharedModels.Models;
using Reflectensions;
using Reflectensions.ExtensionMethods;

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

            var isArray = body.GetType().IsEnumerableType();
            if (isArray)
            {
                var list = new List<object>();
                var arr = body as IEnumerable;
                foreach (var o in arr)
                {
                    if (o is PSObject pso)
                    {
                        var json = Json.Converter.ToJToken(o);
                        list.Add(Json.Converter.ToBasicDotNetObject(json));
                    }
                }
                _middlerResponseContext.SetBody(list);
                return;
            }

            if (body is PSObject _pso)
            {
                var json = Json.Converter.ToJToken(_pso);
                var obj = Json.Converter.ToBasicDotNetObject(json);
                _middlerResponseContext.SetBody(obj);
                return;
            }

            _middlerResponseContext.SetBody(body);
        }


    }
}