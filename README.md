﻿# Concerns Casework
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
```
[Razor pages tutorial](https://www.learnrazorpages.com/)

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
```
[GitHub Repository](https://github.com/DFE-Digital/sdd-technical-documentation/blob/main/development_guidance/logging.md)

## TRAMS API
```
TRAMS API is designed to replace dynamics365 were most of the legacy systems are feeding data from.

For integration with TRAMS API a member of the team will make available an API key (per environment).
Include header 'ApiKey' with the service key provided by TRAMS team.
```
[GitHub Repository](https://github.com/DFE-Digital/trams-data-api)

## Docker SQLServer
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

### Docker Application Image
```
Testing docker image locally,
1. Have Docker running
2. Build image from command line
	2.1 docker build . -t amsd-casework
3. Verify image produced
	3.1 docker images
4. Run docker image
	4.1 docker run -e -p 8080:80
5. Browse localhost:8080
```

## PaaS Account
```
The section is well-described in the playbook, link to oficial documentation
Note: Don't enable SSO in your account if you are using the account credentials to login to PaaS.
```
[Gov PaaS](https://docs.cloud.service.gov.uk/get_started.html?_ga=2.255108360.1068852604.1627038231-1095670286.1624019946#get-started)

### Useful Cloud Foundry Commands
```
cf help
cf spaces
cf login -a api.london.cloud.service.gov.uk -u
cf logout
cf target -o dfe -s amsd-casework-dev --> switch spaces
cf logs --recent amsd-casework-dev --> see logs
cf service amsd-casework-tf-state --> AWS S3 terraform state
```

## Terraform AWS S3 storage
After some research two options are available, AWS S3 (no versioning) and Azure (with versioniong)
Until Concerns as an Azure account we will use AWS S3 bucket to store terraform state.

### Cloud Foundry Commands
```
cf target -o dfe -s amsd-casework-dev
cf create-service aws-s3-bucket default amsd-casework-tf-state
cf create-service-key amsd-casework-tf-state amsd-casework-tf-state-key -c '{"allow_external_access": true}'
cf service-key amsd-casework-tf-state amsd-casework-tf-state-key
```
[Gov PaaS](https://docs.cloud.service.gov.uk/deploying_services/s3/#amazon-s3)

### Terraform Local
```
If you want to run terraform locally to test the scripts a few steps are required,
1º Create new profile under ~/.aws/credentials e.g. [dfe]
2º Request bucket credentials and update step 1
3º Run the comand,
	terraform init \
    	-backend-config="bucket=<BUCKET_NAME>>" \
    	-backend-config="key=<KEY>>" \
    	-backend-config="region=eu-west-2" \
    	-backend-config="profile=dfe"
```