using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace middler.Scripting.HttpCommand
{
    internal class JsonHttpContent : HttpContent
    {
        public object SerializationTarget { get; }
        public JsonHttpContent(object serializationTarget)
        {
            SerializationTarget = serializationTarget;
            this.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            await using var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);

            await JsonSerializer.SerializeAsync(stream, SerializationTarget);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
