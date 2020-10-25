using System;
using System.Threading.Tasks;

namespace middler.Action.Scripting.Shared
{
    public interface IScriptEngine: IDisposable
    {

        Func<string, string> CompileScript { get; }

        bool NeedsCompiledScript => this.CompileScript != null;

        void Initialize();

        void SetValue(string name, object value);
        string GetValue(string name);

        Task Execute(string script);

        string Invoke(string script);

        void Stop();
    }
}
