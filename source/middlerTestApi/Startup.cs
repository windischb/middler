using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using middler.Action.Scripting;
using middler.Action.Scripting.Shared;
using middler.Core;

namespace middlerTestApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            
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
                map.On("{**path}", actions => actions.InvokeScript(new ScriptingOptions()
                {
                    Language = ScriptLanguage.Powershell,
                    SourceCode = @"
$h = get-service | convertto-json -depth 1 | convertfrom-json
$m.Response.SetBody($h)
"
                }));
            });
        }
    }
}
