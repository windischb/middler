using System;
using System.Linq;
using middler.Action.Scripting.Shared;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.PowerShell.Commands;

namespace middler.Action.Scripting.Powershell
{
    public class PowershellEngine: IScriptEngine
    {
        public Func<string, string> CompileScript => null;


        private PsRunspace _psEngine;


        public PowershellEngine()
        {
            _psEngine = new PsRunspace();
        }
        
        public void Initialize()
        {
           
        }

        public void Stop()
        {
            
            _psEngine.Stop();
        }

        public void SetValue(string name, object value)
        {
            _psEngine.SetVariable(name, value);
        }

        public string GetValue(string name)
        {
            var value = _psEngine.GetVariable(name);
            return Converter.Json.ToJson(value);
        }

        public Task Execute(string script)
        {
            _psEngine.Invoke(script);
            return Task.CompletedTask;
        }

        public string Invoke(string script)
        {
            
            var results = _psEngine.Invoke(script).ToList();


            if (results.Count == 0)
            {
                return null;
            }

            if (results.Count == 1)
            {
                return Converter.Json.ToJson(results[0]);
            }

            return Converter.Json.ToJson(results);

        }

        public void Dispose()
        {
            _psEngine?.Dispose();
        }
    }
}
