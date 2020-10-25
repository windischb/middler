using System;

namespace middler.Action.Scripting
{
    public class Converter
    {
        private static readonly Lazy<Reflectensions.Json> lazyJson = new Lazy<Reflectensions.Json>(() => new Reflectensions.Json()

        );

        public static Reflectensions.Json Json => lazyJson.Value;

    }
}
