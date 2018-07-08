using System.Collections.Generic;
using System.Threading.Tasks;

namespace <%=assemblyName%>.Contract
{
    public interface IDeviceProfiler
    {
        string DeriveFingerprint(IUser user);
    }
}