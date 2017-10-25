using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace <%=assemblyName%>.Contract
{
    public interface IManageProfileService
    {
        Task<IUserExtended> UpdateUserCulture(long userId, string cultureName, string timeZoneId);
    }
}
