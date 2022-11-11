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
    public hasSubmissionRequired(submissionRequired: string): this {
        cy.task("log", `Has Submission Required ${submissionRequired}`);

        cy.getByTestId("submission-required-text").should("contain.text", submissionRequired);

        return this;
    } 

    public hasSubmissionLink(submissionLink: string): this {
        cy.task("log", `Has Submission link ${submissionLink}`);

        cy.getByTestId("submission-link-text").should("contain.text", submissionLink);

        return this;
    } 
      public hasDateESFAReceivedRequest(dateESFAReceivedRequest: string): this {
        cy.task("log", `Has Date ESFA Received Request ${dateESFAReceivedRequest}`);

        cy.getByTestId("date-esfa-received-text").should("contain.text", dateESFAReceivedRequest);

        return this;
    } 
      public hasTotalAmountRequested(totalAmountRequested: string): this {
        cy.task("log", `Has total Amount Requested ${totalAmountRequested}`);

        cy.getByTestId("amount-requested-text").should("contain.text", totalAmountRequested);

        return this;
    } 
    public hasTypeOfDecisiond(typeOfDecisiond: string): this {
        cy.task("log", `Has type of decision  ${typeOfDecisiond}`);

        cy.getByTestId("decision-type-text").should("contain.text", typeOfDecisiond);

        return this;
    } 
    public hasSupportingNotes(supportingNotes: string): this {
        cy.task("log", `Has Supporting Notes ${supportingNotes}`);

        cy.getByTestId("supporting-notes-text").should("contain.text", supportingNotes);

        return this;
    } 
    public hasActionEdit(actionEdit: string): this {
        cy.task("log", `Has Action Edit link ${actionEdit}`);

        cy.getByTestId("edit-decision-text").should("contain.text", actionEdit);

        return this;
    } 
}