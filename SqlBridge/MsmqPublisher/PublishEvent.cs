﻿
namespace MsmqPublisher
{
    using System;
    using Events;
    using NServiceBus;

    /// <summary>
    /// Bootstrapper that facilitates testing by publishing an event every time Enter is pressed
    /// </summary>
    public class PublishEvent : IWantToRunWhenBusStartsAndStops
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Console.WriteLine("Press Enter to publish the SomethingHappened Event");
            while (Console.ReadLine() != null)
            {
                Console.WriteLine("Event published");
                Bus.Publish(new SomethingHappened());
            }
        }

        public void Stop()
        {
        }
    }
}
