using System;

namespace middler.Action.Scripting.Javascript
{
    public class Converter
    {
        private static readonly Lazy<Reflectensions.Json> lazyJson = new Lazy<Reflectensions.Json>(() => new Reflectensions.Json()
            //.RegisterJsonConverter<JsValueJsonConverter>()
        );

        public static Reflectensions.Json Json => lazyJson.Value;

    }
}
