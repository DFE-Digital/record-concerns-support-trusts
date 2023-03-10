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