using System;
using System.Collections.Generic;
using middler.Common.SharedModels.Interfaces;
using Newtonsoft.Json.Linq;

namespace middler.Common.SharedModels.Models {
    public class MiddlerAction: IMiddlerAction {

        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual bool Terminating { get; set; } = false;
        public virtual bool WriteStreamDirect { get; set; } = false;
        public virtual bool Enabled { get; set; } = true;
        public string ActionType { get; set; }

        public JObject Parameters { get; set; } = new JObject();


    }

    public abstract class MiddlerAction<T> : IMiddlerAction where T : class, new() {
       

        public virtual bool Terminating { get; set; } = false;
        public virtual bool WriteStreamDirect { get; set; } = false;
        public virtual bool Enabled { get; set; } = true;
        public abstract string ActionType { get; }


        public T Parameters { get; set; } = new T();

    }

}
