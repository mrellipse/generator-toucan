using StructureMap;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Contract.Security;
using <%=assemblyName%>.Common;
using <%=assemblyName%>.Service.Security;

namespace <%=assemblyName%>.Service
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();

            For<ILocalAuthenticationService>().Use<LocalAuthenticationService>();
            For<IExternalAuthenticationService>().Use<ExternalAuthenticationService>();
            For<IExternalAuthenticationProvider>().Add<GoogleAuthenticationProvider>();
            For<IExternalAuthenticationProvider>().Add<MicrosoftAuthenticationProvider>();
            
            For<IAuditService>().Use<AuditService>();
            For<IManageRoleService>().Use<ManageRoleService>();
            For<IManageProfileService>().Use<ManageUserService>();
            For<IManageUserService>().Use<ManageUserService>();
            For<ISecurityClaimsInspector>().Use<SecurityClaimsInspector>().Singleton();
            For<ISignupService>().Use<SignupService>();
            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
        }
    }
}