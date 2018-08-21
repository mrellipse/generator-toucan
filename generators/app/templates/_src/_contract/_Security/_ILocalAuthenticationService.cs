using System.Security.Claims;
using System.Threading.Tasks;

namespace <%=assemblyName%>.Contract.Security
{
    public interface ILocalAuthenticationService
    {
        Task<ClaimsIdentity> ResolveUser(string username, string password);

        Task<IUser> ResolveUser(string username);

        Task<bool> ValidateUser(string username);
    }
}
