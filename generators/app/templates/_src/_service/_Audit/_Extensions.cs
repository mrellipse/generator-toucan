using <%=assemblyName%>.Contract;

namespace <%=assemblyName%>.Service
{
    public static partial class Extensions
    {
        public static void TokenCreatedEvent(this <%=assemblyName%>.Contract.IAuditService audit, string userName, string issuer, int? exp)
        {
            var eventTypeId = <%=assemblyName%>.Service.AuditServiceEventType.TokenCreated;
            IAuditEventData data = new <%=assemblyName%>.Service.Model.AuditEventData(eventTypeId, $"Token Create. Issuer: {issuer}. User Name: {userName}. Exp: {exp}");
            audit.Record(data);
        }
    }
}