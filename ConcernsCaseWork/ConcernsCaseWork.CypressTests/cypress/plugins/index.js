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
module.exports = (on, config) => {
    const cache = {};

    // `on` is used to hook into various events Cypress emits
    // `config` is the resolved Cypress config
    on('task', {
        azureLogin({ url, username, password }) {
            const cookie = azureLogin(url, username, password);

            return cookie;
        },
    });

    on('task', {
        log(message) {
            console.log(message);
            return null;
        },
    });

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

            const result = value ? value : null;

            return result;
        },
    });

    config.baseUrl = config.env.url;

    // Username for case-owner assertions (Cypress.env('username')):
    // - E2E_USERNAME set (pipeline or env): use it.
    // - Otherwise, if not running against localhost, assume pipeline E2E and use the service account.
    // - Otherwise use cypress.env.json "username" (e.g. "cypress") for local runs.
    const url = (config.env.url || config.baseUrl || '').toLowerCase();
    const isLocal = url.includes('localhost') || url.startsWith('https://localhost') || url.startsWith('http://127.0.0.1');
    const pipelineE2eUsername = 'svc-rdscc-e2etest@education.gov.uk';

    if (process.env.E2E_USERNAME) {
        config.env.username = process.env.E2E_USERNAME;
    } else if (!isLocal) {
        config.env.username = pipelineE2eUsername;
    }

    return config;
};
