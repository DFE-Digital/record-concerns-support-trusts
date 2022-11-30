# Concerns Casework API
This API services all requests for data specific to Concerns Casework. 

## Run the API locally

### Secret storage
***
Note that these secrets are 
In a console window:
1. Navigate to ```ConcernsCaseWork``` project root
1. Run 	```dotnet user-secrets init``` to initialise secrets in this directory
1. Run the following to create API key secret:
```dotnet user-secrets set "ConcernsCaseworkApi:ApiKeys" "comma-separated list of accepted api keys" ```
1. Set the database connection string in user secrets: ```dotnet user-secrets set "ConnectionStrings:DefaultConnection" "enter connection string to database here" ```

### SQL Server Database
***
The API uses Entity Framework to manage the database.
To create the database, or to apply the latest migrations:

In a console window: 
1. Navigate to ```ConcernsCaseWork.Data``` project root
1. Run migrations ```dotnet ef database update --connection "enter connection string to database here" ```


## Accessing the API

This API has a swagger page accessible at <url>/swagger, which lists all available endpoints. 

Authorisation is handled by an api key sent in the HTTP request header ```'ApiKey'```. 

This api key should be one of the comma-separated list of valid keys which is held in the ```ConcernsCaseworkApi:ApiKeys``` property.

Use this api key in the 'Authorise' option in the swagger page to run commands directly against the API. 

Alternatively, include an HTTP request header of ```'ApiKey'``` in any requests to this API, such as from Postman.
