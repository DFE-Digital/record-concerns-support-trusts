import { ViewDecisionPage } from "../../../pages/caseActions/viewDecisionPage";

describe("Viewing an existing decision", () => {

    const decisionPage: ViewDecisionPage = new ViewDecisionPage();

    it("Should display the decision information", () => {
        cy.visit("/case/306/management/action/decision/6");

        decisionPage
            .hasCrmEnquiry("123456")
            .hasRetrospectiveRequest("No");
    });
});