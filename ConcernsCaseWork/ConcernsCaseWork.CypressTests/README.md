## Cypress testing

### Test setup
The Cypress tests will run against the front-end of the application

To acheive this you will need the following configuration named cypress.env.json
You can also pass these in the commands, but its far easier in a config

```
{
    "url": "<enter frontend url>",
    "username": "<enter the user you want to run the tests with>",
    "password": "<enter the users password>",
    "api": "<enter backend url>",
    "apiKey": "<enter api key for backend>",
    "authKey": "<enter key set for the CypressTestSecret>"
}
```

#### Authentication
We have two mechanisms of authentication supported
- Logging in using the azure UI (Deprecated)
- Using the authKey setting (Current method)

Both have been left in just in case a problem occurs later down the line

They are invoked in every test by the login command:

```
	beforeEach(() => {
		cy.login();
	});
```

The current method will intercept all browser requests and add the special auth header using the authKey
You must set the CypressTestSecret in your app and it must match the authKey in the cypress env

### Test execution

As mentioned above if you use a cypress.env.json the cy:open and cy:run commands will pickup the configuration automatically

```
cd ConcernsCaseWork/ConcernsCaseWork.CypressTests/
```

Followed by:

```
npm run cy:open
```

To execute the tests in headless mode, run the following (the output will log to the console):

```
npm run cy:run
```

### Security testing with ZAP

The Cypress tests can also be run, proxied via [OWASP ZAP](https://zaproxy.org) for passive security scanning of the application.

These can be run using the configured `docker-compose.yml`, which will spin up containers for the ZAP daemon and the Cypress tests, including all networking required. You will need to update any config in the file before running

Create a `.env` file for docker, this file needs to include

- all of your required cypress configuration
- HTTP_PROXY e.g. http://zap:8080
- ZAP_API_KEY, can be any random guid

Example env:

```
URL=<Enter URL>
USERNAME=<Enter username>
API=<Enter API>
API_KEY=<Enter API key>
AUTH_KEY=<Enter auth key>
HTTP_PROXY=http://zap:8080
ZAP_API_KEY=<Enter random guid>

```

**Note**: You might have trouble running this locally because of docker thinking localhost is the container and not your machine

To run docker compose use:

`docker-compose -f docker-compose.yml --exit-code-from cypress`

**Note**: `--exit-code-from cypress` tells the container to quit when cypress finishes

You can also exclude URLs from being intercepted by using the NO_PROXY setting

e.g. NO_PROXY=google.com,yahoo.co.uk

Alternatively, you can run the Cypress tests against an existing ZAP proxy by setting the environment configuration

```
HTTP_PROXY="<zap-daemon-url>"
NO_PROXY="<list-of-urls-to-ignore>"
```

and setting the runtime variables

`zapReport=true,zapApiKey=<zap-api-key>,zapUrl="<zap-daemon-url>"`