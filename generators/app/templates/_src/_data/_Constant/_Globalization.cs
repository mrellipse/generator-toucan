using System;
using System.Globalization;
using System.Threading;

namespace <%=assemblyName%>.Data
{
    public static class Globalization
    {
        public static string DefaultTimeZoneId
        {
            get
            {
                return TimeZoneInfo.Local.Id;
            }
        }
    }
}