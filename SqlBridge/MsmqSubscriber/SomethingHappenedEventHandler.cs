namespace MsmqSubscriber
{
    using System;
    using Events;
    using NServiceBus;

    public class SomethingHappenedEventHandler : IHandleMessages<SomethingHappened>
    {
        public void Handle(SomethingHappened message)
        {
            Console.WriteLine("Received event: SomethingHappened");
        }
    }
}
