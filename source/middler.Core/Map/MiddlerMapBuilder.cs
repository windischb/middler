using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using middler.Common.Interfaces;
using middler.Common.SharedModels.Enums;
using middler.Common.SharedModels.Models;

namespace middler.Core.Map {
    public class MiddlerMapBuilder {

        private IServiceProvider ServiceProvider { get; }
        private IMiddlerMap Map { get; }

        public MiddlerMapBuilder(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider;
            Map = serviceProvider.GetRequiredService<IMiddlerMap>();
        }

        public MiddlerMapBuilder On(string path, Action<IMiddlerMapActionsBuilder> actions, Action<MiddlerMapRuleEnhancer> options = null) {
            return On(null, "*", path, actions, options);
        }

        public MiddlerMapBuilder On(string hostname, string path, Action<IMiddlerMapActionsBuilder> actions, Action<MiddlerMapRuleEnhancer> options = null) {
            return On(null, hostname, path, actions, options);
        }

        public MiddlerMapBuilder On(string scheme, string hostname, string path, Action<IMiddlerMapActionsBuilder> actions, Action<MiddlerMapRuleEnhancer> options = null) {
            var rule = new MiddlerRule();
            rule.Scheme = scheme is null ? new List<string>() : new List<string>() { scheme };
            rule.Path = path;
            rule.Hostname = hostname;

            var actionBuilder = new MiddlerMapActionsBuilder(ServiceProvider);
            actions.Invoke(actionBuilder);

            rule.Actions = actionBuilder.MiddlerActions;

            if (options != null) {
                var enhancer = new MiddlerMapRuleEnhancer(rule);
                options.Invoke(enhancer);
                rule = enhancer;
            }


            Map.AddRule(rule);
            return this;

        }


        public MiddlerMapBuilder AddRepo<T>() where T : IMiddlerRepository {
            Map.AddRepo<T>();
            return this;
        }

        public MiddlerMapBuilder AddNamedRepo(string named) {
            Map.AddNamedRepo(named);
            return this;
        }

        public IMiddlerMap Build() {
            return Map;
        }

    }

    public class MiddlerMapRuleEnhancer {
        private MiddlerRule MiddlerRule { get; set; }


        internal MiddlerMapRuleEnhancer(MiddlerRule rule) {
            MiddlerRule = rule;
        }

        public MiddlerMapRuleEnhancer AllowHttpMethods(params string[] method) {
            MiddlerRule.HttpMethods = method.ToList();
            return this;
        }

        public MiddlerMapRuleEnhancer AllowScheme(params string[] scheme) {
            MiddlerRule.Scheme = scheme.ToList();
            return this;
        }

        public MiddlerMapRuleEnhancer AllowAnyScheme(params string[] scheme) {
            MiddlerRule.Scheme = new List<string>() { "*" };
            return this;
        }


        public MiddlerMapRuleEnhancer AddPermission(string principalName, PrincipalType type, AccessMode accessMode,
            string client = null, string sourceAddress = null) {
            var entry = new MiddlerRulePermission();
            entry.PrincipalName = principalName;
            entry.Type = type;
            entry.AccessMode = accessMode;
            entry.Client = client;
            entry.SourceAddress = sourceAddress;
            MiddlerRule.Permissions.Add(entry);
            return this;
        }

        public MiddlerMapRuleEnhancer AllowEveryone() {
            return AddPermission("*", PrincipalType.Everyone, AccessMode.Allow);
        }

        public MiddlerMapRuleEnhancer AllowAuthenticated() {
            return AddPermission("*", PrincipalType.Authenticated, AccessMode.Allow);
        }


        public static implicit operator MiddlerRule(MiddlerMapRuleEnhancer enhancer) {
            return enhancer.MiddlerRule;
        }

    }
}
