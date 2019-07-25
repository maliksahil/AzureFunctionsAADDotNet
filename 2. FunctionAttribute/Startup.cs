using System;
using DotNetAuthorizeFunction;
using DotNetAuthorizeFunction.Config;
using DotNetAuthorizeFunction.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DotNetAuthorizeFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.Configure<Settings>(settings => {
                settings.TenantId = Environment.GetEnvironmentVariable("TenantId");
                settings.ClientId = Environment.GetEnvironmentVariable("ClientId");
                settings.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
                settings.Audience = Environment.GetEnvironmentVariable("Audience");
                settings.Issuer = Environment.GetEnvironmentVariable("Issuer");
                settings.MetadataUrl = Environment.GetEnvironmentVariable("MetadataUrl");
            });

            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        }
    }
}