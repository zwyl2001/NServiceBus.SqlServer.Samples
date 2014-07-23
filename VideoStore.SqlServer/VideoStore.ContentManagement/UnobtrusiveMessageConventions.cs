namespace VideoStore
{
    using NServiceBus;

    static class UnobtrusiveMessageConventions
    {
        public static void Init(Configure.ConventionsBuilder conventionsBuilder)
        {
            conventionsBuilder
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("VideoStore") && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("VideoStore") && t.Namespace.EndsWith("Events"))
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.StartsWith("VideoStore") && t.Namespace.EndsWith("RequestResponse"))
                .DefiningEncryptedPropertiesAs(p => p.Name.StartsWith("Encrypted"));
        }
    }
}
