using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using StructureMap;
using <%=assemblyName%>.Common;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Data;

namespace <%=assemblyName%>.Server
{
    public partial class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            AppConfig config = WebApp.Configuration.Get<<%=assemblyName%>.Server.AppConfig>();
            IConfigurationSection logging = WebApp.Configuration.GetSection("Logging");

            if (logging.GetSection("Debug").Exists())
                loggerFactory.AddDebug();

            if (logging.GetSection("Console").Exists())
                loggerFactory.AddConsole(logging.GetSection("Console"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions()
                {
                    HotModuleReplacement = true,
                    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\ui")
                });
            }

            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseResponseCompression();

            string webRoot = new DirectoryInfo(config.Server.Webroot).FullName;

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(webRoot),
                OnPrepareResponse = (content) =>
                {
                    var cultureService = content.Context.RequestServices.GetRequiredService<CultureService>();
                    cultureService.EnsureCookie(content.Context);
                }
            });

            app.UseAntiforgeryMiddleware(config.Server.AntiForgery.ClientName);
            app.UseRequestLocalization();
            app.UseMvc();
            app.UseHistoryModeMiddleware(webRoot, config.Server.Areas);

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
            var config = WebApp.Configuration.Get<<%=assemblyName%>.Server.AppConfig>();

            services.AddOptions();
            services.Configure<AppConfig>(WebApp.Configuration); // root web configuration
            services.Configure<<%=assemblyName%>.Service.Config>(WebApp.Configuration.GetSection("service")); // services configuration
            services.Configure<<%=assemblyName%>.Service.TokenProviderConfig>(WebApp.Configuration.GetSection("service:tokenProvider")); // token provider configuration
            services.Configure<<%=assemblyName%>.Data.Config>(WebApp.Configuration.GetSection("data")); // configuration
            services.Configure<<%=assemblyName%>.Server.Config>(WebApp.Configuration.GetSection("server"));

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.MimeTypes = new[]
                {
                    // Default
                    "text/plain",
                    "text/css",
                    "application/javascript",
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",
                    // Custom
                    "image/svg+xml"
                };
            });

            services.AddMemoryCache();
            services.ConfigureAuthentication(config.Service.TokenProvider, new string[] { "admin" });
            services.ConfigureMvc(config.Server.AntiForgery);
            <%if(dbProvider == 'npgsql'){%>
            services.AddDbContext<NpgSqlContext>(options =>
            {    
                string assemblyName = typeof(<%=assemblyName%>.Data.Config).GetAssemblyName();
                options.UseNpgsql(config.Data.ConnectionString, s => s.MigrationsAssembly(assemblyName));
            });<%}%><%if(dbProvider == 'mssql'){%>
            services.AddDbContext<MsSqlContext>(options =>
            {
                string assemblyName = typeof(<%=assemblyName%>.Data.Config).GetAssemblyName();
                options.UseSqlServer(config.Data.ConnectionString, s => s.MigrationsAssembly(assemblyName));
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