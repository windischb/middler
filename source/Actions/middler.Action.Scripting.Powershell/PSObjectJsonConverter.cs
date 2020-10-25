using System;
using System.Management.Automation;
using Newtonsoft.Json;

namespace middler.Action.Scripting.Powershell
{
    public class PSObjectJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PSObject).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var psObj = (PSObject)value;

            object obj = null;
            if (psObj.BaseObject is PSCustomObject)
            {
                writer.WriteStartObject();
                foreach (var prop in psObj.Properties)
                {
                    if (!prop.IsGettable)
                        continue;

                    writer.WritePropertyName(prop.Name);
                    serializer.Serialize(writer, prop.Value);
                }
                writer.WriteEndObject();
            }
            else
            {
                obj = psObj.BaseObject;
                serializer.Serialize(writer, obj);
               
            }

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get { return false; } }
    }
}
