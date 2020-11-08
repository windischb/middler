using middler.Common.SharedModels.Attributes;

namespace middler.Action.Scripting
{
    public class ScriptingOptions 
    {
        public string Language { get; set; }

        public string SourceCode { get; set; }

        [Internal]
        public string CompiledCode { get; set; }
    }
}
