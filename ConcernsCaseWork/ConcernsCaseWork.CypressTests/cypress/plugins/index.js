/// <reference types="cypress" />

// ***********************************************************
// This example plugins/index.js can be used to load plugins
//
// You can change the location of this file or turn off loading
// the plugins file with the 'pluginsFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/plugins-guide
// ***********************************************************

// This function is called when a project is opened or re-opened (e.g. due to
// the project's config changing)

const azureLogin = require('./azureLogin').azureLogin;

/**
 * @type {Cypress.PluginConfig}
 */
// eslint-disable-next-line no-unused-vars
module.exports = (on, config) => {

  const cache = {};

  // `on` is used to hook into various events Cypress emits
  // `config` is the resolved Cypress config
  on('task', {
    azureLogin: azureLogin
  })

  on('task', {

    log(message) {

      console.log(message)
      return null
    },
  })

  on('task', {
     setGlobalCacheItem({ name, value }) {
      cache[name] = value;

      return null;
    },
    getGlobalCacheItem(name) {
      const value = cache[name];

      const result = (value) ? value : null;

      return result;
    }
  })
}