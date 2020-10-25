using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;

namespace middler.Scripting.TemplateCommand
{
    public class MTemplate
    {


        public string Parse(string template, object data)
        {
            return Parse(template, new List<object> {data});
        }

        public string Parse(string template, params object[] data)
        {
            return Parse(template, data.ToList());
        }
        //public string Parse(string template, object[] data)
        //{
        //    return Parse(template, data.ToList());
        //}

        private string Parse(string template, IEnumerable<object> data)
        {
            JObject jobject = new JObject();
            data.Aggregate(jobject, (a, b) => {
                var json = Converter.Json.ToJson(b);
                var jo = Converter.Json.ToJObject(json);
                return Converter.Json.Merge(a, jo);
            });

            var dict = Converter.Json.ToDictionary(jobject);

            return ParseScriptObject(template, dict);
        }

        private string ParseScriptObject(string template, Dictionary<string, object> data)
        {
            var scriptObj = new ScriptObject(StringComparer.OrdinalIgnoreCase);
            scriptObj.Import(data, renamer: member => member.Name);
            var scribanTemplate = Template.Parse(template);
            return scribanTemplate.Render(scriptObj);
        }

    }
}
