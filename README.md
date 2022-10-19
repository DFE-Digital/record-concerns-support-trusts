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
dotnet user-secrets set "app:username" "secret_here" --> Store a list comma separated users e.g.  dotnet user-secrets set "app:username" "Concerns.casework,e2e.cypress.test,ben.memmott,richard.machen,elijah.aremu,paul.simmons,james.cheetham,christian.gleadall,philip.pybus,emma.wadsworth,israt.choudhury,chanel.diep,magdalena.bober,case.worker1,case.worker2,emma.whitcroft,chris.dexter,samantha.harbison,mara.ashraf,jane.dickinson,fahad.darwish,mike.stock,judy.cheung,shad-carine.ohayon,deaglan.lloyd,joe.peffers,ayesha.rahman,mohammed.hoque,carl.richmond,sue.randall,jenny.cheetham,simon.ellis,terry.jones,forrest.mcdonald,sham.choudhury,tracey.eason,john.russell,rebecca.green,tracey.carter,riffat.jabeen,natasha.walters,ben.hodgkins,teresa.phillipson,michael3.marshall,julia.paton,kirsty.boxall,simon.wadsworth,reshma.chetty,alison.oliver,josephine.holloway,lee3.turner,alastair.dawson,molly.quinn,laura.bridge,mark.holt,nicky.shue,ralph.day,maureen.sammon"
dotnet user-secrets set "app:password" "secret_here"
dotnet user-secrets set "VCAP_SERVICES" "{'redis': [{'credentials': {'host': '127.0.0.1','password': 'password','port': '6379','tls_enabled': 'false'}}]}"
dotnet user-secrets set "ConcernsCaseworkApi:ApiEndpoint" "https://localhost:3001"
dotnet user-secrets set "ConcernsCaseworkApi:ApiKey" "secret_here" 

Remove a secret:
dotnet user-secrets remove "app:username"

Remove all secrets:
dotnet user-secrets clear
```
[Microsoft page](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows)

### Build Node Dependencies
***
```
npm run build
```

### Docker SQLServer
***
```
Based on the GitHub username configured to access TRAMS API repository, a few steps are required to
download the docker image from Container registry.
1. Docker installed on local machine
2. Authentication with ghcr.io
	2.1. Create a PAT (personal access token) https://docs.github.com/en/github/authenticating-to-github/keeping-your-account-and-data-secure/creating-a-personal-access-token
	2.2. $ export CR_PAT=YOUR_TOKEN
		 $ echo $CR_PAT | docker login ghcr.io -u USERNAME --password-stdin
	2.3. After Login successfully with Docker account
		 > Check TRAMM API https://github.com/DFE-Digital/trams-data-api -> or run the next command
		 > docker run -d -p 1433:1433 ghcr.io/dfe-digital/trams-development-database:latest
3. Open docker container, should see a image running on port 1443, click on it, will open a log window, on the middle right top there is a button "inspect"
	click on it and will display the MSSQL_USER and MSSQL_SA_PASSWORD
4. Install SSMS or Azure Data Studio and Login using the credentials from step 3
5. e.g. SQLServer Management Studio
	> Server name: localhost or 127.0.0.1
	> Select SQLServer Authentication
		> Login: Get from Docker inspect
		> Password: Get from Docker inspect 
```

### Docker Application Image
***
```
Testing docker image locally,
1. Check Docker running
2. Build image from command line
	2.1 docker build . -t amsd-casework
3. Verify image produced
	3.1 docker images
4. Run docker image
	4.1 docker run -e -p 8080:80
5. Browse localhost:8080
```

## Login
***
```
Request login credentials within the team.
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

