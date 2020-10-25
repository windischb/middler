using System;
using System.Collections.Generic;
using System.Linq;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Models;

namespace middler.Core.Map {
    public class MiddlerMap : IMiddlerMap {

        private List<MapItem> MapItems { get; } = new List<MapItem>();

        public List<MiddlerRule> GetFlatList(IServiceProvider serviceProvider) => MapItems.SelectMany(item => item.GetRules(serviceProvider)).ToList();



        public void AddRule(params MiddlerRule[] middlerRules) {
            lock (mapLock) {
                foreach (var middlerRule in middlerRules)
                {
                    MapItems.Add(MapItem.FromRule(middlerRule));
                }
            }
        }

        private object mapLock = new object();
        public void AddRepo<T>() {
            lock (mapLock) {
                MapItems.Add(MapItem.FromRepo<T>());
            }
        }

        public void AddNamedRepo(string name) {
            lock (mapLock) {
                MapItems.Add(MapItem.FromNamedRepo(name));
            }
        }
    }
}
