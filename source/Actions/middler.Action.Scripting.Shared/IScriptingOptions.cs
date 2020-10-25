namespace middler.Action.Scripting.Shared
{
    public interface IScriptingOptions
    {
        ScriptLanguage Language { get; set; }
        string SourceCode { get; set; }
        //string CompiledCode { get; set; }
    }
}