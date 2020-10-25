using System.Collections.Generic;
using System.Linq;

namespace middler.Common.SharedModels.Models
{
    public class MiddlerRouteQueryParameters : SimpleDictionary<string> {

        private List<RuleQueryParameter> QueryParameters { get; } = new List<RuleQueryParameter>();

        
        public MiddlerRouteQueryParameters(IDictionary<string, string> dict) : base(dict) {


        }

        public MiddlerRouteQueryParameters(IDictionary<string, string> dict, List<RuleQueryParameter> queryParameters) : this(dict) {
            QueryParameters = queryParameters;
        }

        public SimpleDictionary<object> All() {
            var all = this
                .Select(kvp => new KeyValuePair<string, object>(kvp.Key, GetValue(kvp.Key)));
            return new SimpleDictionary<object>(all);
        }

        public SimpleDictionary<object> Others() {
            var definedParams = QueryParameters.Select(q => q.Name);
            var others = this
                .Where(kvp => !definedParams.Contains(kvp.Key))
                .Select(kvp => new KeyValuePair<string, object>(kvp.Key, GetValue(kvp.Key)));
            return new SimpleDictionary<object>(others);
        }


        public new object this[string key] => GetValue(key);


        private object GetValue(string key)
        {

            if (TryGetValue(key, out var val)) {
                if (val == null) {
                    return null;
                }
                var isDefined = QueryParameters.FirstOrDefault(q => q.Name == key);
                if (isDefined != null) {
                    if (isDefined.IsArray) {
                        return val.Split(',');
                    }
                    return val;
                }
                return val;
            }

            return null;

        }
    }
}