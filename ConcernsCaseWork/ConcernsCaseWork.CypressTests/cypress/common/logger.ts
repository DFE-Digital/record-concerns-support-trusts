import { EnvLog } from "cypress/constants/cypressConstants";

export class Logger
{
    public static Log(message: string)
    {
        if (Cypress.env(EnvLog)) {
            cy.task("log", message);
        }
    }
}