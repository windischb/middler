using middler.Common.SharedModels.Interfaces;
using middler.Scripting.HttpCommand;
using middler.Scripting.SmtpCommand;
using middler.Scripting.TemplateCommand;
using middler.Scripting.Variables;

namespace middler.Scripting
{
    public class Environment
    {
        public Http Http => new Http();
        public TaskHelper Task => new TaskHelper();

        public VariableCommand Variables;

        public Smtp Smtp => new Smtp();

        public MTemplate Template => new MTemplate();

        public Environment(IVariablesRepository variableStore)
        {
            Variables = new VariableCommand(variableStore);    
        }
    }
}
