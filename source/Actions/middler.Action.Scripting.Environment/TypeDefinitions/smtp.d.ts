
declare namespace Middler.Scripting.SmtpCommand {


    export class Smtp {

        Client(): MSmtpClient;
        CreateMessage(): MMailMessage;
    }

    export class MSmtpClient {

        UseSmtpServer(smtpServer: string): MSmtpClient;
        UseSmtpServerPort(port: number): MSmtpClient;
        UseSSL(value: boolean): MSmtpClient;
        UseBasicAuthentication(username: string, password: string): MSmtpClient;
        UseBasicAuthentication(credentials: Middler.Variables.HelperClasses.SimpleCredentials): MSmtpClient;
        IgnoreSSlError(value: boolean): MSmtpClient;
        SendMessage(message: MMailMessage): void;
        SendMessageAsync(message: MMailMessage): Middler.Scripting.Task;

    }

    export class MMailMessage {

        constructor();

        From(address: string): MMailMessage;
        From(name: string, address: string): MMailMessage;

        SetTo(address: string): MMailMessage;
        SetTo(...addresses: string[]): MMailMessage;
        SetTo(name: string, address: string): MMailMessage;

        AddTo(address: string): MMailMessage;
        AddTo(...addresses: string[]): MMailMessage;
        AddTo(name: string, address: string): MMailMessage;


        SetCc(address: string): MMailMessage;
        SetCc(...addresses: string[]): MMailMessage;
        SetCc(name: string, address: string): MMailMessage;

        AddCc(address: string): MMailMessage;
        AddCc(...addresses: string[]): MMailMessage;
        AddCc(name: string, address: string): MMailMessage;


        SetBcc(address: string): MMailMessage;
        SetBcc(...addresses: string[]): MMailMessage;
        SetBcc(name: string, address: string): MMailMessage;

        AddBcc(address: string): MMailMessage;
        AddBcc(...addresses: string[]): MMailMessage;
        AddBcc(name: string, address: string): MMailMessage;

        WithSubject(subject: string): MMailMessage;

        WithTextBody(body: string): MMailMessage;
        WithHtmlBody(body: string): MMailMessage;


    }
}