using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Utf8Json;

namespace middler.Scripting.ExtensionMethods
{
    public static class ObjectExtensions
    {
        
        public static ExpandoObject ToExpandoObject(this object @object)
        {
            return JsonSerializer.Deserialize<ExpandoObject>(JsonSerializer.Serialize(@object));
        }

        public static ExpandoObject ToExpandoObject(this HttpHeaders headers)
        {
            return headers.ToDictionary(h => h.Key, h => String.Join("; ", h.Value)).ToExpandoObject();
        }
    }
}
