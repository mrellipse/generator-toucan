
using System;
using System.Collections.Generic;
using System.Globalization;
using <%=assemblyName%>.Contract.Model;

namespace <%=assemblyName%>.Contract
{
    public interface ILocalizationDictionary
    {
        ILocalizedString this[string key] { get; }
        CultureInfo Culture { get; }
        TimeZoneInfo TimeZone { get; }
        IEnumerable<string> Keys { get; }
        IEnumerable<ILocalizedString> Values { get; }
        bool ContainsKey(string key);
        bool TryGetValue(string key, out ILocalizedString value);
    }
}