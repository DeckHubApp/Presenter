using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slidable.Presenter.Authentication;
using Slidable.Presenter.Internals;
using Slidable.Presenter.Messaging;
using Slidable.Presenter.Options;
using Slidable.Presenter.Services;
using StackExchange.Redis;

namespace Slidable.Presenter
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IIdentityPaths, IdentityPaths>();

            ConfigureAuth(services);

            var connectionMultiplexer = ConfigureRedis(services);

            ConfigureDataProtection(services, connectionMultiplexer);

            services.Configure<MessagingOptions>(Configuration.GetSection("Messaging"));

            services.AddSingleton<IMessageBus, MessageBus>();

            services.AddSingleton<IApiKeyProvider, ApiKeyProvider>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddMetrics();
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var pathBase = Configuration["Runtime:PathBase"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseMiddleware<BypassAuthMiddleware>();
            }
            else
            {
                app.UseAuthentication();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
       }

        private void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
        }

        private ConnectionMultiplexer ConfigureRedis(IServiceCollection services)
        {
            var redisHost = Configuration.GetSection("Redis").GetValue<string>("Host");
            if (!string.IsNullOrWhiteSpace(redisHost))
            {
                var redisPort = Configuration.GetSection("Redis").GetValue<int>("Port");
                if (redisPort == 0)
                {
                    redisPort = 6379;
                }

                var connectionMultiplexer = ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
                services.AddSingleton(connectionMultiplexer);
                return connectionMultiplexer;
            }

            return null;
        }

        private void ConfigureDataProtection(IServiceCollection services, ConnectionMultiplexer connectionMultiplexer)
        {
            if (!_env.IsDevelopment())
            {
                var dpBuilder = services.AddDataProtection().SetApplicationName("slidable");

                if (connectionMultiplexer != null)
                {
                    dpBuilder.PersistKeysToRedis(connectionMultiplexer, "DataProtection:Keys");
                }
            }
            else
            {
                services.AddDataProtection()
                    .DisableAutomaticKeyGeneration()
                    .SetApplicationName("slidable");
            }
        }

    }


}
