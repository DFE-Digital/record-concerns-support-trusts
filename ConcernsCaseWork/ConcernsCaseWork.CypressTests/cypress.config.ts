import { defineConfig } from 'cypress'
import { CaseworkerClaim, EnvUsername, JwtSecretIssuer, JwtSecretKey } from 'cypress/constants/cypressConstants';
import { generateZapReport } from 'cypress/plugins/generateZapReport'
import jwt from 'jsonwebtoken';

export default defineConfig({
  defaultCommandTimeout: 20000,
  pageLoadTimeout: 20000,
  watchForFileChanges: false,
  chromeWebSecurity: false,
  video: false,
  retries: {
    runMode: 1
  },
  reporter: 'cypress-multi-reporters',
  reporterOptions: {
    reporterEnabled: 'mochawesome',
    mochawesomeReporterOptions: {
      reportDir: 'cypress/reports/mocha',
      quite: true,
      overwrite: false,
      html: false,
      json: true,
    },
  },
  userAgent: 'ConcernsCaseWork/1.0 Cypress',
  e2e: {
    // We've imported your old cypress plugins here.
    // You may want to clean this up later by importing these.
    setupNodeEvents(on, config) {

      on('before:run', () => {
        // Map cypress env vars to process env vars for usage outside of Cypress run environment
        process.env = config.env
      })
      on('task', {
        generateJwtToken() {
          // Fetch the secret key and roles from environment variables
          const secretKey = Cypress.env(JwtSecretKey); 
          const issuer = Cypress.env(JwtSecretIssuer); 
          if (!secretKey) {
            throw new Error('JWT secret key is not defined in cypress.env.json');
          } 
          if (!issuer) {
            throw new Error('JWT secret issuer is not defined in cypress.env.json');
          } 
          

          const payload = {
            userId: 123,
            username: Cypress.env(EnvUsername),
            roles: [CaseworkerClaim, ], // Add roles as a list to the payload
          };
          const options: jwt.SignOptions = {
            expiresIn: '1h',
            issuer: issuer,
          };

          // Generate the JWT token
          return jwt.sign(payload, secretKey, options);
        },
      });

      on('after:run', async () => {
        if (process.env.ZAP) {
          await generateZapReport()
        }
      })

      return require('./cypress/plugins/index.js')(on, config)
    },
  },
})