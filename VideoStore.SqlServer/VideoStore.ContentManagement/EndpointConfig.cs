namespace VideoStore.ContentManagement
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher, UsingTransport<SqlServer>
    {
        public void Customize(ConfigurationBuilder builder)
        {
            builder.Conventions(UnobtrusiveMessageConventions.Init);
        }
    }
}
