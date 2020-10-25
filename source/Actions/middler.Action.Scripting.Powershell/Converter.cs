using System;

namespace middler.Action.Scripting.Powershell
{
    public class Converter
    {
        private static readonly Lazy<Reflectensions.Json> lazyJson = new Lazy<Reflectensions.Json>(() => new Reflectensions.Json()
            .RegisterJsonConverter<PSObjectJsonConverter>()
        );

        public static Reflectensions.Json Json => lazyJson.Value;

    }
}
