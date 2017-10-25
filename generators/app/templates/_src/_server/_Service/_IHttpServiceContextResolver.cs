using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using <%=assemblyName%>.Contract;

namespace <%=assemblyName%>.Server
{
    public interface IHttpServiceContextResolver : IDomainContextResolver
    {
        IUser Resolve(HttpContext context, bool cache = true);
    }
}