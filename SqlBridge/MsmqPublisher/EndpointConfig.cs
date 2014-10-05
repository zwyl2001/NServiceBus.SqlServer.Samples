namespace MsmqPublisher 
{
    using NServiceBus;

	/*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://nservicebus.com/GenericHost.aspx
	*/

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher
    {
        
    }

    public class MessageConventions : IWantToRunBeforeConfiguration
    {
        public void Init()
        {
            Configure.Instance.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        }
    }

    class InititalizeSubscriptionStorage : INeedInitialization
    {
        public void Init()
        {
            Configure.Instance
                .UseNHibernateSubscriptionPersister() // subscription storage using NHibernate
                .UseNHibernateTimeoutPersister() // Timeout Persistance using NHibernate
                .UseNHibernateSagaPersister(); // Saga Persistance using NHibernate
        }
    }
}