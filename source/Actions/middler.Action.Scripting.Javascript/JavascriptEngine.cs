using System;
using System.Threading.Tasks;
using Esprima;
using Esprima.Ast;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using Jint.Runtime.Interop;
using middler.Action.Scripting.Shared;

namespace middler.Action.Scripting.Javascript
{
    public class JavascriptEngine: IScriptEngine
    {
        public Func<string, string> CompileScript => null;

        private Engine _engine;
        public const string StopExecutionIdentifier = "e06e73c8-67ec-411c-9761-c2f3b063f436";

        public static ParserOptions EsprimaOptions = new ParserOptions {
            Tolerant = true
        };

        private bool managedExit = false;


        public JavascriptEngine()
        {
            _engine = new Engine(ConfigureOptions);

           

        }

        

        public void Initialize()
        {
            _engine.Execute($"function exit() {{ setManagedExit(true); throw \"{StopExecutionIdentifier}\";}}", EsprimaOptions);
            _engine.Execute("var exports = {};", EsprimaOptions);
            
            void SetManagedExit(bool value) {
                managedExit = value;
            }


            _engine.SetValue("setManagedExit", new Action<bool>(SetManagedExit));
            managedExit = false;
            _engine.SetValue("managedExit", managedExit);
            _engine.Global.FastAddProperty("middler", new NamespaceReference(_engine, "middler"), false, false, false );
            //_engine.Execute("var middler = importNamespace('middler')");
            _engine.Step += EngineOnStep;


        }



        private StepMode EngineOnStep(object sender, DebugInformation e)
        {
            if (managedExit)
            {
                throw new OperationCanceledException();
            }
            return StepMode.Over;
        }

        private void ConfigureOptions(Options options) {

            options.CatchClrExceptions();
            options.DebugMode();
            options.AllowClr(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Stop()
        {
            managedExit = true;
        }


        public void SetValue(string name, object value)
        {
            switch (value) {
                case string str:
                    _engine.SetValue(name, str);
                    break;
                case double dbl:
                    _engine.SetValue(name, dbl);
                    break;
                case bool _bool:
                    _engine.SetValue(name, _bool);
                    break;
                default:
                    var obj = JsValue.FromObject(_engine, value);
                    _engine.SetValue(name, obj);
                    break;
            }

        }

        public string GetValue(string name)
        {
            var value = _engine.GetValue(name);


            var json = Convert(value);
            //var json = Converter.Json.ToJson(value.ToObject());

            return json;
        }

        private string Convert(JsValue jsValue)
        {

            if (jsValue.IsArray())
            {
                var arr = jsValue.AsArray();
                return Converter.Json.ToJson(arr);
            }

            if(jsValue.IsObject())
            {
                return Converter.Json.ToJson(jsValue.ToObject());
            }


            return Converter.Json.ToJson(jsValue);

        }
        

        public Task Execute(string script)
        {
            return Task.Run(() => InternalExecute(script, false));

        }

        public string Invoke(string script)
        {
            var result = InternalExecute(script, true);

            var stringObject = _engine.Json.Stringify(null, Jint.Runtime.Arguments.From(result)).AsString();

            return stringObject;

        }


        private JsValue InternalExecute(string script, bool returnValue)
        {
            managedExit = false;
            if (String.IsNullOrWhiteSpace(script))
                return null;


            try
            {
                _engine.Execute(script, EsprimaOptions);
                return returnValue ? _engine.GetCompletionValue() : null;
                //_engine.Execute(javaScript);
            } catch (Exception exception) {
                if (!managedExit) {
                    var ex = exception.GetBaseException();
                    if (ex is JavaScriptException jsex) {
                        if (jsex.Message != StopExecutionIdentifier) {
                            throw;
                        }
                    } else {
                        throw;
                    }
                }


            }

            return null;
        }

        public void Dispose()
        {
            _engine = null;
        }
    }
}
