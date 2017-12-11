using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using <%=assemblyName%>.Server.Model;

namespace <%=assemblyName%>.Server.Filters
{
    public class ApiExceptionFilterTargets : Dictionary<Type, PayloadMessageType>
    {

    }
}