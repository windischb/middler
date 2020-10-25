using System.Collections.Generic;
using middler.Common.SharedModels.Enums;
using middler.Common.SharedModels.Models;

namespace middler.Core.Models
{
    public class MiddlerRuleMatch
    {
        internal AccessMode AccessMode { get; set; }
        internal MiddlerRule MiddlerRule { get; set; }
        //internal Dictionary<string, object> RouteData { get; set; }
        internal List<MiddlerRule> RemainingEndpointInfos { get; set; }

    }
}
