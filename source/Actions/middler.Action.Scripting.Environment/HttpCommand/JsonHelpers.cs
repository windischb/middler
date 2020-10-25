using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace middler.Scripting.HttpCommand
{
    public static class JsonHelpers
    {
        public static object ToBasicDotNetObject(JToken jtoken) {
            if (jtoken == null)
                return null;

            switch (jtoken.Type) {
                case JTokenType.None:
                    return null;
                case JTokenType.Object:
                    var obj = jtoken as JObject;
                    return ToBasicDotNetExpando(obj);
                case JTokenType.Array:
                    var arr = jtoken as JArray;
                    return ToBasicDotNetObjectEnumerable(arr);
                case JTokenType.Constructor:
                    return null;
                case JTokenType.Property:
                    return null;
                case JTokenType.Comment:
                    return null;
                case JTokenType.Integer:
                    return jtoken.ToObject<int>();
                case JTokenType.Float:
                    return jtoken.ToObject<float>();
                case JTokenType.String:
                    return jtoken.ToObject<string>();
                case JTokenType.Boolean:
                    return jtoken.ToObject<bool>();
                case JTokenType.Null:
                    return null;
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Date:
                    return jtoken.ToObject<DateTime>();
                case JTokenType.Raw:
                    return null;
                case JTokenType.Bytes:
                    return jtoken.ToObject<Byte[]>();
                case JTokenType.Guid:
                    return jtoken.ToObject<Guid>();
                case JTokenType.Uri:
                    return jtoken.ToObject<Uri>();
                case JTokenType.TimeSpan:
                    return jtoken.ToObject<TimeSpan>();
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public static ExpandoObject ToBasicDotNetExpando(JObject jObject) {
            if (jObject == null)
                return null;

            var exp = new ExpandoObject();
            var dict = exp as IDictionary<string, object>;

            foreach (var kvp in jObject) {
                dict.Add(kvp.Key,
                    kvp.Value.Type == JTokenType.Object
                        ? ToBasicDotNetExpando((JObject)kvp.Value)
                        : ToBasicDotNetObject(kvp.Value));
            }

            return exp;
        }

        public static IEnumerable<object> ToBasicDotNetObjectEnumerable(JArray jArray, bool ignoreErrors = false) {
            return jArray?.Select(ToBasicDotNetObject).ToList();
        }
    }
}
