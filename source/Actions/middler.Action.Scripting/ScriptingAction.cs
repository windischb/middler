using System;
using System.Threading.Tasks;
using middler.Action.Scripting.Models;
using middler.Common;
using middler.Common.SharedModels.Models;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Scripter.Shared;


namespace middler.Action.Scripting
{
    public class ScriptingAction: MiddlerAction<ScriptingOptions>
    {
        internal static string DefaultActionType => "Script";
        public override string ActionType => DefaultActionType;

        public override bool Terminating { get; set; } = true;

        private IServiceProvider _serviceProvider;

        public async Task ExecuteRequestAsync(IMiddlerContext middlerContext)
        {
            _serviceProvider = middlerContext.RequestServices;

            IScriptEngine scriptEngine = _serviceProvider.GetRequiredNamedService<IScriptEngine>(Parameters.Language);

            var compile = scriptEngine.NeedsCompiledScript && (!string.IsNullOrEmpty(Parameters.SourceCode) &&
                                                               string.IsNullOrWhiteSpace(Parameters.CompiledCode));

            
            if (compile)
            {
                CompileScriptIfNeeded();
            }

            var scriptContextMethods = new ScriptContextMethods();
            scriptContextMethods.SendResponse = () =>
            {
                Console.WriteLine("Test von Action");
                scriptEngine.Stop();
            };

            var scriptContext = new ScriptContext(middlerContext, scriptContextMethods);
            scriptContext.Terminating = Terminating;

            
            scriptEngine.SetValue("context", scriptContext);

            //scriptEngine.SetValue("m", new Environment(middlerContext.RequestServices.GetService<IVariablesRepository>()));
            

            try
            {
                await scriptEngine.ExecuteAsync(scriptEngine.NeedsCompiledScript ? Parameters.CompiledCode : Parameters.SourceCode);
                //SendResponse(middlerContext.Response, scriptContext);
            }
            catch (Exception e)
            {
                throw e;
                //await httpContext.BadRequest(e.GetBaseException().Message);
            }

            Terminating = scriptContext.Terminating;
        }


        //private IScriptEngine GetScriptEngine()
        //{
        //    switch (Parameters.Language)
        //    {
        //        case ScriptLanguage.Javascript:
        //        {
        //           return new JavascriptEngine();
        //        }
        //        case ScriptLanguage.Powershell:
        //        {
        //            return new PowershellEngine();
        //        }
        //        case ScriptLanguage.Typescript:
        //        {
        //            return new TypescriptEngine();
        //        }
        //        default:
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }

        //}

        public string CompileScriptIfNeeded()
        {
            
            IScriptEngine scriptEngine = _serviceProvider.GetRequiredNamedService<IScriptEngine>(Parameters.Language);
            if (scriptEngine.NeedsCompiledScript)
            {
                Parameters.CompiledCode = scriptEngine.CompileScript?.Invoke(Parameters.SourceCode);
            }

            return Parameters.CompiledCode;
        }

        //public string GetTypeScriptDefinitions()
        //{

        //}

    }
}
