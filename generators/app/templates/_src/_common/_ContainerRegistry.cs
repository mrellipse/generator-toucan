using StructureMap;
using <%=assemblyName%>.Contract;

namespace <%=assemblyName%>.Common
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();
        }
    }
}