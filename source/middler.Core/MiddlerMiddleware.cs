using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using middler.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing.Constraints;
using middler.Common;

using middler.Common.ExtensionMethods;
using middler.Common.SharedModels.Enums;
using middler.Common.SharedModels.Interfaces;
using middler.Common.SharedModels.Models;
using middler.Core.ExtensionMethods;
using middler.Core.Models;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;
using middler.Common.StreamHelper;
using middler.Core.Context;
using Type = System.Type;

namespace middler.Core
{
    public class MiddlerMiddleware
    {

        private readonly RequestDelegate _next;
        private ILogger Logger { get; set; }
        private ILogger ConstraintLogger { get; set; }

        public MiddlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IMiddlerOptions middlerOptions, InternalHelper intHelper)
        {
            var sw = new Stopwatch();
            
            sw.Start();
            EnsureLoggers(httpContext);
            //Stream originalBody = null;

            var middlerMap = httpContext.RequestServices.GetRequiredService<IMiddlerMap>();


            var endpoints = middlerMap.GetFlatList(httpContext.RequestServices);

            var executedActions = new Stack<IMiddlerAction>();

            var  middlerContext = new MiddlerContext(httpContext, middlerOptions);

            //var first = true;
            try
            {
                bool terminating;
                do
                {

                    var matchingEndpoint = FindMatchingEndpoint(middlerOptions, endpoints, middlerContext);

                    if (matchingEndpoint == null)
                    {
                        //await _next(httpContext).ConfigureAwait(false);
                        break;
                    }

                    if (matchingEndpoint.AccessMode == AccessMode.Deny)
                    {
                        await httpContext.Forbid().ConfigureAwait(false);
                        return;
                    }

                    //middlerContext.Features.Set(matchingEndpoint);

                    endpoints = matchingEndpoint.RemainingEndpointInfos;

                    //var interMediateStreamNeeded = matchingEndpoint.MiddlerRule.Actions.Any(a => !a.WriteStreamDirect);
                    //if (interMediateStreamNeeded)
                    //{
                    //    originalBody ??= httpContext.Response.Body;
                    //    httpContext.Response.Body = new AutoStream(opts => 
                    //        opts
                    //            .WithMemoryThreshold(middlerOptions.AutoStreamDefaultMemoryThreshold)
                    //            .WithFilePrefix("middler"), middlerContext.Request.RequestAborted);

                    //}

                    

                    terminating = false;
                    foreach (var endpointAction in matchingEndpoint.MiddlerRule.Actions)
                    {
                        if (!endpointAction.Enabled)
                        {
                            continue;
                        }
                        //if (!first)
                        //{
                        //    middlerContext.PrepareNext();
                        //}
                        var action = intHelper.BuildConcreteActionInstance(endpointAction);
                        if (action != null)
                        {
                            
                            await ExecuteRequestAction(action, middlerContext);
                            executedActions.Push(action);
                            terminating = action.Terminating;
                            //first = false;
                        }

                        if (terminating)
                            break;
                    }

                    
                } while (!terminating);


                if (executedActions.Any())
                {
                    while (executedActions.TryPop(out var executedAction))
                    {
                        await ExecuteResponseAction(executedAction, middlerContext);
                    }

                    //httpContext.Response.Body = middlerContext.MiddlerResponseContext.Body;

                    await WriteToAspNetCoreResponseBodyAsync(httpContext, middlerContext).ConfigureAwait(false);
                }
                else
                {
                    await _next(httpContext);
                }
                


            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }



        }


