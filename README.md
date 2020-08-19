# GeoTaskManager
![Deploy demo to Azure Web App](https://github.com/artem-kuskov/GeoTaskManager/workflows/Deploy%20demo%20to%20Azure%20Web%20App/badge.svg)

> Task Management project extended with 2D geospatial coordinates and queries.

The app, additionally to standard task management features, allows set multiple collections of 2D-coordinates of points, lines, and polygons for tasks and query them by interception with 2D-box bounds. It helps to visualize the position, area, and path of the tasks on maps, building plans, or, for example, on design layouts. 

You can try the REST API and OpenAPI (Swagger) specification at [Demo site](https://geotaskmanagerapi20200816172751.azurewebsites.net/swagger/)

(The API uses OAuth2 authorization, and to get demo access, click **Authorize** button at the top of Demo site, insert the line showed below in the form that opens, and then press **Login** to have full rights while working with the API)
> Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuaWNrbmFtZSI6ImEua3Vza292IiwibmFtZSI6ImEua3Vza292QGJrLnJ1IiwicGljdHVyZSI6Imh0dHBzOi8vcy5ncmF2YXRhci5jb20vYXZhdGFyLzZmNjkzMzlhYWJmYjZjMGU5OWUwNDQzNzRjNzRkZTJlP3M9NDgwJnI9cGcmZD1odHRwcyUzQSUyRiUyRmNkbi5hdXRoMC5jb20lMkZhdmF0YXJzJTJGYS5wbmciLCJ1cGRhdGVkX2F0IjoiMjAyMC0wOC0xN1QxNDo0MTo0NC41MDVaIiwiZW1haWwiOiJhLmt1c2tvdkBiay5ydSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJpc3MiOiJodHRwczovL2dlb3Rhc2ttYW5hZ2VyLmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJhdXRoMHw1ZWUxZjVjNzZjYzQzOTAwMTRjMzQ0ZDIiLCJhdWQiOiJZN0ZtOVh5enFVWUkwMlUyNk14SnZNWGdQMGxGODlmSCIsImlhdCI6MTU5NzY3NTMwNCwiZXhwIjoxNjUwMDAwMDAwfQ.R729_qPYrVyGZnL9CjUyyAhsbPnqjOW8B0QZC9_JwG4

## Architecture
The REST API backend is currently being developed in ASP.NET Core 3.1. The MongoDb cluster is used as the database.

Authentification is implemented by external OAuth2 providers and can be configurated.

The solution architecturally consists of the layers:
* Logic layer (or Domain in terms of DDD) where GeoTaskManager.Application project is responsible for the implementation of business logic.
* Interface (or Application in terms of DDD) layer includes the GeoTaskManager.Api project which interacts with external API requests.
* Infrastructure layer (GeoTaskManager.MongoDb project) implements the MongoDb data storage interface.

The layers are loosely coupled by using Mediator pattern, which routes requests, commands, and events.

Queries, commands, and models are checked with FluentValidation library at layers' boundaries.

## Domain Logic

A task has one responsible actor, several assistants and observer actors, description, GeoJson attributes, deadline, current execution status, and the history of changes. A task has implementation status: **New, InWork, FinishRequested, Finished, CancelRequested, Canceled**.

Tasks are grouped into projects.  The project has a visual layer which can be image, map, or GeoJSON collection to use as a background for the tasks. The project contains a list of Project Roles for some actors to extend their rights in that project.

The actors (users) have two levels of roles:
* Global roles are applied to every project and task
* Project roles are applied only to the specific project.

|	Global Role	|	Rights												|
|---------------|-------------------------------------------------------|
|	Admin		|	Can view, add, change and remove other actors;		|
|				|	view, add, change and remove all projects;			|
|				|	view, add, change and remove all tasks;				|
|				|	view, add, change and remove all geospatial entities|
|				|
|				|	Can not remove himself or change self global role	|
|				|
|	Manager		|	Can view all actors;								|
|				|	view, add, change and remove all projects;			|
|				|	view, add, change and remove all tasks;				|
|				|	view, add, change and remove all geospatial entities|
| |
|				|	Can not add, change or remove any actors			|
|				|
|	Actor		|	Can view all actors;								|
|				|	view all projects;									|
|				|	view only the tasks where he participates; 			|
|				|	change status **New -> InWork, CancelRequest, CompleteRequest; InWork -> CancelRequest, CompleteRequest** in the task where he is a responsible actor;		|
|				|	view all geospatial entities						|
|				|
|				|	Can not add, change or remove actors, projects, tasks, geospatial entities, view other actors' tasks|

|	Project Role|	Rights												|
|---------------|-------------------------------------------------------|
|	Admin		|	Can change the project role of other actors;		|
|				|	view, change and remove the project;				|
|				|	view, add, change and remove all 					|
|				|		tasks in the project;							|
|				|	view, add, change and remove all 					|
|				|		geospatial entities in the project				|
||
|				|	Can not change himself project role					|
||
|	Manager		|	Can view, add, change and remove all tasks 			|
|				|		in the project;									|
|				|	view, add, change and remove all 					|
|				|		geospatial entities in the project				|
||
|				|	Can not add, change or remove any actors			|

Geos (geospatial) entities contain primitives in GeoJSON specification. They can be linked to tasks and project background layer.

## Application settings

File appsettings.json contains some parameters to configure application.


```sh
{
  "ConnectionStrings": {

    // Connection string to MongoDb storage
    "MongoDbConnection": "mongodb://localhost:27017/?appname=GeoTaskManagerApi&ssl=false",

    // Database name at data storage
    "MongoDbName": "geotaskmanager"
  },

  "ApiConstraints": {

    // Max entity count API can return in one response
    "MaxEntityCollectionSize": 100,

    // Default entity count API will return 
    // when Limit parameter in API request is empty
    "DefaultEntityCollectionSize": 20
  },

  "SeedData": {

    // Actor with that account will be added as global Admin 
    // when there is no any other actor
    "AdminActorLogin": "INSERT_ADMIN_LOGIN_FROM_AUTHENTIFICATION_SERVICE",
    "AdminActorEmail": "INSERT_ADMIN_EMAIL"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },

  // Configuration for external OAuth2 provider
  "OAuth20": {
    "Issuer": "INSERT_YOUR_JWT_ISSUER",
    "Audience": "INSERT_YOUR_JWT_AUDIENCE",
    "UserNameClaim": "name",
    "UserRoleClaim": "role",
    "Secret": "INSERT_YOUR_AUTH_SERVICE_SECRET_KEY"

    // Uncomment to check user e-mail verification in authorization service
    // if it supports
    /* 
      "EmailVerificationClaimName": "email_verified",
      "EmailVerificationClaimValue": "true"
    */
  }
}
```

## Release History

* 1.0.0
    * REST API backend (ASP.Net Core 3.1 + MongoDb cluster)

## Road Map

* REST API backend (ASP.Net Core 3.1 + MongoDb cluster) (Completed in 1.0.0)
* Add Redis cache for API response acceleration
* Add SignalR service for tasks realtime monitoring. Connect the service to the backend by message query broker (Redis, RabbitMQ?)
* Add web frontend (Angular, Blazor?)
* Add mobile application

## Meta

Artem Kuskov – [Linked.in](https://www.linkedin.com/in/artem-kuskov/) – a.kuskov@bk.ru
