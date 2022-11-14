export class ViewDecisionPage

{
    public withCrmEnquiry(crmNumber: string): this {
cy.task("log", `With Crm enquiry ${crmNumber}`);

cy.getById("crm-enquiry-number").clear().type(crmNumber);

return this;


    }

    public withRetrospectiveRequest(retrospectiveRequest: string): this {
        cy.task("log", `With retrospective request ${retrospectiveRequest}`);
        

        cy.getByTestId("retrospective-request-text").clear().type(retrospectiveRequest);

        return this;
    }
    public withSubmissionRequired(submissionRequired: string): this {
        cy.task("log", `With Submission Required ${submissionRequired}`);

        cy.getByTestId("submission-required-text").clear().type(submissionRequired);

        return this;
    } 

    public withSubmissionLink(submissionLink: string): this {
        cy.task("log", `Has Submission link ${submissionLink}`);

        cy.getByTestId("submission-link-text").clear().type(submissionLink);

        return this;
    } 
      public withDateESFAReceivedRequest(dateESFAReceivedRequest: string): this {
        cy.task("log", `With Date ESFA Received Request ${dateESFAReceivedRequest}`);

        cy.getByTestId("date-esfa-received-text").clear().type(dateESFAReceivedRequest);

        return this;
    } 
      public withTotalAmountRequested(totalAmountRequested: string): this {
        cy.task("log", `With total Amount Requested ${totalAmountRequested}`);

        cy.getByTestId("amount-requested-text").clear().type(totalAmountRequested);

        return this;
    } 
    public withTypeOfDecision(typeOfDecisiond: string): this {
        cy.task("log", `with type of decision  ${typeOfDecisiond}`);

        cy.getByTestId("decision-type-text").clear().type(typeOfDecisiond);

        return this;
    } 
    public withSupportingNotes(supportingNotes: string): this {
        cy.task("log", `With Supporting Notes ${supportingNotes}`);

        cy.getByTestId("supporting-notes-text").clear().type(supportingNotes);

        return this;
    } 
    public withActionEdit(actionEdit: string): this {
        cy.task("log", `With Action Edit link ${actionEdit}`);

        cy.getByTestId("edit-decision-text").clear().type(actionEdit);

        return this;
    } 










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
    public hasTypeOfDecision(typeOfDecisiond: string): this {
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