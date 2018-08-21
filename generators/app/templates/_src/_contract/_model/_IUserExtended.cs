using System;
using System.Collections.Generic;

namespace <%=assemblyName%>.Contract
{
    public interface IUserExtended : IUser
    {
        IEnumerable<string> Claims { get; }
        IEnumerable<string> Roles { get; }
        DateTime CreatedOn { get; }
    }
}