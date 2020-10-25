using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Native;
using Jint.Runtime;
using Newtonsoft.Json;

namespace BMIBenutzerVerwaltung
{
    public class JsValueJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(JsValue).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            //var jsValue = (JsValue) value;

            //switch (jsValue.Type)
            //{
            //    case Types.Object:
            //    {
            //        var obj = jsValue.AsObject();
            //        var props = obj.GetOwnProperties();
            //        foreach (var prop in props)
            //        {
            //            var name = prop.Key;
            //            var value = prop.Value.Value;
            //        }
            //    }
            //}

            //var psObj = (PSObject)value;

            //object obj = null;
            //if (psObj.BaseObject is PSCustomObject)
            //{
            //    writer.WriteStartObject();
            //    foreach (var prop in psObj.Properties)
            //    {
            //        if (!prop.IsGettable)
            //            continue;

            //        writer.WritePropertyName(prop.Name);
            //        serializer.Serialize(writer, prop.Value);
            //    }
            //    writer.WriteEndObject();
            //}
            //else
            //{
            //    obj = psObj.BaseObject;
            //    serializer.Serialize(writer, obj);
               
            //}

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get { return false; } }
    }
}
