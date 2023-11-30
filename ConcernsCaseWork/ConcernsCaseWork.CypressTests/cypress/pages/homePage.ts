import { Logger } from "cypress/common/logger";

class HomePage {

    getClosedCasesBtn() {
        return cy.get('[href="/case/closed"]');
    }

    public viewOtherCases(): this
    {
        cy.getByTestId("team-casework-tab").click();

        return this;
    }

    public selectColleagues(): this
    {
        Logger.log("Selecting colleagues");
        cy.getByTestId("select-colleagues").click();

        return this;
    }
}

export default new HomePage();