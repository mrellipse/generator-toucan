
using <%=assemblyName%>.Contract.Model;

namespace <%=assemblyName%>.Service.Model
{
    public class KeyValue : IKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}