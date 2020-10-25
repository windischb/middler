using System.Collections.Generic;
using middler.Common.SharedModels.Models;

namespace middler.Common.Interfaces
{
    public interface IMiddlerRepository {
        
        List<MiddlerRule> ProvideRules();
    }
   
}
