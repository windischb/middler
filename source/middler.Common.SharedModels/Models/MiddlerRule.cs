using System;
using System.Collections.Generic;

namespace middler.Common.SharedModels.Models
{
    public class MiddlerRule
    {
        public string Name { get; set; }
        public List<string> Scheme { get; set; } = new List<string>();
        public string Hostname { get; set; }
        public string Path { get; set; }
        public List<string> HttpMethods { get; set; } = new List<string>();
        public List<MiddlerRulePermission> Permissions { get; set; } = new List<MiddlerRulePermission>();
        public List<MiddlerAction> Actions { get; set; } = new List<MiddlerAction>();
        
    }

    
}
