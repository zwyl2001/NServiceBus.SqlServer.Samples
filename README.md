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

This sample shows how to setup a sql subscriber so it can subscribe to events from a Version 3.3 MSMQ publisher. The solution comprises of these 5 projects

**Events** 

- Uses version 3.3 of `NServiceBus.Interfaces` and defines an event that implements IEvent
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/Events/SomethingHappenedEvent.cs#L8](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/Events/SomethingHappenedEvent.cs#L8)

**MsmqPublisher** 

- Uses version 3.3 of `NServiceBus.Host`, uses Msmq subscription storage and publishes events. Although this sample uses MSMQ Subscription storage, using either `NHibernate` or `RavenDB` subscription storage is recommended for production.
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/MsmqPublisher/EndpointConfig.cs#L9](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/MsmqPublisher/EndpointConfig.cs#L9)
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/MsmqPublisher/PublishEvent.cs#L19-20](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/MsmqPublisher/PublishEvent.cs#L19-20)

**MsmqSubscriber**
Uses the latest version of `NServiceBus.Host`, uses MsmqTransport and subscribes to the events from the `MsmqPublisher`


**SqlBridge** 

- Its an NServiceBus Host, uses the latest released version, and uses SqlTransport.
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/EndpointConfig.cs#L9](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/EndpointConfig.cs#L9)
- This endpoint is setup to read messages that arrive in a specified "MSMQ" Queue configured in app.config via an advanced satellite.
 [https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/App.config#L18](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/App.config#L18)
- Create a transactional MSMQ called `SqlMsmqTransportBridge` or whatever queue name specified in the app.config. This will be the queue that the SqlBridge endpoint will look for events published by the MSMQ publisher.  
- Add a new entry in the Subscriptions collection for the new queue specified in the app.config to the list of subscribers in the MsmqPublisher's subscription storage. 
  

*How does the advanced satellite work?*

- It uses a MSMQ Dequeue strategy to read messages from its Input queue.
- References the message schema dll.
- The input queue is specified here (the value from app.config)
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/MsmqReceiver.cs#L67](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/MsmqReceiver.cs#L67)
- The MSMQ dequeue strategy is set here for reading messages from the queue (MSMQ)
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/MsmqReceiver.cs#L28-31](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/MsmqReceiver.cs#L28-31)
- The satellite will automatically process any message that is received in that queue (MSMQ).
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/MsmqReceiver.cs#L42-60](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/MsmqReceiver.cs#L42-60)
- The satellite will publish the received event. Since this endpoint uses SqlTransport, it will publish to its Sql queues. 


**SqlSubscriber** 

- Its an NServiceBus.Host, uses the latest released version, and subscribes to events from the SqlBridge.
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/EndpointConfig.cs#L9](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/EndpointConfig.cs#L9)
- References the schema dll.
- It setups to receive events from the SqlBridge in the app.config. The endpoint address is the sql bridge address and not the original publisher's address.
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/App.config#L9-14](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/App.config#L9-14)
- It has an event handler for the event that it wishes to process.
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/SomethingHappenedEventHandler.cs#L11-17](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/SomethingHappenedEventHandler.cs#L11-17)

***NOTE***

Both the SqlSubscriber and the SqlBridge has asm redirects, so that it can use the latest version of NServiceBus. See:
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/App.config#L26-37](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlSubscriber/App.config#L26-37)
[https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/App.config#L28-39](https://github.com/Particular/NServiceBus.SqlServer.Samples/blob/SqlBridge/SqlBridge/SqlBridge/App.config#L28-39)

**To summarize:**

1. Creating a new transactional queue that the MSMQ publisher will be sending its events to.

2. In the original MSMQ Publisher's Subscriptions storage, in addition to its list of all its current subscribers, has an additional entry for the queue that the SqlBridge will be listening to.

3. The Sql bridge endpoint (setup to read from that input queue) processes that message and publishes the event.

4. The Sql Subscriber, subscribes to the SqlBridge.

Once this is working, the steps of creating the queue and adding the additional subscription message in the subscriptions queue of the publisher can be automated for deployment. 


