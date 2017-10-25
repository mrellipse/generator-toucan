using System;
using System.Globalization;

namespace <%=assemblyName%>.Contract
{
    public interface IDomainContextResolver
    {
        IDomainContext Resolve(bool cache = true);
    }
}