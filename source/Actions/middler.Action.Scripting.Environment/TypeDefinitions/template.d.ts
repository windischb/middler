declare namespace Middler.Scripting.TemplateCommand {


    export class MTemplate {

        Parse(template: string, data: {}):string;
        Parse(template: string, ...data: Array<{}>): string;
    }
}