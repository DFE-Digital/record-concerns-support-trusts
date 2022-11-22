export class Logger
{
    public static Log(message: string)
    {
        cy.task("log", message);
    }
}