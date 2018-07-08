using System.Collections.Generic;
using System.Threading.Tasks;

namespace <%=assemblyName%>.Contract
{
    public interface IVerificationProvider
    {
        string Key { get; }
        bool CanHandle(IUser user, string code);
        Task<string> IssueCode(IUser user);
        Task<bool> RedeemCode(IUser user, string code);
    }
}
