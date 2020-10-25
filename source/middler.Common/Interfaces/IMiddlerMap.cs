using System;
using System.Collections.Generic;
using middler.Common.SharedModels.Models;

namespace middler.Common.Interfaces
{
    public interface IMiddlerMap
    {
        List<MiddlerRule> GetFlatList(IServiceProvider serviceProvider);
        void AddRule(params MiddlerRule[] middlerRules);
        void AddRepo<T>();
        void AddNamedRepo(string name);
    }
}
