using middler.Common.Interfaces;

namespace middler.Action.Scripting
{
    public static class ScriptingExtensions
    {

        public static void InvokeScript(this IMiddlerMapActionsBuilder builder, ScriptingOptions options)
        {
            builder.AddAction<ScriptingAction, ScriptingOptions>(options);
        }
        

    }
}