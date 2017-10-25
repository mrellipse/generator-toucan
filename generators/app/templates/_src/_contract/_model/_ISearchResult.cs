using System.Collections.Generic;

namespace <%=assemblyName%>.Contract
{
    public interface ISearchResult<TOut>
    {
        IEnumerable<TOut> Items { get; }
        int Page { get; }
        int PageSize { get; }
        long Total { get; }
    }
}
