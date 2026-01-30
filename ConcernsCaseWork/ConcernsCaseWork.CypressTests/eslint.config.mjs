import eslintPluginCypress from 'eslint-plugin-cypress/flat';
import tsParser from '@typescript-eslint/parser';

export default [
    eslintPluginCypress.configs.recommended,
    {
        files: ['**/*.js', '**/*.jsx', '**/*.ts', '**/*.tsx'],
        languageOptions: {
            parser: tsParser,
            globals: {
                cy: 'readonly',
                Cypress: 'readonly',
                describe: 'readonly',
                it: 'readonly',
                before: 'readonly',
                beforeEach: 'readonly',
                after: 'readonly',
                afterEach: 'readonly',
                expect: 'readonly',
            },
        },
        rules: {
            'cypress/no-assigning-return-values': 'warn',
            'cypress/no-unnecessary-waiting': 'warn',
            'cypress/assertion-before-screenshot': 'warn',
            'cypress/no-force': 'warn',
            'cypress/no-async-tests': 'warn',
            'cypress/no-pause': 'warn',
            'cypress/unsafe-to-chain-command': 'warn',
        },
    },
];