## Logging Configuration
***
```
Project configuration based on other internal streams for consistency.
```
[GitHub Repository](https://github.com/DFE-Digital/sdd-technical-documentation/blob/main/development_guidance/logging.md)

## Academies API
***
```
Academies API is designed to replace dynamics365 were most of the legacy systems are feeding data from.

For integration with TRAMS API a member of the team will make available an API key (per environment).
Include header 'ApiKey' with the service key provided by TRAMS team.
```
[GitHub Repository](https://github.com/DFE-Digital/trams-data-api)

## PaaS Account
***
```
The section is well-described in the playbook, link to oficial documentation
Note: Don't enable SSO in your account if you are using the account credentials to login to PaaS.
```
[Gov PaaS](https://docs.cloud.service.gov.uk/get_started.html?_ga=2.255108360.1068852604.1627038231-1095670286.1624019946#get-started)

### Redis
***
```
Redis resource is created via terraform and bind to the app.
Eviction policy is set by default to volatile-lru

Volatile-lru:
evict keys by trying to remove the least recently used (LRU) keys first, 
but only among keys that have an expire set, in order to make space for the new data added
```

### Useful Cloud Foundry Commands
***
```
cf help
cf spaces
cf login -a api.london.cloud.service.gov.uk
cf logout
cf target -o dfe -s amsd-casework-dev --> switch spaces
cf logs --recent amsd-casework-dev --> see logs
cf service amsd-casework-tf-state --> AWS S3 terraform state
cf stop amsd-casework-dev
cf delete -r amsd-casework-dev
cf env amsd-casework-dev --> see environment variables of the target space
cf set-space-role USERNAME ORGNAME SPACE ROLE --> Grant roles to user
cf env <APP_NAME> --> See environment variables

cf install-plugin conduit
cf conduit amsd-casework-redis-dev --> Run Redis locally
cf conduit amsd-casework-redis-dev -- redis-cli
```
[Redis-CLI](https://redis.io/download)

## Terraform AWS S3 storage
***
After some research two options are available, AWS S3 (no versioning) and Azure (with versioniong)
Until Concerns as an Azure account we will use AWS S3 bucket to store terraform state.

### Cloud Foundry Commands
***
```
cf target -o dfe -s amsd-casework-dev
cf create-service aws-s3-bucket default amsd-casework-tf-state
cf create-service-key amsd-casework-tf-state amsd-casework-tf-state-key -c '{"allow_external_access": true}'
cf service-key amsd-casework-tf-state amsd-casework-tf-state-key
```
[Gov PaaS](https://docs.cloud.service.gov.uk/deploying_services/s3/#amazon-s3)

## Cypress testing

### Test execution
The Cypress tests will run against the front-end of the application, so the credentials you provide below should be of the user that is set up to run against the UI.

To execute the tests locally and view the output, run the following:

```
cd ConcernsCaseWork/ConcernsCaseWork.CypressTests/
```

Followed by:

```
npm run cy:open -- --env apiKey='APIKEY',api='TRAMS_BASE_URL',username='USERNAME',password='PASSWORD',url="BASE_URL_OF_APP"
```

To execute the tests in headless mode, run the following (the output will log to the console):

```
npm run cy:run -- --env apiKey='APIKEY',api='TRAMS_BASE_URL',username='USERNAME',password='PASSWORD',url="BASE_URL_OF_APP"
```

### Useful tips

#### Maintaining sessions
Each 'it' block usually runs the test with a clear cache. For our purposes, we may need to maintain the user session to test various scenarios. This can be achieved by adding the following code to your tests:

```
afterEach(() => {
		cy.storeSessionData();
	});
```

##### Writing global commands
The cypress.json file in the `support` folder contains functions which can be used globally throughout your tests. Below is an example of a custom login command

```
Cypress.Commands.add("login",()=> {
	cy.visit(Cypress.env('url')+"/login");
	cy.get("#username").type(Cypress.env('username'));
	cy.get("#password").type(Cypress.env('password')+"{enter}");
	cy.saveLocalStorage();
})

```

Which you can access in your tests like so:

```
before(function () {
	cy.login();
});
```

Further details about Cypress can be found here: https://docs.cypress.io/api/table-of-contents
