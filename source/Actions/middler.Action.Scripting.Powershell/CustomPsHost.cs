using System;
using System.Management.Automation.Host;
using System.Threading;

namespace middler.Action.Scripting.Powershell
{
    internal class CustomPsHost : PSHost
    {
        private Guid _hostId = Guid.NewGuid();
        private CustomPSHostUserInterface _ui = null;
        private bool logToConsole = false;

        
        public CustomPsHost(bool logToConsole = false)
        {
            
            this.logToConsole = logToConsole;
            _ui = new CustomPSHostUserInterface(this.logToConsole);
        }

        public override System.Globalization.CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override System.Globalization.CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override Guid InstanceId
        {
            get { return _hostId; }
        }

        public override string Name
        {
            get { return "CustomPSHost"; }
        }

        public override void NotifyBeginApplication()
        {
            return;
        }

        public override void NotifyEndApplication()
        {
            return;
        }

        public override void SetShouldExit(int exitCode)
        {
            return;
        }

        public override PSHostUserInterface UI
        {
            get
            {
                return _ui;
            }
        }

        public override Version Version
        {
            get { return new Version(1, 0); }
        }



    }
}
