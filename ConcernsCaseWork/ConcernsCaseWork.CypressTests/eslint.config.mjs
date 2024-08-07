export default [
  {
    files: [
      ".js", ".jsx", ".ts", ".tsx"
    ],
    parser: "@typescript-eslint/parser",
    plugins: [
      "cypress"
    ],
    rules: {
      "cypress/no-assigning-return-values": "warn",
      "cypress/no-unnecessary-waiting": "warn",
      "cypress/assertion-before-screenshot": "warn",
      "cypress/no-force": "warn",
      "cypress/no-async-tests": "warn",
      "cypress/no-pause": "warn",
      "cypress/unsafe-to-chain-command": "warn"
    },
    env: {
      "cypress/globals": true
    },
    extends: [
      "plugin:cypress/recommended"
    ]
  }
]
