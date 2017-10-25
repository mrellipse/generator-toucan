using System.IO;
using Microsoft.Extensions.Options;
using <%=assemblyName%>.Server.Model;

namespace <%=assemblyName%>.Server
{
    public class LocalizationResolverOptions : IConfigureOptions<LocalizationOptions>
    {
        private readonly Config config;

        public LocalizationResolverOptions(IOptions<<%=assemblyName%>.Server.Config> config)
        {
            this.config = config.Value;
        }

        public void Configure(LocalizationOptions options)
        {
            options.Directory = new DirectoryInfo("./resources");
            options.Pattern = "{0}.json";
        }
    }
}