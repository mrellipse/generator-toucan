
using System.Collections.Generic;

namespace <%=assemblyName%>.Contract
{
    public interface IUserExtended : IUser
    {
        IEnumerable<string> Roles { get; }
    }
}