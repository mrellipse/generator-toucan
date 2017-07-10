using System;
using StructureMap;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using <%=assemblyName%>.Data;

namespace <%=assemblyName%>.Server
{
    internal class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            var targets = new Dictionary<Type, PayloadMessageType>()
            {
                { typeof(<%=assemblyName%>.Service.ServiceException), PayloadMessageType.Failure}
            };

            For<IConfiguration>().Use(WebApp.Configuration).Singleton();
            <%if(dbProvider == 'npgsql'){%>For<DbContextBase>().Use<NpgSqlContext>();<%}%>
            <%if(dbProvider == 'mssql'){%>For<DbContextBase>().Use<MsSqlContext>();<%}%>
            For<Filters.ApiResultFilter>();
            For<Filters.ApiExceptionFilter>().Use(() => new Filters.ApiExceptionFilter(targets));
            For<Filters.IdentityMappingFilter>();
        }
    }
}