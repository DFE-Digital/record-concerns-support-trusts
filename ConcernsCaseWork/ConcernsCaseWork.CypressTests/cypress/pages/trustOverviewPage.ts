import { Logger } from "cypress/common/logger";

export class TrustOverviewPage {
    public trustTypeIsNotEmpty(): this
    {
        Logger.log(`Trust type is not empty`);

        cy.getByTestId(`trust-type`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustAddressIsNotEmpty(): this
    {
        Logger.log(`Trust address is not empty`);

        cy.getByTestId(`trust-address`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustAcademiesIsNotEmpty(): this
    {
        Logger.log(`Trust academies is not empty`);

        cy.getByTestId(`trust-academies`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustPupilCapacityIsNotEmpty(): this
    {
        Logger.log(`Trust pupil capacity is not empty`);

        cy.getByTestId(`trust-pupil-capacity`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustPupilNumbersIsNotEmpty(): this
    {
        Logger.log(`Trust pupil numbers is not empty`);

        cy.getByTestId(`trust-number-of-pupils`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustGroupIdIsNotEmpty(): this
    {
        Logger.log(`Trust group id is not empty`);

        cy.getByTestId(`trust-group-id`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustUKPRNIsNotEmpty(): this
    {
        Logger.log(`Trust UKPRN is not empty`);

        cy.getByTestId(`trust-UKPRN`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustPhoneNumberIsNotEmpty(): this
    {
        Logger.log(`Trust phone number is not empty`);

        cy.getByTestId(`trust-phone-number`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public trustCompanyHouseNumberIsNotEmpty(): this
    {
        Logger.log(`Trust company house number is not empty`);

        cy.getByTestId(`trust-company-house-number`).invoke("text").should("have.length.above", 1);

        return this;
    }

    public createCase(): this
    {
        Logger.log("Creating a case against the trust");

        cy.getByTestId("create-case-button").click();

        return this;
    }

    public showClosedCases(): this
    {
        Logger.log("Showing closed cases");

        cy.getByTestId("closed-cases-tab").click();

        return this;
    }
}

const trustOverviewPage = new TrustOverviewPage();

export default trustOverviewPage;