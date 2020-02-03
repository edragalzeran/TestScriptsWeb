using System;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap;

namespace TestScriptsWeb.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AuscultacioApi>();
            services.AddScoped<LocalStorageService>();
            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                })
                .AddBootstrapProviders();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.Services
                .UseBootstrapProviders();
            app.AddComponent<App>("app");
        }
    }
}
