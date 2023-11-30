export class Logger
{
    public static log(message: string)
    {
        cy.task("log", message);
    }
}