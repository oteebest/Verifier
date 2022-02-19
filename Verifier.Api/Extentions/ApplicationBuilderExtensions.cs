using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Verifier.Shared.Constants.Application;

namespace Verifier.Api.Extentions
{
    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder Initialize(this IApplicationBuilder app, Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize(Convert.ToBoolean(_configuration.GetSection(ApplicationConstants.DataSeeder.ShouldSeedDefaultData).Value));
            }

            return app;
        }
    }
}