        private async Task ExecuteRequestAction(IMiddlerAction middlerAction, MiddlerContext middlerContext)
        {
            var method = middlerAction.GetType().GetMethod("ExecuteRequestAsync") ?? middlerAction.GetType().GetMethod("ExecuteRequest");
            if (method == null)
            {
                return;
            }

            var man = new Dictionary<Type, object>()
            {
                
            };

            

            var parameters = BuildExecuteMethodParameters(method, middlerContext, man);
            
            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                var t = (Task)method.Invoke(middlerAction, parameters);
                await t.ConfigureAwait(false);
            }
            else
            {
                method.Invoke(middlerAction, parameters);
            }

        }

        private async Task ExecuteResponseAction(IMiddlerAction middlerAction, MiddlerContext middlerContext)
        {
            var method = middlerAction.GetType().GetMethod("ExecuteResponseAsync") ?? middlerAction.GetType().GetMethod("ExecuteResponse");
            if (method == null)
            {
                return;
            }

            var man = new Dictionary<Type, object>
            {
                
            };

            var parameters = BuildExecuteMethodParameters(method, middlerContext, man);

            if (typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                var t = (Task)method.Invoke(middlerAction, parameters);
                await t.ConfigureAwait(false);
            }
            else
            {
                method.Invoke(middlerAction, parameters);
            }

        }


        private object[] BuildExecuteMethodParameters(MethodInfo methodInfo, MiddlerContext middlerContext, Dictionary<Type, object> manualCreateParameters)
        {

            return methodInfo.GetParameters().Select(p =>
            {

                if (manualCreateParameters.TryGetValue(p.ParameterType, out var manualCreated))
                    return manualCreated;

                if (p.ParameterType == typeof(IMiddlerContext))
                {
                    return middlerContext;
                }
               

                if (p.ParameterType == typeof(IActionHelper))
                {
                    return new ActionHelper(middlerContext.Request);
                }

                return middlerContext.RequestServices.GetRequiredService(p.ParameterType);

            }).ToArray();

        }


        private async Task WriteToAspNetCoreResponseBodyAsync(HttpContext context, MiddlerContext middlerContext)
        {
            //if (originalBody == null)
            //    return;

           
            //var tempStream = context.Response.Body;
            //context.Response.Body = originalBody;

            foreach (var (key, value) in middlerContext.MiddlerResponseContext.Headers)
            {
                context.Response.Headers[key] = value;
            }

            if (middlerContext.Response.StatusCode != 0)
            {
                context.Response.StatusCode = middlerContext.Response.StatusCode;
            }


            context.Response.Headers["Content-Type"] = "application/json";
            context.Response.Headers.ContentLength = null;
            if (context.Response.Body.CanWrite)
            {
                middlerContext.MiddlerResponseContext.Body.Seek(0, SeekOrigin.Begin);
                await middlerContext.MiddlerResponseContext.Body.CopyToAsync(context.Response.Body, 131072, context.RequestAborted).ConfigureAwait(false);
                await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
            }


            await middlerContext.MiddlerResponseContext.Body.DisposeAsync().ConfigureAwait(false);
        }

       
        private MiddlerRuleMatch FindMatchingEndpoint(IMiddlerOptions middlerOptions, List<MiddlerRule> rule,  MiddlerContext middlerContext)
        {

            for (int i = 0; i < rule.Count; i++)
            {
                var match = CheckMatch(middlerOptions, rule[i], middlerContext);
                if (match != null)
                {

                    if (match.AccessMode != AccessMode.Ignore)
                    {
                        match.RemainingEndpointInfos = rule.Skip(i + 1).ToList();
                        return match;
                    }
                }

            }

            return null;
        }


        private MiddlerRuleMatch CheckMatch(IMiddlerOptions middlerOptions, MiddlerRule rule, MiddlerContext middlerContext)
        {

            var allowedHttpMethods = (rule.HttpMethods?.IgnoreNullOrWhiteSpace().Any() == true ? rule.HttpMethods.IgnoreNullOrWhiteSpace() : middlerOptions.DefaultHttpMethods).ToList();

            if (allowedHttpMethods.Any() && !allowedHttpMethods.Contains(middlerContext.Request.HttpMethod, StringComparer.OrdinalIgnoreCase))
                return null;

            //var uri = new Uri(context.Request.GetEncodedUrl());

            var allowedSchemes = (rule.Scheme?.IgnoreNullOrWhiteSpace().Any() == true ? rule.Scheme.IgnoreNullOrWhiteSpace() : middlerOptions.DefaultScheme).ToList();

            if (allowedSchemes.Any() && !allowedSchemes.Any(scheme => Wildcard.Match(middlerContext.MiddlerRequestContext.Uri.Scheme, scheme)))
                return null;

            if (!Wildcard.Match($"{middlerContext.MiddlerRequestContext.Uri.Host}:{middlerContext.MiddlerRequestContext.Uri.Port}", rule.Hostname ?? "*"))
                return null;

            if (String.IsNullOrWhiteSpace(rule.Path))
            {
                rule.Path = "{**path}";
            }

            var parsedTemplate = TemplateParser.Parse(rule.Path);

            var defaults = parsedTemplate.Parameters.Where(p => p.DefaultValue != null)
                .Aggregate(new RouteValueDictionary(), (current, next) =>
                {
                    current.Add(next.Name, next.DefaultValue);
                    return current;
                });

            var matcher = new TemplateMatcher(parsedTemplate, defaults);
            var rd = middlerContext.MiddlerRequestContext.GetRouteData();
            var router = rd.Routers.FirstOrDefault() ?? new RouteCollection();

            if (matcher.TryMatch(middlerContext.MiddlerRequestContext.Uri.AbsolutePath, rd.Values))
            {
                var constraints = GetConstraints(middlerContext.RequestServices.GetRequiredService<IInlineConstraintResolver>(), parsedTemplate, null);
                if (MiddlerRouteConstraintMatcher.Match(constraints, rd.Values, router, RouteDirection.IncomingRequest, ConstraintLogger))
                {

                    middlerContext.SetRouteData(constraints);
                    
                    return new MiddlerRuleMatch
                    {
                        MiddlerRule = rule,
                        AccessMode = rule.AccessAllowed(middlerContext.Request) ?? middlerOptions.DefaultAccessMode
                    };

                }
            }

            return null;
        }


        private static IDictionary<string, IRouteConstraint> GetConstraints(IInlineConstraintResolver inlineConstraintResolver, RouteTemplate parsedTemplate, IDictionary<string, object> constraints)
        {

            var constraintBuilder = new RouteConstraintBuilder(inlineConstraintResolver, parsedTemplate.TemplateText);

            if (constraints != null)
            {
                foreach (var kvp in constraints)
                {
                    constraintBuilder.AddConstraint(kvp.Key, kvp.Value);
                }
            }

            foreach (var parameter in parsedTemplate.Parameters)
            {

                if (parameter.IsOptional)
                {
                    constraintBuilder.SetOptional(parameter.Name);
                }

                foreach (var inlineConstraint in parameter.InlineConstraints)
                {
                    constraintBuilder.AddResolvedConstraint(parameter.Name, inlineConstraint.Constraint);
                }

            }

            return constraintBuilder.Build();
        }

        private void EnsureLoggers(HttpContext context)
        {

            if (Logger == null)
            {
                var factory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                Logger = factory.CreateLogger(typeof(RouteBase).FullName);
                ConstraintLogger = factory.CreateLogger(typeof(RouteConstraintMatcher).FullName);
            }

        }

       
    }
}
