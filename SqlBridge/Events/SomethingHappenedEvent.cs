using NServiceBus;

namespace Events
{
    /// <summary>
    /// Some event that is being published by MSMQ endpoint
    /// </summary>
    public class SomethingHappened : IEvent
    {
    }
}
