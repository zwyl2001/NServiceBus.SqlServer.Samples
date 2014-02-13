namespace MsmqSubscriber
{
    using System;
    using Events;
    using NServiceBus;

    public class SomethingHappenedEventHandler : IHandleMessages<SomethingHappened>
    {
        public void Handle(SomethingHappened message)
        {
            Console.WriteLine("MSMQ Subscriber has now received the event: SomethingHappened");
        }
    }
}
