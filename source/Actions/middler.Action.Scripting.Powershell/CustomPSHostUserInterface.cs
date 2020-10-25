using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text;

namespace middler.Action.Scripting.Powershell
{
    internal class CustomPSHostUserInterface : PSHostUserInterface
    {

        private StringBuilder _output;
        private List<String> _errors;
        private bool _hasError;
        private bool logToConsole;

        public CustomPSHostUserInterface(Boolean logToConsole = false)
        {
            _output = new StringBuilder();
            _errors = new List<string>();
            this.logToConsole = logToConsole;
        }

        private CustomPSHostRawUserInterface rawUi = new CustomPSHostRawUserInterface();

        public override PSHostRawUserInterface RawUI
        {
            get { return this.rawUi; }
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("The method or operation is not implemented - PromptForChoice");
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException("The method or operation is not implemented - PromptForCredential");
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException("The method or operation is not implemented - PromptForCredential");
        }

        public override string ReadLine()
        {
            throw new NotImplementedException("The method or operation is not implemented - ReadLine");
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("The method or operation is not implemented - ReadLineAsSecureString");
        }

        public override void Write(string value)
        {
            _output.Append(value);
            if (logToConsole)
                Console.Write(value);
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            _output.Append(value);
            if (logToConsole)
            {
                var _fore = Console.ForegroundColor;
                var _back = Console.BackgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
                Console.Write(value);
                Console.ForegroundColor = _fore;
                Console.BackgroundColor = _back;
            }
        }

        public override void WriteDebugLine(string message)
        {
            _output.AppendLine("DEBUG: " + message);
            if (logToConsole)
            {
                var _fore = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("DEBUG: " + message);
                Console.ForegroundColor = _fore;
            }
        }

        public override void WriteErrorLine(string message)
        {
            _hasError = true;
            _errors.Add(message);
            _output.AppendLine("ERROR: " + message);
            if (logToConsole)
            {
                var _fore = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + message);
                Console.ForegroundColor = _fore;
            }
        }

        public override void WriteLine()
        {
            _output.AppendLine();
            if (logToConsole)
                Console.WriteLine();
        }

        public override void WriteLine(string value)
        {
            _output.AppendLine(value);
            if (logToConsole)
                Console.WriteLine(value);
        }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            _output.AppendLine(value);
            if (logToConsole)
            {
                var _fore = Console.ForegroundColor;
                var _back = Console.BackgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
                Console.WriteLine(value);
                Console.ForegroundColor = _fore;
                Console.BackgroundColor = _back;
            }
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        public override void WriteVerboseLine(string message)
        {
            _output.AppendLine("VERBOSE: " + message);
            if (logToConsole)
            {
                var _fore = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("VERBOSE: " + message);
                Console.ForegroundColor = _fore;
            }
        }

        public override void WriteWarningLine(string message)
        {
            _output.AppendLine("WARNING: " + message);
            if (logToConsole)
            {
                var _fore = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: " + message);
                Console.ForegroundColor = _fore;
            }
        }

        public string Output
        {
            get
            {
                return _output.ToString();
            }
        }

        public List<String> Errors
        {
            get
            {
                return _errors;
            }
        }

        public bool HasError
        {
            get
            {
                return _hasError;
            }
        }
    }
}