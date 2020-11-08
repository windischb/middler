declare const m: Middler.Scripting.Environment;

declare namespace Middler.Scripting {

    export class Environment {
        Http: Middler.Scripting.HttpCommand.Http;
        Task: Middler.Scripting.TaskHelper;
        Variables: Middler.Scripting.Variables.VariableCommand;
        Smtp: Middler.Scripting.SmtpCommand.Smtp;
        Template: Middler.Scripting.TemplateCommand.MTemplate;
    }
}