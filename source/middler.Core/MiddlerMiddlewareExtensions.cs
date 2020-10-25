using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using middler.Common.Interfaces;
using System;
using NamedServices.Microsoft.Extensions.DependencyInjection;
using Reflectensions.ExtensionMethods;
using middler.Core.Map;
using middler.Core.Models;

namespace middler.Core {
    public static class MiddlerMiddlewareExtensions {
        public static IServiceCollection AddMiddler(this IServiceCollection services, IMiddlerOptions options) {

            services.AddSingleton<IMiddlerMap, MiddlerMap>();
            services.AddSingleton<IMiddlerOptions>(sp => options);
            services.AddTransient<InternalHelper>(sp => new InternalHelper(sp));
            services.AddTransient<IMiddlerMapActionsBuilder, MiddlerMapActionsBuilder>();

            return services;
        }

        public static IServiceCollection AddMiddler(this IServiceCollection services, Action<MiddlerOptionsBuilder> optionsBuilder) {

            var mOptions = new MiddlerOptionsBuilder(services);
            optionsBuilder.Invoke(mOptions);
            return AddMiddler(services, mOptions.Options);
        }

        public static IServiceCollection AddMiddler(this IServiceCollection services) {
            return AddMiddler(services, new MiddlerOptions());
        }

        public static void AddMiddlerRepo<T>(this IServiceCollection services, ServiceLifetime serviceLifetime) where T : class, IMiddlerRepository {
            services.Add(new ServiceDescriptor(typeof(T), typeof(T), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IMiddlerRepository), sp => sp.GetRequiredService(typeof(T)), serviceLifetime));
        }

        public static void AddMiddlerRepo<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory, ServiceLifetime serviceLifetime) where T : class, IMiddlerRepository {
            services.Add(new ServiceDescriptor(typeof(T), implementationFactory, serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(T), sp => sp.GetRequiredService(typeof(T)), serviceLifetime));
        }

        public static void AddMiddlerRepo<T>(this IServiceCollection services) where T : class, IMiddlerRepository {
            services.AddMiddlerRepo<T>(ServiceLifetime.Singleton);
        }

        public static void AddMiddlerRepo<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IMiddlerRepository {
            services.AddMiddlerRepo<T>(implementationFactory, ServiceLifetime.Singleton);
        }

        public static void AddNamedMiddlerRepo<T>(this IServiceCollection services, string name, ServiceLifetime serviceLifetime) where T : class, IMiddlerRepository {

            services.AddNamed<T>(name, serviceLifetime);
            services.AddNamed<IMiddlerRepository>(name, sp => sp.GetNamedService<T>(name), serviceLifetime);

        }

        public static void AddNamedMiddlerRepo<T>(this IServiceCollection services, string name, Func<IServiceProvider, T> implementationFactory, ServiceLifetime serviceLifetime) where T : class, IMiddlerRepository {

            if (implementationFactory != null) {
                services.AddNamed<T>(name, implementationFactory, serviceLifetime);
            } else {
                services.AddNamed<T>(name, serviceLifetime);
            }
            services.AddNamed<IMiddlerRepository>(name, sp => {
                return sp.GetNamedService<T>(name);
            }, serviceLifetime);

        }

        public static void AddNamedMiddlerRepo<T>(this IServiceCollection services, string name) where T : class, IMiddlerRepository {
            services.AddNamedMiddlerRepo<T>(name, ServiceLifetime.Singleton);
        }

        public static void AddNamedMiddlerRepo<T>(this IServiceCollection services, string name, Func<IServiceProvider, T> implementationFactory) where T : class, IMiddlerRepository {
            services.AddNamedMiddlerRepo<T>(name, implementationFactory, ServiceLifetime.Singleton);
        }

        public static IApplicationBuilder UseMiddler(this IApplicationBuilder appBuilder, Action<MiddlerMapBuilder> builder) {

            var mapBuilder = new MiddlerMapBuilder(appBuilder.ApplicationServices);
            builder.InvokeAction(mapBuilder);
            
            appBuilder.UseMiddleware<MiddlerMiddleware>();
            return appBuilder;
        }
    }
}
