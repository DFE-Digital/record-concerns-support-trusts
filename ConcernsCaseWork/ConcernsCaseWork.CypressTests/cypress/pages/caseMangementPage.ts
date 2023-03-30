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

    public addAnotherConcern(): this
    {
        Logger.Log("Adding another concern");
        cy.getByTestId("add-additional-concern").click();

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

    public hasCaseOwner(value: string): this {
        Logger.Log(`Has case owner ${value}`);

        // Can be improved later
        // Currently its driven by the casing of the email when the user logs in
        // We can't control this, so safer to ignore case for now
        cy.getByTestId("case owner_field").contains(value, { matchCase: false });

        return this;
    }

    public editCaseOwner(): this
    {
        Logger.Log("Editing case owner");

        this.getEditCaseOwner().click();

        return this;
    }

    public canEditCaseOwner(): this
    {
        Logger.Log("Can edit case owner");

        this.getEditCaseOwner();

        return this;
    }

    public cannotEditCaseOwner(): this
    {
        Logger.Log("Cannot edit case owner");
        this.getEditCaseOwner().should("not.exist");

        return this;
    }

    public hasCaseOwnerReassignedBanner(): this
    {
        Logger.Log("Has case reassigned banner");

        this.getCaseOwnerReassignedBanner().should("contain.text", "Case has been reassigned");

        return this;
    }

    public hasNoCaseOwnerReassignedBanner(): this
    {
        Logger.Log("Has no case reassigned banner");

        this.getCaseOwnerReassignedBanner().should("not.exist");

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

    private getEditCaseOwner()
    {
        return cy.getByTestId("edit-case-owner");
    }

    private getCaseOwnerReassignedBanner()
    {
        return cy.getByTestId("case-reassigned-success");
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

    public getTrust(): Cypress.Chainable<string> {
        return cy.getByTestId(`trust_Field`).invoke('text');
    }

    public viewTrustOverview(): this {
        Logger.Log("Viewing trust overview");

        this.getTrustOverviewTab().click();

        return this;
    }

    // has methods
    public hasTrust(value: string): this
    {
        Logger.Log(`Has trust ${value}`);

        cy.getByTestId(`trust_Field`).should("contain.text", value);

        return this;
    }

    public hasRiskToTrust(value: string): this
    {
        Logger.Log(`Has risk to trust ${value}`);

        cy.getByTestId(`risk_to_trust_Field`).should("contain.text", value);

        return this;
    }

    public hasDirectionOfTravel(value: string): this
    {
        Logger.Log(`Has direction of travel ${value}`);

        cy.getByTestId(`direction-of-travel`).should("contain.text", value);

        return this;
    }

    public hasConcerns(concern: string, ratingTags: Array<string>): this
    {
        Logger.Log(`Has concerns ${concern}`);

        cy.getByTestId(`concerns_Field`).should("contain.text", concern);

        let concernsRow = cy.getByTestId("concerns_Field").contains(concern);

        let parentRow = concernsRow.parent();

        parentRow.find("td").eq(1).then((element) =>
        {
            ratingTags.forEach(rating =>
            {
                expect(element.text()).to.contain(rating);
            });
        });

        return this;
    }

    public hasTerritory(value: string): this
    {
        Logger.Log(`Has territory ${value}`);

        cy.getByTestId(`territory_Field`).should("contain.text", value);

        return this;
    }

    public hasIssue(value: string): this
    {
        Logger.Log(`Has issue ${value}`);

        cy.getByTestId(`issue`).should("contain.text", value);

        return this;
    }

    public hasCurrentStatus(value: string): this
    {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`status`).should("contain.text", value);

        return this;
    }

    public hasCaseAim(value: string): this
    {
        Logger.Log(`Has caseAim ${value}`);

        cy.getByTestId(`case-aim`).should("contain.text", value);

        return this;
    }

    public hasDeEscalationPoint(value: string): this
    {
        Logger.Log(`Has deEscalationPoint ${value}`);

        cy.getByTestId(`de-escalation-point`).should("contain.text", value);

        return this;
    }

    public hasNextSteps(value: string): this
    {
        Logger.Log(`Has nextSteps ${value}`);

        cy.getByTestId(`next-steps`).should("contain.text", value);

        return this;
    }

    public hasEmptyCurrentStatus(): this
    {
        this.hasCurrentStatus("");

        return this;
    }

    public hasEmptyCaseAim(): this
    {

        this.hasCaseAim("");

        return this;
    }

    public hasEmptyDeEscalationPoint(): this
    {
        this.hasDeEscalationPoint("");

        return this;
    }

    public hasEmptyNextSteps(): this
    {

        this.hasNextSteps("");

        return this;
    }

    public hasEmptyCaseHistory(): this
    {
        this.hasCaseHistory("");

        return this;
    }

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

}

export default new CaseManagementPage();