import { Logger } from "cypress/common/logger";

export class TrustOverviewPage {
    public trustTypeIsNotEmpty(): this
    {
        Logger.Log(`Trust type is not empty`);

        cy.getByTestId(`trust-type`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustAddressIsNotEmpty(): this
    {
        Logger.Log(`Trust address is not empty`);

        cy.getByTestId(`trust-address`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustAcademiesIsNotEmpty(): this
    {
        Logger.Log(`Trust academies is not empty`);

        cy.getByTestId(`trust-academies`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustPupilCapacityIsNotEmpty(): this
    {
        Logger.Log(`Trust pupil capacity is not empty`);

        cy.getByTestId(`trust-pupil-capacity`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustPupilNumbersIsNotEmpty(): this
    {
        Logger.Log(`Trust pupil numbers is not empty`);

        cy.getByTestId(`trust-number-of-pupils`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustGroupIdIsNotEmpty(): this
    {
        Logger.Log(`Trust group id is not empty`);

        cy.getByTestId(`trust-group-id`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustUKPRNIsNotEmpty(): this
    {
        Logger.Log(`Trust UKPRN is not empty`);

        cy.getByTestId(`trust-UKPRN`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustPhoneNumberIsNotEmpty(): this
    {
        Logger.Log(`Trust phone number is not empty`);

        cy.getByTestId(`trust-phone-number`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustCompanyHouseNumberIsNotEmpty(): this
    {
        Logger.Log(`Trust company house number is not empty`);

        cy.getByTestId(`trust-company-house-number`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public createCase(): this
    {
        Logger.Log("Creating a case against the trust");

        cy.getByTestId("create-case-button").click();

        return this;
    }

    public showClosedCases(): this
    {
        Logger.Log("Showing closed cases");

        cy.getById("closed-cases-tab").click();

        return this;
    }
}

const trustOverviewPage = new TrustOverviewPage();

export default trustOverviewPage;