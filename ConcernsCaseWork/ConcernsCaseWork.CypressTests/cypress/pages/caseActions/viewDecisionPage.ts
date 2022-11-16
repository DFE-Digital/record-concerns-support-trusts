export class ViewDecisionPage
{
    public hasCrmEnquiry(crmNumber: string): this {
        cy.task("log", `Has CRM enquiry ${crmNumber}`);

        cy.getByTestId("crm-enquiry-text").should("contain.text", crmNumber);

        return this;
    }

    public hasRetrospectiveRequest(retrospectiveRequest: string): this {
        cy.task("log", `Has retrospective request ${retrospectiveRequest}`);

        cy.getByTestId("retrospective-request-text").should("contain.text", retrospectiveRequest);

        return this;
    }
}