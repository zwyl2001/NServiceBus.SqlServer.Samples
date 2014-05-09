namespace SqlBridge
{
    using System;
    using System.Configuration;
    using System.Security.Cryptography;
    using System.Text;
    using NServiceBus;
    using NServiceBus.Satellites;
    using NServiceBus.Transports;
    using NServiceBus.Transports.Msmq;

    /// <summary>
    /// Implements an advanced satellite. Allows to receive messages on a different transport.
    /// </summary>
    class MsmqReceiver : IAdvancedSatellite
    {
        /// <summary>
        /// Since this endpoint's transport is usingSqlServer, the IPublishMessages will be using the SqlTransport to publish
        /// messages
        /// </summary>
        public IPublishMessages Publisher { get; set; }

        /// <summary>
        /// Since we want to listen to the events published by MSMQ, we are newing up MsmqDequeueStrategy and setting the
        /// input queue to the queue which will be receiving all the events from the MSMQ publishers.
        /// </summary>
        /// <returns>MsmqTransport receiver</returns>
        public Action<NServiceBus.Unicast.Transport.TransportReceiver> GetReceiverCustomization()
        {
            return (tr => {tr.Receiver = new MsmqDequeueStrategy(); });
        }

        public bool Disabled
        {
            get { return false; }
        }

        /// <summary>
        /// Will get invoked, whenever a new event is published by the Msmq publishers and when they notify the bridge. 
        /// The bridge is a MSMQ and the publishers have an entry for this queue in their subscription storage.
        /// </summary>
        public bool Handle(TransportMessage message)
        {
            var eventTypes = new[] { Type.GetType(message.Headers["NServiceBus.EnclosedMessageTypes"]) };

            var msmqId = message.Headers["NServiceBus.MessageId"];
            
            // Set the Id to a deterministic guid, as Sql message Ids are Guids and Msmq message ids are guid\nnnn.
            // Newer versions of Nsb already return just a guid for the messageId. So, check to see if the Id is a valid Guid and if 
            // not, only then create a valid Guid. This check is important as it affects the retries if the message is rolled back.
            // If the Ids are different, then the FLR/SLR won't know its the same message.
            Guid newGuid;
            if (!Guid.TryParse(msmqId, out newGuid))
            {
                message.Headers["NServiceBus.MessageId"] = BuildDeterministicGuid(msmqId).ToString();
            }

            Publisher.Publish(message, eventTypes);
            return true;
        }

        /// <summary>
        /// Address of the MSMQ that will be receiving all of the events from all of hte MSMQ publishers.
        /// </summary>
        public Address InputAddress
        {
            get { return Address.Parse(ConfigurationManager.AppSettings["SqlBridgeAddress"]); }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        Guid BuildDeterministicGuid(string msmqMessageId)
        {
            // use MD5 hash to get a 16-byte hash of the string
            using (var provider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.Default.GetBytes(msmqMessageId);
                var hashBytes = provider.ComputeHash(inputBytes);
                // generate a guid from the hash:
                return new Guid(hashBytes);
            }
        }
    }
}
