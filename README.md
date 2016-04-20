# NServiceBus.SqlServer.Samples
----------

This repository contains the Samples for the SqlServer transport. The samples include needed references for SqlServer. The endpoints are configured to use SqlServer as its transport. For example:
````c#
  public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, UsingTransport<SqlServer> { }
````

The app.config provides the necessary connection information needed to communicate to SQL server. For example:

````xml
<connectionStrings>
  <add name="NServiceBus/Transport" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True" />
</connectionStrings>
````
 
##VideoStore.SqlServer

This sample implements the following workflow of a fictional video store. Users can order videos from the website. Once orders are submitted, there is a window of time allocated for handling cancellations due to buyer's remorse. Once the order has been accepted, they are provisioned and made available for download. In implementing the above workflow various aspects are highlighted:


- The Sales endpoint illustrates the use of the Saga pattern to handle the buyer's remorse scenario.  
The CustomerRelations endpoint illustrates how in-memory events (domain events pattern) can be defined and subscribed to.

- The request/response pattern is illustrated for the video provisioning between the ContentManagement endpoint and the Operations Endpoint.
The ECommerce endpoint is implemented as an ASP.NET MVC4 application which uses SignalR to show feedback to the user. 

- This sample also illustrates the use of Unobtrusive message conventions to let NServiceBus know in order to identify commands, events and messages defined as POCOs which avoids having to add a reference to the NServiceBus libraries in the message definition dlls.

- The use of message headers and message mutator is also illustrated when the user clicks on the Check box on the ECommerce web page, which conveniently stops at the predefined breakpoints in the message handler code on the endpoints.

- The use of encryption is illustrated by passing in the Credit Card number and the Expiration date from the ECommerce web site. The Unobtrusive conventions defined in the ECommerce endpoint show how to treat certain properties as encrypted. Both the ECommerce and the Sales endpoint is setup for RijndaelEncryption and the encryption key is provided in the config file. If the messages are inspected in the queue, both the Credit Card number and the Expiration Date will show the encrypted values.  

##Sql Bridge (Transport Integration)

This sample shows how to setup a sql subscriber so it can subscribe to events from a Version 4.x MSMQ publisher. The solution comprises of these 5 projects.
**NOTE:** - This sample uses NHibernate persistence. It uses a database called, `PersistenceForMsmqTransport` for MSMQ transport endpoints and a different database called, `PersistenceForSqlTransport` for SQL Transport endpoints.

**Events** 

- Uses a POCO class for defining an event. 
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/Events/SomethingHappenedEvent.cs#L8](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/Events/SomethingHappenedEvent.cs#L6)

**MsmqPublisher** 

- Uses version 4.x of `NServiceBus.Host`, use unobtrusive conventions to consume the events, uses NHibernate subscription storage and publishes events. 
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/MsmqPublisher/EndpointConfig.cs#L10](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/MsmqPublisher/EndpointConfig.cs#L10)
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/MsmqPublisher/PublishEvent.cs#L19-20](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/MsmqPublisher/PublishEvent.cs#L21)

**MsmqSubscriber**
Uses the latest version of `NServiceBus.Host`, uses MsmqTransport and subscribes to the events from the `MsmqPublisher`


**SqlBridge** 

- Its an NServiceBus Host, uses the latest released version, and uses SqlTransport and NHibernate Persistence.
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/EndpointConfig.cs#L9](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/EndpointConfig.cs#L9)
- This endpoint is setup to read messages that arrive in a specified "MSMQ" Queue configured in app.config via an advanced satellite.
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/App.config#L18](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/App.config#L17)
- Create a transactional MSMQ called `SqlMsmqTransportBridge` or whatever queue name specified in the app.config. This will be the queue that the SqlBridge endpoint will look for events published by the MSMQ publisher.  
- Add a new entry in the Subscriptions collection for the new queue specified in the app.config to the list of subscribers in the MsmqPublisher's subscription storage. 

**How to add an additional entry to the existing list of subscribers?**
**If using RavenDB as the persistence:**

- To add the SqlMsmqTransportBridge as one of the subscribers, go to http://localhost:8080/raven/studio.html#/documents?database=MsmqPublisher

- Double click on the subscriptions document to open it.

- Add a new entry in the Clients section for SqlMsmqTransportBridge and press Save. For example, after adding, it would appear like this below:

```
{
  "MessageType": "Events.SomethingHappened, Version=0.0.0.0",
  "Clients": [
    {
      "Queue": "MsmqSubscriber",
      "Machine": "MachineName"
    }, 
        {
      "Queue": "SqlMsmqTransportBridge",
      "Machine": "MachineName"
    }
  ]
}
```

**If using NHibernate as the persistence:**
Run a similar script like below to add the new entry:

```
  Use PersistenceForMsmqTransport
  Go
  
  INSERT INTO Subscription
           ([SubscriberEndpoint]
           ,[MessageType]
           ,[Version]
           ,[TypeName])
     VALUES
           ('SqlMsmqTransportBridge@MachineName',
           'Events.SomethingHappened,0.0.0.0',
           '0.0.0.0',
           'Events.SomethingHappened')
  GO
```
where the `SqlMsmqTransportBridge` is the name of the queue that the SqlBridge will be watching. 

*How does the advanced satellite work?*

- It uses a MSMQ Dequeue strategy to read messages from its Input queue.
- References the message schema dll.
- The input queue is specified here (the value from app.config)
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/MsmqReceiver.cs#L67](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/MsmqReceiver.cs#L71)
- The MSMQ dequeue strategy is set here for reading messages from the queue (MSMQ)
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/MsmqReceiver.cs#L28-31](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/MsmqReceiver.cs#L34)
- The satellite will automatically process any message that is received in that queue (MSMQ).
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/MsmqReceiver.cs#L42-60](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlBridge/MsmqReceiver.cs#L42-64)
- The satellite will publish the received event. Since this endpoint uses SqlTransport, it will publish to its Sql queues. 


**SqlSubscriber** 

- Its an NServiceBus.Host, uses the latest released version, and subscribes to events from the SqlBridge.
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlSubscriber/EndpointConfig.cs#L9](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlSubscriber/EndpointConfig.cs#L9)
- References the schema dll.
- It setups to receive events from the SqlBridge in the app.config. The endpoint address is the sql bridge address and not the original publisher's address.
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlSubscriber/App.config#L9-14](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlSubscriber/App.config#L9-13)
- It has an event handler for the event that it wishes to process.
[https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlSubscriber/SomethingHappenedEventHandler.cs#L11-17](https://github.com/Particular/NServiceBus.SqlServer.Samples/tree/master/SqlBridge/SqlSubscriber/SomethingHappenedEventHandler.cs#L9-12)

**To summarize:**

1. Creating a new transactional queue that the MSMQ publisher will be sending its events to.

2. In the original MSMQ Publisher's Subscriptions storage, in addition to its list of all its current subscribers, has an additional entry for the queue that the SqlBridge will be listening to.

3. The Sql bridge endpoint (setup to read from that input queue) processes that message and publishes the event.

4. The Sql Subscriber, subscribes to the SqlBridge.

Once this is working, the steps of creating the queue and adding the additional subscription message in the subscriptions queue of the publisher can be automated for deployment. 


