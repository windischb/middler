using System;
using System.Linq;
using middler.Common.SharedModels.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Scripting.Variables
{
    public class VariableCommand
    {
        private readonly IVariablesRepository _variablesStore;

        public VariableCommand(IVariablesRepository variablesStore)
        {
            _variablesStore = variablesStore;
        }

        public ITreeNode GetVariable(string path)
        {
            path = path.Replace(".", "/");

            string parent = null;
            string name = path;
            if (path.Contains("/"))
            {
                var parts = path.Split('/');
                parent = String.Join('/', parts.Take(parts.Length - 1));
                name = parts.Last();
            }

            return _variablesStore.GetVariable(parent, name);
        }

        public T GetVariableContent<T>(string path)
        {
            var variable = this.GetVariable(path);
            return Converter.Json.ToObject<T>(variable.Content);
        }

        public object GetAny(string path)
        {
            var variable = this.GetVariable(path);
            return variable.Content;
        }

        public string GetString(string path)
        {
            var variable = this.GetVariable($"{path}");
            return (string)variable.Content;
        }

        public decimal GetNumber(string path)
        {
            var variable = this.GetVariable($"{path}");
            return (decimal)variable.Content;
        }

        public bool GetBoolean(string path)
        {
            var variable = this.GetVariable($"{path}");
            return (bool)variable.Content;
        }

        public SimpleCredentials GetCredential(string path)
        {
            return GetVariableContent<SimpleCredentials>($"{path}.credential");
        }
    }
}
