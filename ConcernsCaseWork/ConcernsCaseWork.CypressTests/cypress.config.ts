import { defineConfig } from 'cypress'
import { generateZapReport } from 'cypress/plugins/generateZapReport'

export default defineConfig({
  defaultCommandTimeout: 20000,
  pageLoadTimeout: 20000,
  watchForFileChanges: false,
  chromeWebSecurity: false,
  video: false,
  retries: 1,
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

      on('after:run', async () => {
        if (process.env.ZAP) {
          await generateZapReport()
        }
      })

      return require('./cypress/plugins/index.js')(on, config)
    },
  },
})
