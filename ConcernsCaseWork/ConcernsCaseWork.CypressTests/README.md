## Cypress Testing

### Test Setup

The Cypress tests are designed to run against the front-end of the application. To set up the tests, you need to provide a configuration file named `cypress.env.json` with the following information:

```javascript
{
    "url": "<enter frontend URL>",
    "username": "<enter the user you want to run the tests with>",
    "password": "<enter the user's password>",
    "api": "<enter backend URL>",
    "apiKey": "<enter API key for backend>",
    "authKey": "<enter key set for the CypressTestSecret>"
}
```

While it is possible to pass these configurations through commands, it is easier to store them in the configuration file.

#### Authentication

There are two mechanisms of authentication supported:

1. Logging in using the Azure UI (Deprecated)
2. Using the `authKey` setting (Current method)

Both methods are included in the tests in case any problems arise. The authentication is invoked in every test using the `login` command:

```javascript
beforeEach(() => {
    cy.login();
});
```

The current method intercepts all browser requests and adds a special auth header using the `authKey`. Make sure you set the `CypressTestSecret` in your app, and it matches the `authKey` in the `cypress.env.json` file.

### Test Execution

If you have a `cypress.env.json` file, the `cy:open` and `cy:run` commands will automatically pick up the configuration.

Navigate to the `ConcernsCaseWork/ConcernsCaseWork.CypressTests/` directory:

```
cd ConcernsCaseWork/ConcernsCaseWork.CypressTests/
```

To open the Cypress Test Runner, run the following command:

```
npm run cy:open
```

To execute the tests in headless mode, use the following command (the output will log to the console):

```
npm run cy:run
```

### Test linting

We have set up [eslint](https://eslint.org) on the Cypress tests to encourage code quality. This can be run by using the script `npm run lint`

Currently, all rules are set to warnings rather than errors. We will be looking to move these to errors long-term.

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

### Accessibility Testing

The `executeAccessibilityTests` command is implemented in Cypress and is used to perform accessibility tests on a web application. It utilises the Axe accessibility testing library to check for accessibility issues based on the specified criteria.

#### Usage

To use this command, simply call `executeAccessibilityTests()` in your Cypress test code. Here's an example:

```javascript
it("should perform accessibility tests", () => {
  // Perform actions and assertions on your web application
  // ...

  // Execute accessibility tests
  cy.executeAccessibilityTests();

  // Continue with other test logic
  // ...
});
```

#### Command Details

The `executeAccessibilityTests` command under "support/commands.ts" performs the following steps:

1. Logs a message to indicate that the command is being executed.
2. Sets the WCAG standards to be checked. In this case, it uses the WCAG 2.2 AA standard.
3. Defines the impact levels to include in the accessibility tests. The impact levels can be "critical", "minor", "moderate", or "serious".
4. Logs a message to indicate that the Axe accessibility library is being injected.
5. Injects the Axe library into the web application under test.
6. Logs a message to indicate that the accessibility check is being performed.
7. Executes the accessibility check using the `checkA11y` command provided by Cypress. It configures the test to run only for the specified WCAG standards and included impact levels.
8. Optionally, you can choose to continue running the tests even if there are failures by setting `continueOnFail` to `true`. By default, it is set to `false`.
9. Logs a message to indicate that the command has finished.

#### Note

Make sure you have the necessary dependencies and configurations set up to use Cypress and the Axe accessibility library before using the `executeAccessibilityTests` command.

It is recommended to customise the command based on your specific accessibility testing needs. You can add additional parameters or modify the WCAG standards and impact levels as required by your project.

Remember to interpret and handle the test results appropriately based on your project requirements. The accessibility check will provide feedback on any accessibility issues found in your web application.

Ensure that you have a thorough understanding of the WCAG standards and impact levels to effectively assess the accessibility of your application. Consider addressing any accessibility issues identified during the testing process to improve the overall accessibility and user experience.

Documentation and resources on Cypress and the Axe accessibility library can be found in their respective official documentation, which should be referenced for detailed information and guidance.

By incorporating accessibility testing into your Cypress tests, you can help ensure that your web application is accessible to a wide range of users, including those with disabilities.
