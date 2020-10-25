using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;
using NamedServices.Microsoft.Extensions.DependencyInjection;

namespace middler.Core.Map
{
    public static class MapItemExtensions {

        public static IEnumerable<MiddlerRule> GetRules(this MapItem item, IServiceProvider serviceProvider) {

            switch (item.ItemType) {
                case MapItemType.NamedRepo: {
                    var repo = serviceProvider.GetRequiredNamedService<IMiddlerRepository>(item.RepoName);
                    return repo.ProvideRules();
                }
                case MapItemType.Repo: {
                    var repo = (IMiddlerRepository)serviceProvider.GetRequiredService(item.RepoType);
                    return repo.ProvideRules();
                }
                case MapItemType.Rule: {
                    return new List<MiddlerRule>() { item.Rule };
                }
            }

            return ArraySegment<MiddlerRule>.Empty;
        }
    }
}