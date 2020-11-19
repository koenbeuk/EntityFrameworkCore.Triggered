# Hello world
A sample application showing triggers integrated with serverside Blazor. The Counter component as provided by the default Blazor template has been updated to be persisted in a database and show a timestamp of when the last time the counter got updated.

Additionally, A trigger is used to publish an event whenever a counter gets updated, which then re-renders active Counter components, try opening 2 different browser windows.

## Triggers

- Counts.SetCreatedOn: This trigger ensures that Count records always have the CreatedOn property set to the moment of creation.
- Counts.PublishCountAddedEvent: This trigger publishes an event informing the Counter component to register the new vote and rerender.


## Build and run this sample
Run `dotnet run` in the root of this project 
