using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace middler.Action.Scripting.Powershell
{
    internal class PsRunspace: IDisposable
    {
        private CustomPsHost PsHost { get; }
        private Runspace Runspace { get; }

        public PsRunspace()
        {

            var config = InitialSessionState.CreateDefault();
            
            
            PsHost = new CustomPsHost();
            
            Runspace = RunspaceFactory.CreateRunspace(PsHost, config);
            
            Runspace.DefaultRunspace = Runspace;
            Runspace.Open();
        }

        public PsRunspace SetVariable(string name, object value)
        {
            Runspace.SessionStateProxy.SetVariable(name, value);
            return this;
        }

        public object GetVariable(string name)
        {
            return Runspace.SessionStateProxy.GetVariable(name);
        }


        private object _psLock = new object();

        private Pipeline _pipeline;
        public IEnumerable<PSObject> Invoke(string command)
        {
            lock (_psLock)
            {

                try
                {
                    
                    _pipeline = Runspace.CreatePipeline();
                    _pipeline.Commands.AddScript(command);
                
                    var ret = _pipeline.Invoke();
                    var errorList = new List<string>();
                    if (_pipeline.Error.Count > 0)
                    {
                        while (!_pipeline.Error.EndOfPipeline)
                        {
                            if (_pipeline.Error.Read() is PSObject value)
                            {
                                if (value.BaseObject is ErrorRecord r)
                                {
                                    errorList.Add(r.Exception.Message);
                                }
                            }
                        }
                    }

                    if (errorList.Any())
                    {
                        throw new Exception(String.Join(Environment.NewLine, errorList));
                    }


                    return ret;
                }
                catch (Exception)
                {
                   
                    throw;
                }
                
            }
            
        }
        
        public void Stop()
        {
            _pipeline?.StopAsync();
        }

        public void Dispose()
        {
            _pipeline?.Dispose();
            Runspace?.Dispose();
        }
    }
}
