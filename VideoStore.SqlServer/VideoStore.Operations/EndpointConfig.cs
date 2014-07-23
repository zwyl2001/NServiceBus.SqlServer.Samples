namespace VideoStore.Operations
{
    using NServiceBus;

	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, UsingTransport<SqlServer>
    {
	    public void Customize(ConfigurationBuilder builder)
	    {
          builder.Conventions(UnobtrusiveMessageConventions.Init);
	    }
    }
	
}
