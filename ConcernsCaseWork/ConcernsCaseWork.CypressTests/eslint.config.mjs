// eslint.config.mjs
import typescriptParser from "@typescript-eslint/parser"
import pluginCypress from 'eslint-plugin-cypress/flat'

export default [
  pluginCypress.configs.globals,
  pluginCypress.configs.recommended,
  {
    files: [
      "cypress/**/*.{js,jsx,ts,tsx}"
    ],
    languageOptions: {
      parser: typescriptParser
    },
    plugins: {
      cypress: pluginCypress
    }
  }
]
