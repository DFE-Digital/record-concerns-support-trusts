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
    azureLogin({ url, username, password }) {
      const cookie = azureLogin(url, username, password);

      return cookie;
    }
  })

  on('task', {

    log(message) {

      console.log(message)
      return null
    },
  })

  // Global cache for storing data across tests
  // Created when you run either cypress run or cypress open
  // Usage: For storing the auth cookie so we don't need to log in on every test
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

  config.baseUrl = config.env.url;

  return config;
}