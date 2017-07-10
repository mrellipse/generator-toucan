using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using StructureMap;
using <%=assemblyName%>.Common.Extensions;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Data;

namespace <%=assemblyName%>.Server
{
    public partial class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var cfg = app.ApplicationServices.GetRequiredService<IOptions<AppConfig>>().Value;
            string webRoot = new DirectoryInfo(cfg.Server.Webroot).FullName;

            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(LogLevel.Debug);

                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions()
                {
                    HotModuleReplacement = true,
                    ProjectPath = Path.Combine(WebApp.ContentRoot, @"..\ui")
                });
            }

            app.UseDefaultFiles();
            app.UseTokenBasedAuthentication(cfg.Service.TokenProvider, cfg.Server.Areas);
            app.UseAntiforgeryMiddleware(cfg.Server.AntiForgery.ClientName);

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(webRoot)
            });

            app.UseMvc();
            app.UseHistoryModeMiddleware(webRoot, cfg.Server.Areas);

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var dbContext = serviceScope.ServiceProvider.GetService<DbContextBase>())
                {
                    ICryptoService crypto = app.ApplicationServices.GetRequiredService<ICryptoService>();
                    dbContext.Database.Migrate();
                    dbContext.EnsureSeedData(crypto);
                }
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var dataConfig = WebApp.Configuration.GetTypedSection<<%=assemblyName%>.Data.Config>("data");

            services.AddOptions();
            services.Configure<AppConfig>(WebApp.Configuration); // root web configuration
            services.Configure<<%=assemblyName%>.Service.Config>(WebApp.Configuration.GetSection("service")); // services configuration
            services.Configure<<%=assemblyName%>.Service.TokenProviderConfig>(WebApp.Configuration.GetSection("service:tokenProvider")); // token provider configuration
            services.Configure<<%=assemblyName%>.Data.Config>(WebApp.Configuration.GetSection("data")); // configuration
            services.Configure<<%=assemblyName%>.Server.Config>(WebApp.Configuration.GetSection("server"));

            services.ConfigureMvc(WebApp.Configuration.GetTypedSection<Config.AntiForgeryConfig>("server:antiForgery"));
            services.AddMemoryCache();
            <%if(dbProvider == 'npgsql'){%>
            services.AddDbContext<NpgSqlContext>(options =>
            {    
                string assemblyName = typeof(<%=assemblyName%>.Data.Config).GetAssemblyName();
                options.UseNpgsql(dataConfig.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            });<%}%>
            <%if(dbProvider == 'mssql'){%>
            services.AddDbContext<MsSqlContext>(options =>
            {
                string assemblyName = typeof(<%=assemblyName%>.Data.Config).GetAssemblyName();
                options.UseSqlServer(dataConfig.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            });<%}%>

            var container = new Container(c =>
            {
                var registry = new Registry();

                registry.IncludeRegistry<<%=assemblyName%>.Common.ContainerRegistry>();
                registry.IncludeRegistry<<%=assemblyName%>.Data.ContainerRegistry>();
                registry.IncludeRegistry<<%=assemblyName%>.Service.ContainerRegistry>();
                registry.IncludeRegistry<<%=assemblyName%>.Server.ContainerRegistry>();

                c.AddRegistry(registry);
                c.Populate(services);
            });

            return container.GetInstance<IServiceProvider>();
        }
    }
}