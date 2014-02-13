using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;
using NServiceBus;

namespace SqlSubscriber
{
    class SomethingHappenedEventHandler : IHandleMessages<SomethingHappened>
    {
        public void Handle(SomethingHappened message)
        {
            Console.WriteLine("Sql subscriber has now received this event. This was originally published by MSMQ publisher.");
        }
    }
}
