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

