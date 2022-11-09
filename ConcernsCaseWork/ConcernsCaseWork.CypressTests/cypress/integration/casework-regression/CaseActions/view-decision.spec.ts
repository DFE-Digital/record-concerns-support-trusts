import { ViewDecisionPage } from "../../../pages/caseActions/viewDecisionPage";

describe("Viewing an existing decision", () => {

    const decisionPage: ViewDecisionPage = new ViewDecisionPage();

    before(() => {
        cy.login();
    });

    it("Should display the decision information", () => {
        cy.visit("/");

        decisionPage
            .hasCrmEnquiry("123456")
            .hasRetrospectiveRequest("No");
    });
});