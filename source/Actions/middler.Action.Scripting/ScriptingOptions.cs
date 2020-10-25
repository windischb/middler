using System;
using System.Collections.Generic;
using System.Text;
using middler.Action.Scripting.Shared;
using middler.Common.SharedModels.Attributes;
using Newtonsoft.Json;

namespace middler.Action.Scripting
{
    public class ScriptingOptions : IScriptingOptions
    {
        public ScriptLanguage Language { get; set; }

        public string SourceCode { get; set; }

        [Internal]
        public string CompiledCode { get; set; }
    }
}
