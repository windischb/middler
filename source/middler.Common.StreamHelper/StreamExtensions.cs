using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace middler.Common.StreamHelper
{
    public static class StreamExtensions
    {

        public static T AsJsonToObject<T>(this Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default;

            using var sr = new StreamReader(stream);
            using var jtr = new JsonTextReader(sr);
            var js = new JsonSerializer();
            var searchResult = js.Deserialize<T>(jtr);
            return searchResult;
        }

        public static void WriteAllText(this Stream stream, string text)
        {
            using var streamWriter = new StreamWriter(stream, Encoding.UTF8, 81920, true);
            streamWriter.Write(text);
            streamWriter.Flush();
            streamWriter.Close();
        }
    }
}
