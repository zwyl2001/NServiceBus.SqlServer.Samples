namespace SqlSubscriber
{
    using System;
    using Events;
    using NServiceBus;

    class SomethingHappenedEventHandler : IHandleMessages<SomethingHappened>
    {
        public void Handle(SomethingHappened message)
        {
            Console.WriteLine("Sql subscriber has now received this event. This was originally published by MSMQ publisher.");
        }
    }
}
