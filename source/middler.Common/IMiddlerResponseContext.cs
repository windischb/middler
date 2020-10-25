using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using middler.Common.SharedModels.Models;

namespace middler.Common
{
    public interface IMiddlerResponseContext
    {
        int StatusCode { get; set; }
        SimpleDictionary<string> Headers { get; set; }

        string GetBodyAsString();
        void SetBody(object body);
    }
}
