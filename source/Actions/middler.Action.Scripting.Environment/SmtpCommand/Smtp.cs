using System;
using System.Net;
using System.Threading.Tasks;
using MailKit.Security;
using middler.Common.SharedModels.Models;

namespace middler.Scripting.SmtpCommand
{
    public class Smtp
    {

        public MSmtpClient Client()
        {
            return new MSmtpClient();
        }

        public MMailMessage CreateMessage()
        {
            return new MMailMessage();
        }

    }

    public class MSmtpClient
    {
        private MSmtpClientOptions options { get; } = new MSmtpClientOptions();

        public MSmtpClient UseSmtpServer(string smtpServer)
        {
            options.SMTPServer = smtpServer;
            return this;
        }

        public MSmtpClient UseSmtpServerPort(int port)
        {
            options.SMTPServerPort = port;
            return this;
        }

        public MSmtpClient UseSSL(bool value)
        {
            options.UseSSL = value;
            return this;
        }

        public MSmtpClient UseBasicAuthentication(string username, string password)
        {
            if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
                options.Credentials = new NetworkCredential(username, password);

            return this;
        }

        public MSmtpClient UseBasicAuthentication(SimpleCredentials credentials)
        {
            options.Credentials = credentials;

            return this;
        }

        public MSmtpClient IgnoreSSlError(bool value)
        {
            options.IgnoreSSLError = value;
            return this;
        }


        public void SendMessage(MMailMessage message)
        {
            var smtpClient = new MailKit.Net.Smtp.SmtpClient();


            if (options.IgnoreSSLError)
            {
                smtpClient.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }

            SecureSocketOptions secOpts = SecureSocketOptions.Auto;
            if (!options.UseSSL)
            {
                secOpts = SecureSocketOptions.None;
            }

            smtpClient.Connect(options.SMTPServer, options.SMTPServerPort, secOpts);


            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

            if (options.Credentials != null)
                smtpClient.Authenticate(options.Credentials);

            
            smtpClient.Send(message);
            smtpClient.Disconnect(true);


        }

        public async Task SendMessageAsync(MMailMessage message)
        {
            var smtpClient = new MailKit.Net.Smtp.SmtpClient();


            if (options.IgnoreSSLError)
            {
                smtpClient.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }

            SecureSocketOptions secOpts = SecureSocketOptions.Auto;
            if (!options.UseSSL)
            {
                secOpts = SecureSocketOptions.None;
            }

            await smtpClient.ConnectAsync(options.SMTPServer, options.SMTPServerPort, secOpts);


            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

            if (options.Credentials != null)
                await smtpClient.AuthenticateAsync(options.Credentials);


            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
            
        }

    }

    public class MSmtpClientOptions
    {
        public string SMTPServer { get; set; }
        public int SMTPServerPort { get; set; } = 25;

        public bool UseSSL { get; set; } = true;
        public bool IgnoreSSLError { get; set; }

        public NetworkCredential Credentials { get; set; }

    }
}
