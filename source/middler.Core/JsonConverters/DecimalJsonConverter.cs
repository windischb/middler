using System;
using Newtonsoft.Json;

namespace middler.Core.JsonConverters
{
    public class DecimalJsonConverter : JsonConverter {

        public override bool CanRead => false;

       

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(decimal) || objectType == typeof(float) || objectType == typeof(double));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(IsWholeValue(value)
                ? JsonConvert.ToString(Convert.ToInt64(value))
                : JsonConvert.ToString(value));
        }

        private static bool IsWholeValue(object value) {
            if (value is decimal decimalValue) {
                int precision = (decimal.GetBits(decimalValue)[3] >> 16) & 0x000000FF;
                return precision == 0;
            } else if (value is float || value is double) {
                var doubleValue = Convert.ToDouble(value);
                return doubleValue == Math.Truncate(doubleValue);
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }

}
