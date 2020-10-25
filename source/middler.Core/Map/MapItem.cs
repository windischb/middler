using System;
using middler.Common.SharedModels.Models;

namespace middler.Core.Map
{
    public class MapItem {
        public MapItemType ItemType { get; private set; }

        public Type RepoType { get; private set; }

        public string RepoName { get; private set; }

        public MiddlerRule Rule { get; private set; }

        public static MapItem FromRule(MiddlerRule rule) {
            var mi = new MapItem();
            mi.ItemType = MapItemType.Rule;
            mi.Rule = rule;
            return mi;
        }

        public static MapItem FromRepo(Type repoType, string name) {
            var mi = new MapItem();
            mi.ItemType = MapItemType.Repo;
            mi.RepoType = repoType;
            mi.RepoName = name;
            return mi;
        }

        public static MapItem FromNamedRepo(string name) {
            var mi = new MapItem();
            mi.ItemType = MapItemType.NamedRepo;
            mi.RepoName = name;
            return mi;
        }

        public static MapItem FromRepo<T>() {
            return FromRepo(typeof(T), null);
        }

    }
}