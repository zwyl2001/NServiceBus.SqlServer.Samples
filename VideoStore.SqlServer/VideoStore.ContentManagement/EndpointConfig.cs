namespace VideoStore.ContentManagement
{
    using System.Diagnostics;
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, UsingTransport<SqlServerTransport>
    {
        public void Customize(BusConfiguration configuration)
        {
            // For production use, please select a durable persistence.
            // To use RavenDB, install-package NServiceBus.RavenDB and then use configuration.UsePersistence<RavenDBPersistence>();
            // To use SQLServer, install-package NServiceBus.NHibernate and then use configuration.UsePersistence<NHibernatePersistence>();
            if (Debugger.IsAttached)
            {
                configuration.UsePersistence<InMemoryPersistence>();
            }
            configuration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("VideoStore") && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("VideoStore") && t.Namespace.EndsWith("Events"))
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.StartsWith("VideoStore") && t.Namespace.EndsWith("RequestResponse"));
            configuration.RijndaelEncryptionService();
        }
    }
}
