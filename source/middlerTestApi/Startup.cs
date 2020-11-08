using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using middler.Action.Scripting;
using middler.Core;
using middler.TaskHelper.ScripterModule;
using middler.Variables.ScripterModule;
using Reflectensions;
using Scripter;
using Scripter.Engine.JavaScript;
using Scripter.Engine.PowerShellCore;
using Scripter.Engine.PowerShellCore.JsonConverter;
using Scripter.Engine.TypeScript;
using Scripter.Modules.Default;

namespace middlerTestApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            Json.Converter.RegisterJsonConverter<PSObjectJsonConverter>();
            

            services.AddScripter(options =>
            {
                options
                    .AddJavaScriptEngine()
                    .AddPowerShellCoreEngine()
                    .AddTypeScriptEngine()
                    .AddDefaultScripterModules()
                    .AddScripterModule<VariablesModule>()
                    .AddScripterModule<TaskHelperModule>();
            });

            services.AddMiddler(options =>
                options
                    .AddScriptingAction()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddler(map =>
            {
                map.On("ps", actions => actions.InvokeScript(new ScriptingOptions()
                {
                    Language ="PowerShellCore",
                    SourceCode = @"
$con = Require Console
$http = Require Http

$httpclient = $http.Client('https://httpbin.org/anything?language=ps');

$result = $httpclient.Get().Content.AsText();

$con.WriteLine()
$con.WriteLine($result)

$con.WriteLine();
$con.WriteLine(($PSVersionTable | out-string))

$context.Response.SetBody(
    @{
        Request = $context.Request; 
        Version = $PsVersionTable
    }
)

"
                }));

                map.On("js", actions => actions.InvokeScript(new ScriptingOptions()
                {
                    Language = "JavaScript",
                    SourceCode = @"
var con = require('Console');
con.Write('Hello ');
con.WriteLine('World!');

var http = require('Http');

var httpclient = http.Client('https://httpbin.org/anything?language=js');

var result = httpclient.Get().Content.AsText();

con.WriteLine();
con.WriteLine(result);

context.Response.SetBody(context.Request)
"
                }));

                map.On("ts", actions => actions.InvokeScript(new ScriptingOptions()
                {
                    Language = "TypeScript",
                    SourceCode = @"
import * as http from 'Http';
import * as variables from 'Variables';


var cl = http.Client('http://10.0.0.21:8123/api/states/switch.buero_fan_buero_ventilator')
                    .SetBearerToken('eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI5M2VmNDdiNDc4ODg0MmI1YjFkYzM0OThjNjM0MWRiNyIsImlhdCI6MTYwMzkxNTA3OCwiZXhwIjoxOTE5Mjc1MDc4fQ.vTY4JseQEpmOkJw1UOkTWiyjALuewgtUR7HvaEqglKA'));

                    var resp = cl.Get().Content.AsObject();

                    context.Response.SetBody(resp);

                "
                }));

            });
        }
    }
}
