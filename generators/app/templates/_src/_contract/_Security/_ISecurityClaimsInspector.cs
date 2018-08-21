
using System.Security.Claims;
using System.Threading.Tasks;

namespace <%=assemblyName%>.Contract.Security
{
    public interface ISecurityClaimsInspector
    {
        bool Satisifies(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claimType, params object[] values);
    }
}