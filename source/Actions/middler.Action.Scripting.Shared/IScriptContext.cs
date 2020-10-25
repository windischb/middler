using System.Collections.Generic;
using System.Text;

namespace middler.Action.Scripting.Shared
{
    public interface IScriptContext
    {

        IScriptContextResponse Response { get; }

        IScriptContextRequest Request { get; }

    }
}
