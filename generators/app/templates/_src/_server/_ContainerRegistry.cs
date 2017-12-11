using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using StructureMap;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Service;
using <%=assemblyName%>.Service.Localization;

using <%=assemblyName%>.Data;

namespace <%=assemblyName%>.Server
{
    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            var targets = new Filters.ApiExceptionFilterTargets()
            {
                { typeof(<%=assemblyName%>.Service.ServiceException), PayloadMessageType.Failure}
            };

            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
            <%if(dbProvider == 'npgsql'){%>For<DbContextBase>().Use<NpgSqlContext>();<%}%>
            <%if(dbProvider == 'mssql'){%>For<DbContextBase>().Use<MsSqlContext>();<%}%>
            
            For<HttpServiceContextFactory>();
            For<IHttpContextAccessor>().Use<HttpContextAccessor>().Transient();
            For<IHttpServiceContextResolver>().Use<HttpServiceContextResolver>();
            For<IDomainContextResolver>().Use<HttpServiceContextResolver>();
            For<ILocalizationResolver>().Add<LocalizationResolver>().Singleton();
            For<ILocalizationService>().Add<LocalizationService>();

            For<Filters.ApiResultFilter>();
            For<Filters.ApiExceptionFilterTargets>().Use(targets);
            For<Filters.ApiExceptionFilter>();
            For<Filters.IdentityMappingFilter>();

            For<CultureService>();
        }
    }
}