import { Logger } from "../common/logger";

class CaseManagementPage {

    //locators
    getHeadingText() {
        return cy.get('h1[class="govuk-heading-l"]');
    }

    getHeadingInnerText() {
        return cy.get('span.govuk-caption-m');
    }

    getSubHeadingText() {
        return cy.get('[class="govuk-caption-m"]');
    }

    getTrustHeadingText() {
        return cy.get('h2[class="govuk-heading-m"]');
    }

    getConcernEditBtn() {
        return cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]');
    }

    getConcernTable() {
        return cy.get('[class^="govuk-table-case-details"]');
    }

    getConcernTableAddConcernBtn() {
        return cy.contains("Add concern");
    }

    getAddToCaseBtn() {
        return cy.get('[role="button"]').contains('Add to case');
    }

    getCaseDetailsTab() {
        return cy.get('[id="tab_case-details"]');
    }

    getTrustOverviewTab() {
        return cy.get('[id="tab_trust-overview"]');
    }

    getOpenActionsTable() {
        return cy.get('[id="open-case-actions"]');
    }

    getClosedActionsTable() {
        return cy.get('[id="close-case-actions"]');
    }

    getCloseCaseBtn() {
        return cy.get('#close-case-button');
    }

    getLiveSRMALink() {
        return cy.get('a[id="open-case-actions"][href*="/action/srma/"]');
    }

    getOpenActionLink(action) {
        return cy.get('[id="open-case-actions"] [href*="/action/' + action + '/"]');
    }

    getClosedActionLink(action) {
        return cy.get('[id="closed-case-actions"] [href*="/action/' + action + '/"]');
    }

    geBackToCaseworkBtn() {
        return cy.get('[class="buttons-topOfPage"]');
    }

    getCaseID() {
        return cy.get('[name=caseID]');
    }

    getBackBtn() {
        return cy.get('[id="back-link-event"]');
    }

    //methods

    closeAllOpenConcerns() {
        const elem = '.govuk-table-case-details__cell_no_border [href*="edit_rating"]';
        if (Cypress.$(elem).length > 0) { //Cypress.$ needed to handle element missing exception

            this.getConcernEditBtn().its('length').then(($elLen) => {

                while ($elLen > 0) {

                    const $elem = Cypress.$('.govuk-table-case-details__cell_no_border [href*="edit_rating"]');
                    cy.log("About to close all lopen concerns");
                    cy.log("$elem.length =" + ($elem).length)

                    if (($elem).length > 0) { //Cypress.$ needed to handle element missing exception

                        this.getConcernEditBtn().its('length').then(($elLen) => {
                            cy.log("Method $elLen " + $elLen)
                            while ($elLen > 0) {

                                this.getConcernEditBtn().eq($elLen - 1).click();
                                cy.get('[href*="closure"]').click();
                                cy.get('.govuk-button-group [href*="edit_rating/closure"]:nth-of-type(1)').click();
                                $elLen = $elLen - 1
                                cy.log($elLen + " more open concerns")
                            }
                        });
                    } else {
                        cy.log('All concerns closed')
                    }
                }
            });
        }
    }

    checkForOpenActions() {
        const $elem = Cypress.$('[id="open-case-actions"]');
        return ($elem.length);
    }

    getCaseIDText() {

        return this.getHeadingText().invoke('text').then((text) => {
            var splitText = text.split('\n')[2]
            console.log("splitText " + splitText)
            return splitText.trim();
        });
    }

    public createCase(): this
    {
        Logger.Log("Creating case");

        cy.getByTestId("create-case-button").click();

        return this;
    }

    public hasCaseHistory(value: string): this {
        Logger.Log(`Has case history ${value}`);
        cy.getByTestId("case-history").should("contain.text", value);

        return this;
    }

    public hasClosedCaseValidationError(value: string): this {
        cy.getByTestId("case-closure-validation-errors").should("contain.text", value);

        return this;
    }

    public showAllConcernDetails(): this {
        Logger.Log("Showing all concerns details");

        const id = ".govuk-accordion__show-all";

        cy.get(id).invoke("text")
            .then((text) => {
                if (text === "Show all sections")
                    cy.get(id).click();
            });

        return this;
    }

    public editConcern(): this {
        Logger.Log("Editing the concern");
        this.getEditConcern().click();

        return this;
    }

    public canEditConcern(): this
    {
        Logger.Log("Can edit the concern");
        this.getEditConcern();

        return this;
    }
    public cannotEditConcern(): this
    {
        Logger.Log("Cannot edit the concern");
        this.getEditConcern().should("not.exist");

        return this;
    }

    public editRiskToTrust(): this {
        Logger.Log("Editing the risk to trust");
        this.getEditRiskToTrust().click();

        return this;
    }

    public canEditRiskToTrust(): this {
        Logger.Log("Can edit the risk to trust");
        this.getEditRiskToTrust();

        return this;
    }

    public cannotEditRiskToTrust(): this {
        Logger.Log("Cannot edit the risk to trust");
        this.getEditRiskToTrust().should("not.exist");

        return this;
    }

    public editDirectionOfTravel(): this {
        Logger.Log("Editing the direction of travel");
        this.getEditDirectionOfTravel().click();

        return this;
    }

    public canEditDirectionOfTravel(): this {
        Logger.Log("Can edit the direction of travel");
        this.getEditDirectionOfTravel();

        return this;
    }

    public cannotEditDirectionOfTravel(): this {
        Logger.Log("Cannot edit the direction of travel");
        this.getEditDirectionOfTravel().should("not.exist");

        return this;
    }

    public editTerritory(): this
    {
        Logger.Log("Editing the territory");
        this.getEditTerritory().click();

        return this;
    }

    public canEditTerritory(): this
    {
        Logger.Log("Can edit the territory");
        this.getEditTerritory();

        return this;
    }

    public cannotEditTerritory(): this
    {
        Logger.Log("Cannot edit the territory");
        this.getEditTerritory().should("not.exist");

        return this;
    }

    public editIssue(): this
    {
        Logger.Log("Editing the issue");
        this.getEditIssue().click();

        return this;
    }

    public canEditIssue() {
        Logger.Log("Can edit the issue");
        this.getEditIssue();

        return this;
    }

    public cannotEditIssue() {
        Logger.Log("Cannot edit the issue");
        this.getEditIssue().should("not.exist");

        return this;
    }

    public editCurrentStatus(): this
    {
        Logger.Log("Editing the current status");
        this.getEditCurrentStatus().click();

        return this;
    }

    public canEditCurrentStatus() {
        Logger.Log("Can edit the current status");
        this.getEditCurrentStatus();

        return this;
    }

    public cannotEditCurrentStatus() {
        Logger.Log("Cannot edit the current status");
        this.getEditCurrentStatus().should("not.exist");

        return this;
    }

    public editCaseAim(): this
    {
        Logger.Log("Editing the case aim");
        this.getEditCaseAim().click();

        return this;
    }

    public canEditCaseAim() {
        Logger.Log("Can edit the case aim");
        this.getEditCaseAim();

        return this;
    }

    public cannotEditCaseAim() {
        Logger.Log("Cannot edit the case aim");
        this.getEditCaseAim().should("not.exist");

        return this;
    }

    public editDeEscalationPoint(): this
    {
        Logger.Log("Editing the de-escalation point");
        this.getEditDeEscalationPoint().click();

        return this;
    }

    public canEditDeEscalationPoint() {
        Logger.Log("Can edit the de-escalation point");
        this.getEditDeEscalationPoint();

        return this;
    }

    public cannotEditDeEscalactionPoint() {
        Logger.Log("Cannot edit the de-escalation point");
        this.getEditDeEscalationPoint().should("not.exist");

        return this;
    }

    public editNextSteps(): this
    {
        Logger.Log("Editing the next steps");
        this.getEditNextSteps().click();

        return this;
    }

    public canEditNextSteps() {
        Logger.Log("Can edit the next steps");
        this.getEditNextSteps();

        return this;
    }

    public cannotEditNextSteps() {
        Logger.Log("Cannot edit the next steps");
        this.getEditNextSteps().should("not.exist");

        return this;
    }

    public editCaseHistory(): this
    {
        Logger.Log("Editing the case history");
        this.getEditCaseHistory().click();

        return this;
    }

    public canEditCaseHistory() {
        Logger.Log("Can edit the case history");
        this.getEditCaseHistory();

        return this;
    }

    public cannotEditCaseHistory() {
        Logger.Log("Cannot edit the case history");
        this.getEditCaseHistory().should("not.exist");

        return this;
    }

    public withRationaleForClosure(reason: string): this {
        Logger.Log(`With rationale for closure ${reason}`)
        cy.getById("case-outcomes").clear().type(reason);

        return this;
    }

    public withRationaleForClosureExceedingLimit(): this {
        Logger.Log(`With rationale for closure that exceeds the limit`)
        cy.getById("case-outcomes").clear().invoke("val", "x1".repeat(200));

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);
        cy.getById("errorSummary").should("contain.text", value);

        return this;
    }

    public addCaseAction(action: string): this
    {
        Logger.Log(`Adding case action ${action}`);
        this.getAddCaseAction().click();
        cy.get(`[value=${action}]`).check();
        cy.getByTestId("add-action-to-case").click();

        return this;
    }

    public canAddCaseAction()
    {
        Logger.Log("Can add case action")
        this.getAddCaseAction();

        return this;
    }

    public cannotAddCaseAction()
    {
        Logger.Log("Cannot add case action")
        this.getAddCaseAction().should("not.exist");

        return this;
    }

    private getEditConcern()
    {
        return cy.getByTestId("edit-concern");
    }

    private getEditRiskToTrust()
    {
        return cy.getByTestId("edit-risk-to-trust");
    }

    private getEditDirectionOfTravel()
    {
        return cy.getByTestId("edit-direction-of-travel")
    }

    private getEditTerritory()
    {
        return cy.getByTestId("edit_Button_SFSO");
    }

    private getEditIssue()
    {
        return cy.getByTestId("edit-issue");
    }

    private getEditCurrentStatus()
    {
        return cy.getByTestId("edit-current-status");
    }

    private getEditCaseAim()
    {
        return cy.getByTestId("edit-case-aim");
    }

    private getEditDeEscalationPoint()
    {
        return cy.getByTestId("edit-de-escalation-point");
    }

    private getEditNextSteps()
    {
        return cy.getByTestId("edit-next-steps");
    }

    private getEditCaseHistory()
    {
        return cy.getByTestId("edit-case-history");
    }

    private getAddCaseAction()
    {
        return cy.getByTestId("add-case-action");
    }
}

export default new CaseManagementPage();