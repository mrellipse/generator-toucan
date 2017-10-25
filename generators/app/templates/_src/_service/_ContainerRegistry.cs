using StructureMap;
using <%=assemblyName%>.Contract;
using <%=assemblyName%>.Common;

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
            
            For<IManageUserService>().Add<ManageUserService>();
            For<IManageProfileService>().Add<ManageUserService>();
            For<ISignupService>().Use<SignupService>();
            For<IVerificationProvider>().Use<SmtpVerificationProvider>();

            For<ITokenProviderService<Token>>().Use<TokenProviderService>();
        }
    }
}