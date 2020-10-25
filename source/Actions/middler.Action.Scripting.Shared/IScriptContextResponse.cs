using middler.Common.SharedModels.Models;

namespace middler.Action.Scripting.Shared
{
    public interface IScriptContextResponse
    {
        int StatusCode { get; set; }
        SimpleDictionary<string> Headers { get; set; }

        string GetBodyAsString();
        void SetBody(object body);
    }
}