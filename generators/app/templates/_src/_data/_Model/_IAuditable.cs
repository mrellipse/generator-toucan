using System;
using <%=assemblyName%>.Data.Model;

namespace <%=assemblyName%>.Data
{
    public interface IAuditable
    {
        long CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        long? LastUpdatedBy { get; set; }
        DateTime? LastUpdatedOn { get; set; }

        User CreatedByUser { get; set; }
        User LastUpdatedByUser { get; set; }
    }
}