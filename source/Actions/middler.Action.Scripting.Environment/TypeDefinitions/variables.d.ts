

declare namespace Middler.Scripting.Variables {

    export class VariableCommand {

        GetVariable(path: string): any;
        GetAny(path: string): any;
        GetString(path: string): string;
        GetNumber(path: string): number;
        GetBoolean(path: string): boolean;
        GetCredential(path: string): Middler.Variables.HelperClasses.SimpleCredentials;

    }
}