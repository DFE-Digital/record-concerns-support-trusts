# Concerns Casework
The Department for Education (DFE) is the UK government department responsible for child protection, 
education (compulsory, further and higher education), apprenticeships and wider skills in England.
Concerns Casework is the activity undertaken by both AMSD and RDD to help Trusts and Academies resolve issues
with financial management, governance, irregularity, and educational performance.

Trusts are required to adhere to the terms laid out in the DFE's financial and governance handbooks
as well as their Funding Agreement which is a document that lays out the terms of the funding they
receive from us to run their Academies.

If a Trust does falls foul of this and the DFE either notices financial irregularities from it's own
monitoring or is alerted to issues by external or internal parties a Concerns Case is created.

## Glossary
***
```
FNTI - Finantial notice to improve
ESFA - Education and skills funding agency
SRMA - School resource management adviser
GIAS - Get information about schools
FSG - Free School Group
EDPERF - Education Performance
KIM aka IFD - Infrastructure Funding Directorate
DART - Data and Reporting Tool
CRM - Dynamics
AMSD - Academies & Maintained Schools Directorate
RDD - Regional Delivery Directorate
```

## Local Setup

### Building the project Nuget proxy error
***
```
The issue is a environment variable name HTTP_PROXY. Remove it from the system, restart IDE and re-try.
```

## Development Setup - Docker compose :star2: NEW :star2: 
 
1 - Bring up the dev stack:
  - Navigate to the root directory of the project
  - Run the command `docker-compose -f Stack/docker-compose.yml up -d --build`. 

        - This will bring up the sql sever with a username and password set.
        - Create an empty database.
        - Apply migrations.
        - Start redis

2 - Run `npm install; npm run build` from the `ConcernsCaseWork/wwwroot` directory to build the styles.

3 - Open the project in Visual studio :rocket:

# Adding migrations

- In command line navigate to ConcernsCaseWork.Data
- Issue the command `dotnet ef migrations add <MigrationName>`
- After making changes to run the migration run the command `dotnet ef database update --context ConcernsDbContext --connection "<db connection string>"`


### Docker Redis
***
```
Running web application locally will need a Docker redis instance runnning.
docker run -p 6379:6379 --name redis -d redis
redis://user:password@localhost:6379
```

### Secret storage
***
```
Secret storage is used only for local development avoiding adding secure properties
into appsettings files.

dotnet user-secrets init
The preceding command adds a UserSecretsId element within a PropertyGroup of the project file. 
By default, the inner text of UserSecretsId is a GUID. The inner text is arbitrary, but is unique to the project.

NOTE:: Setup is done under the ConcernsCaseWork project root.

List of secrets:
dotnet user-secrets list

Set a secret:
dotnet user-secrets set "trams:api_endpoint" "secret_here"
dotnet user-secrets set "trams:api_key" "secret_here"
dotnet user-secrets set "app:username" "secret_here"
dotnet user-secrets set "app:password" "secret_here"
dotnet user-secrets set "VCAP_SERVICES" "{'redis': [{'credentials': {'host': '127.0.0.1','password': 'password','port': '6379','tls_enabled': 'false'}}]}"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=sip;Integrated Security=true"
dotnet user-secrets set "ConcernsCaseworkApi:ApiKeys" "app-key"
dotnet user-secrets set "ConcernsCasework:ApiKey" "app-key"
dotnet user-secrets set "ConcernsCasework:ApiEndpoint" "https://localhost"
dotnet user-secrets set "AzureAd:ClientSecret" "secret_here"
dotnet user-secrets set "AzureAd:ClientId" "secret_here"
dotnet user-secrets set "AzureAd:TenantId" "secret_here"
dotnet user-secrets set "CypressTestSecret" "secret that matches the key in the cypress test suite"
dotnet user-secrets set "AzureAdGroups:CaseWorkerGroupId" "secret_here"
dotnet user-secrets set "AzureAdGroups:TeamleaderGroupId" "secret_here"
dotnet user-secrets set "AzureAdGroups:AdminGroupId" "secret_here"

Remove a secret:
dotnet user-secrets remove "app:username"

Remove all secrets:
dotnet user-secrets clear
```
[Microsoft page](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows)

### Concerns CaseWork SQL Server Database
***
The API uses Entity Framework to manage the database.
To create the database, or to apply the latest migrations:

In a console window: 
1. Navigate to ```ConcernsCaseWork.Data``` project root
1. Run migrations ```dotnet ef database update --connection "enter connection string to database here" ```

## Concerns Casework Docker images
There are 4 docker images configured in amsd-casework:
- webapi - UI and API
- sqlcmd - creates initial database and then exits
- sql-server - contains Concerns Casework database
- redis - redis cache

To update the config files, run the following:
```cp .env.database.example .env.database```
```cp .env.development.local.example .env.development.local```

Update config values in .env.database and .env.development.local as necessary

Then run the following to build and start the docker containers:
```docker-compose -f docker-compose.yml up --build```

Note that you may need to update the line feed character type to LF if running on Windows, on files in the /scripts folder. 
Try this if the webapi container exits with error.

## Login
***
```
Active Directory.
```

## Create razor pages
***
```
e.g. dotnet new page --name Cookies --namespace ConcernsCaseWork.Pages --output Pages
```
[Razor pages tutorial](https://www.learnrazorpages.com/)

## Node and Design system Setup
***
```
https://design-system.service.gov.uk/community/resources-and-tools/
Go to project root -> ConcernsCaseWork directory -> wwwroot directory and run the command,
npm install
npm run build -> production
npm run build:dev -> development

The command execution pulls from node_modules all the required dependencies, scss, js to be able
to run the razor web app frontend and moj components.

JQuery dependency was manually added don't remove from dist folder or if you find an automated way 
of bringing the library over to the project update this document.

```

### Linting Sonar rules

Include the following extension in your IDE installation: [SonarQube for IDE](https://marketplace.visualstudio.com/items?itemName=SonarSource.sonarlint-vscode)

Update your [settings.json file](https://code.visualstudio.com/docs/getstarted/settings#_settings-json-file) to include the following

```json
"sonarlint.connectedMode.connections.sonarcloud": [   
    {
        "connectionId": "DfE",
        "organizationKey": "dfe-digital",
        "disableNotifications": false
    }   
]
```

Then follow [these steps](https://youtu.be/m8sAdYCIWhY) to connect to the SonarCloud instance.
