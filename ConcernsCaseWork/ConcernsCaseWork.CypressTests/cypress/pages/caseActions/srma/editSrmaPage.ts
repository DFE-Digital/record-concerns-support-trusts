import { Logger } from "../../../common/logger";

export class EditSrmaPage
{
    public withStatus(status: string)
    {
        Logger.Log(`With status {status}`);

        const id = status.split(" ").join("");

        cy.getByTestId(id).click();

        return this;
    }

    public withContactedDay(day: string): this
    {
        Logger.Log(`With contacted day ${day}`);

        cy.getById("dtr-day").clear().type(day);

        return this;
    }

    public withContactedMonth(month: string): this
    {
        Logger.Log(`With contacted month ${month}`);

        cy.getById("dtr-month").clear().type(month);

        return this;
    }

    public withContactedYear(year: string): this
    {
        Logger.Log(`With contacted year ${year}`);

        cy.getById("dtr-year").clear().type(year);

        return this;
    }

    public save(): this
    {
        Logger.Log("Saving SRMA");

        cy.getById("add-srma-button").click();

        return this;
    }
}