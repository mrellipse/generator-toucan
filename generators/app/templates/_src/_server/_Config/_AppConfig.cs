namespace <%=assemblyName%>.Server
{
    public class AppConfig
    {
        public AppConfig()
        {
        }

        public <%=assemblyName%>.Data.Config Data { get; set; }
        public <%=assemblyName%>.Server.Config Server { get; set; }
        public <%=assemblyName%>.Service.Config Service { get; set; }
    }
}
