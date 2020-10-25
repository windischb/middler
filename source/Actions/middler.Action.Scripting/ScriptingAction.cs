using System;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using middler.Action.Scripting.ExtensionMethods;
using middler.Action.Scripting.Javascript;
using middler.Action.Scripting.Models;
using middler.Action.Scripting.Powershell;
using middler.Action.Scripting.Shared;
using middler.Action.Scripting.Typescript;
using middler.Common;
using middler.Common.SharedModels.Interfaces;
using middler.Common.SharedModels.Models;
using Environment = middler.Scripting.Environment;

namespace middler.Action.Scripting
{
    public class ScriptingAction: MiddlerAction<ScriptingOptions>
    {
        internal static string DefaultActionType => "Script";
        public override string ActionType => DefaultActionType;

        public override bool Terminating { get; set; } = true;

        public async Task ExecuteRequestAsync(IMiddlerContext middlerContext)
        {

            IScriptEngine scriptEngine = GetScriptEngine();




            var scriptContextMethods = new ScriptContextMethods();
            scriptContextMethods.SendResponse = () =>
            {
                Console.WriteLine("Test von Action");
                scriptEngine.Stop();
            };

            var scriptContext = new ScriptContext(middlerContext, scriptContextMethods);
            scriptContext.Terminating = Terminating;

            scriptEngine.Initialize();
            scriptEngine.SetValue("context", scriptContext);

            scriptEngine.SetValue("m", new Environment(middlerContext.RequestServices.GetService<IVariablesRepository>()));
            

            try
            {
                await scriptEngine.Execute(scriptEngine.NeedsCompiledScript ? Parameters.CompiledCode : Parameters.SourceCode);
                //SendResponse(middlerContext.Response, scriptContext);
            }
            catch (Exception e)
            {
                throw e;
                //await httpContext.BadRequest(e.GetBaseException().Message);
            }

            Terminating = scriptContext.Terminating;
        }


        //private void SendResponse(IMiddlerResponseContext httpContext, ScriptContext scriptContext)
        //{
        //    httpContext.StatusCode = scriptContext.Response.StatusCode ?? 200;
        //    httpContext.SetBody(scriptContext.Response.Body.ToString());
        //}


        private IScriptEngine GetScriptEngine()
        {
            switch (Parameters.Language)
            {
                case ScriptLanguage.Javascript:
                {
                   return new JavascriptEngine();
                }
                case ScriptLanguage.Powershell:
                {
                    return new PowershellEngine();
                }
                case ScriptLanguage.Typescript:
                {
                    return new TypescriptEngine();
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }

        }

        public string CompileScriptIfNeeded()
        {
            IScriptEngine scriptEngine = GetScriptEngine();
            if (scriptEngine.NeedsCompiledScript)
            {
                Parameters.CompiledCode = scriptEngine.CompileScript?.Invoke(Parameters.SourceCode);
            }

            return Parameters.CompiledCode;
        }

    }
}
