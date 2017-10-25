using System;

namespace <%=assemblyName%>.Common
{
    public static partial class Extensions
    {
        public static string GetAssemblyName(this Type type)
        {
            return type.AssemblyQualifiedName.Split(',')[1].Trim();
        }
    }
}