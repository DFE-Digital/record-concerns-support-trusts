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

## Create razor pages
```
e.g. dotnet new page --name Cookies --namespace ConcernsCaseWork.Pages --output Pages
Link - https://www.learnrazorpages.com/
```

## Node and Design system Setup
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
```
Project configuration based on other internal streams for consistency.
Following development guidance
https://github.com/DFE-Digital/sdd-technical-documentation/blob/main/development_guidance/logging.md
```

## TRAMS API
```
TRAMS API is designed to replace dynamics365 were most of the legacy systems are feeding data from.

For integration with TRAMS API a member of the team will make available an API key (per environment).
Include header 'ApiKey' with the service key provided by TRAMS team.
```

### GitHub
[Here to go to TRAMS API repository](https://github.com/DFE-Digital/trams-data-api)

### Docker SQLServer
```
Based on the GitHub username configured to access TRAMS API repository, a few steps are required to
download the docker image from Container registry.
1. Docker installed on local machine
2. Authentication with ghcr.io
	1.1. Create a PAT (personal access token) https://docs.github.com/en/github/authenticating-to-github/keeping-your-account-and-data-secure/creating-a-personal-access-token
	1.2. Run command - $ echo $CR_PAT | docker login ghcr.io -u USERNAME --password-stdin
		 > Login Succeeded
3. Open docker container, should see a image running on port 1443, click on it, will open a log window, on the middle right top there is a button "inspect"
	click on it and will display the MSSQL_USER and MSSQL_SA_PASSWORD
4. Install SSMS or Azure Data Studio and Login using the credentials from step 3
```