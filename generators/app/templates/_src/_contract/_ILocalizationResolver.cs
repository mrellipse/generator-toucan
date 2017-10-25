using System.Collections.Generic;
using <%=assemblyName%>.Contract.Model;

namespace <%=assemblyName%>.Contract
{
    public interface ILocalizationResolver
    {
        IEnumerable<IKeyValue> ResolveSupportedCultures();
        object ResolveCulture(string cultureName);
    }
}
