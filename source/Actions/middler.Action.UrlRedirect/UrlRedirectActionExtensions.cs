using System;
using Microsoft.Extensions.DependencyInjection;
using middler.Common.Interfaces;

namespace middler.Common.Actions.UrlRedirect
{
    public static class UrlRedirectActionExtensions
    {
        
        public static IMiddlerOptionsBuilder AddUrlRedirectAction(this IMiddlerOptionsBuilder optionsBuilder, string alias = null)
        {
            alias = !String.IsNullOrWhiteSpace(alias) ? alias : UrlRedirectAction.DefaultActionType;
            optionsBuilder.ServiceCollection.AddTransient<UrlRedirectAction>();
            optionsBuilder.RegisterAction<UrlRedirectAction>(alias);

            return optionsBuilder;
        }
    }
}
