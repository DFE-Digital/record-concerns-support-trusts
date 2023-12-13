import { Logger } from "../common/logger";

class CaseManagementPage {

	getAddToCaseBtn() {
		return cy.get('[role="button"]').contains("Add to case");
	}

	getCloseCaseBtn() {
		return cy.get("#close-case-button");
	}

	getCaseIDText() {
		return cy.getByTestId("heading-case-id")
		.invoke("text")
		.then(text =>
		{
			const caseId = text.split(" ").pop();

			return caseId;
		});
	}

	public createCase(): this {
		Logger.log("Creating case");

		cy.getByTestId("create-case-button").click();

		return this;
	}

	public hasCaseHistory(value: string): this {
		Logger.log(`Has case history ${value}`);
		this.getCaseHistory().should("contain.text", value);

		return this;
	}

	public hasClosedCaseValidationError(value: string): this {
		cy.getById("errorSummary").should("contain.text", value);

		return this;
	}

	public showAllConcernDetails(): this {
		Logger.log("Showing all concerns details");

		const id = ".govuk-accordion__show-all";

		cy.get(id)
			.invoke("text")
			.then((text) => {
				if (text === "Show all sections") cy.get(id).click();
			});

		return this;
	}

	public canEditCase(): this {
		this.getEditConcern();
		this.getCloseConcern();
		this.getEditRiskToTrust();
		this.getEditDirectionOfTravel();
		this.getManagedBy();
		this.getEditCaseOwner();
		this.getEditIssue();
		this.getEditCurrentStatus();
		this.getEditCaseAim();
		this.getEditDeEscalationPoint();
		this.getEditNextSteps();
		this.getEditCaseHistory();
		this.getAddCaseAction();

		return this;
	}

	public cannotEditCase(): this {

		this.getEditConcern().should("not.exist");
		this.getCloseConcern().should("not.exist");
		this.getEditRiskToTrust().should("not.exist");
		this.getEditDirectionOfTravel().should("not.exist");
		this.getManagedBy().should("not.exist");
		this.getEditCaseOwner().should("not.exist");
		this.getEditIssue().should("not.exist");
		this.getEditCurrentStatus().should("not.exist");
		this.getEditCaseAim().should("not.exist");
		this.getEditDeEscalationPoint().should("not.exist");
		this.getEditNextSteps().should("not.exist");
		this.getEditCaseHistory().should("not.exist");
		this.getAddCaseAction().should("not.exist");

		return this;
	}

	public editConcern(): this {
		Logger.log("Editing the concern");
		this.getEditConcern().click();

		return this;
	}

	public closeConcern(): this {
		this.getCloseConcern().first().click();

		return this;
	}

	public addAnotherConcern(): this {
		Logger.log("Adding another concern");
		cy.getByTestId("add-additional-concern").click();

		return this;
	}
	public addAnotherConcernForNonConcern(): this {
		Logger.log("Adding another concern for non concern journey");
		cy.getByTestId("Add_Button_Concern").click();

		return this;
	}
	public editRiskToTrust(): this {
		Logger.log("Editing the risk to trust");
		this.getEditRiskToTrust().click();

		return this;
	}

	public editDirectionOfTravel(): this {
		Logger.log("Editing the direction of travel");
		this.getEditDirectionOfTravel().click();

		return this;
	}

	public editManagedBy(): this {
		Logger.log("Editing managed by");
		this.getManagedBy().click();

		return this;
	}

	public hasCaseOwner(value: string): this {
		Logger.log(`Has case owner ${value}`);

		// Can be improved later
		// Currently its driven by the casing of the email when the user logs in
		// We can't control this, so safer to ignore case for now
		cy.getByTestId("case owner_field").contains(value, { matchCase: false });

		return this;
	}

	public editCaseOwner(): this {
		Logger.log("Editing case owner");

		this.getEditCaseOwner().click();

		return this;
	}

	public hasCaseOwnerReassignedBanner(): this {
		Logger.log("Has case reassigned banner");

		this.getCaseOwnerReassignedBanner().should(
			"contain.text",
			"Case has been reassigned"
		);

		return this;
	}

	public hasNoCaseOwnerReassignedBanner(): this {
		Logger.log("Has no case reassigned banner");

		this.getCaseOwnerReassignedBanner().should("not.exist");

		return this;
	}

	public editIssue(): this {
		Logger.log("Editing the issue");
		this.getEditIssue().click();

		return this;
	}

	public editCurrentStatus(): this {
		Logger.log("Editing the current status");
		this.getEditCurrentStatus().click();

		return this;
	}

	public editCaseAim(): this {
		Logger.log("Editing the case aim");
		this.getEditCaseAim().click();

		return this;
	}

	public editDeEscalationPoint(): this {
		Logger.log("Editing the de-escalation point");
		this.getEditDeEscalationPoint().click();

		return this;
	}

	public editNextSteps(): this {
		Logger.log("Editing the next steps");
		this.getEditNextSteps().click();

		return this;
	}

	public editCaseHistory(): this {
		Logger.log("Editing the case history");
		this.getEditCaseHistory().click();

		return this;
	}

	public withRationaleForClosure(reason: string): this {
		Logger.log(`With rationale for closure ${reason}`);
		cy.getById("case-outcomes").clear().type(reason);

		return this;
	}

	public withRationaleForClosureExceedingLimit(): this {
		Logger.log(`With rationale for closure that exceeds the limit`);
		cy.getById("case-outcomes").clear().invoke("val", "x1".repeat(200));

		return this;
	}

	public hasValidationError(value: string): this {
		Logger.log(`Has validation error ${value}`);
		cy.getById("errorSummary").should("contain.text", value);

		return this;
	}

	public addCaseAction(action: string): this {
		Logger.log(`Adding case action ${action}`);
		this.getAddCaseAction().click();
		cy.getByTestId(action).check();
		cy.getByTestId("add-action-to-case").click();

		return this;
	}

	private getEditConcern() {
		return cy.getByTestId("edit-concern");
	}

	private getCloseConcern() {
		return cy.getByTestId("close-concern");
	}

	private getEditRiskToTrust() {
		return cy.getByTestId("edit-risk-to-trust");
	}

	private getEditDirectionOfTravel() {
		return cy.getByTestId("edit-direction-of-travel");
	}

	private getManagedBy() {
		return cy.getByTestId("edit_Button_SFSO");
	}

	private getEditCaseOwner() {
		return cy.getByTestId("edit-case-owner");
	}

	private getCaseOwnerReassignedBanner() {
		return cy.getByTestId("case-reassigned-success");
	}

	private getEditIssue() {
		return cy.getByTestId("edit-issue");
	}

	private getEditCurrentStatus() {
		return cy.getByTestId("edit-current-status");
	}

	private getEditCaseAim() {
		return cy.getByTestId("edit-case-aim");
	}

	private getEditDeEscalationPoint() {
		return cy.getByTestId("edit-de-escalation-point");
	}

	private getEditNextSteps() {
		return cy.getByTestId("edit-next-steps");
	}

	private getEditCaseHistory() {
		return cy.getByTestId("edit-case-history");
	}

	private getAddCaseAction() {
		return cy.getByTestId("add-case-action");
	}

	public getTrust(): Cypress.Chainable<string> {
		return cy.getByTestId(`trust_Field`).invoke("text");
	}

	public viewTrustOverview(): this {
		Logger.log("Viewing trust overview");

		cy.getByTestId("trust-overview-tab").click();

		return this;
	}

	public viewCase(): this {
		Logger.log("Viewing case");

		cy.getByTestId("case-overview-tab").click();

		return this;
	}

	public hasTrust(value: string): this {
		Logger.log(`Has trust ${value}`);

		cy.getByTestId(`trust_Field`).should("contain.text", value);

		return this;
	}

	public hasTrustContain(value: string): this {
		Logger.log(`Has trust ${value}`);

		cy.getByTestId("trust_Field").contains(value, { matchCase: false });

		return this;
	}

	public hasRiskToTrust(value: string): this {
		Logger.log(`Has risk to trust ${value}`);

		cy.getByTestId(`risk_to_trust_Field`).should("contain.text", value);

		return this;
	}

	public hasDirectionOfTravel(value: string): this {
		Logger.log(`Has direction of travel ${value}`);

		cy.getByTestId(`direction-of-travel`).should("contain.text", value);

		return this;
	}

	public hasConcerns(concern: string, ratingTags: Array<string>): this {
		Logger.log(`Has concerns ${concern}`);

		cy.getByTestId(`concerns_Field`).should("contain.text", concern);

		let concernsRow = cy.getByTestId("concerns_Field").contains(concern);

		let parentRow = concernsRow.parent();

		parentRow
			.find("td")
			.eq(1)
			.then((element) => {
				ratingTags.forEach((rating) => {
					expect(element.text()).to.contain(rating);
				});
			});

		return this;
	}

	public hasNumberOfConcerns(count: number): this {
		Logger.log(`Has number of concerns ${count}`);
		cy.getByTestId("concerns-details-table").find('tr').should("have.length", count);

		return this;
	}

	public hasManagedBy(division: string, territory: string): this {
		Logger.log(`Has managed by ${division} ${territory}`);

		cy.getByTestId(`territory_Field`).should("contain.text", division);
		cy.getByTestId(`territory_Field`).should("contain.text", territory);

		return this;
	}

	public hasIssue(value: string): this {
		Logger.log(`Has issue ${value}`);

		cy.getByTestId(`issue`).should("contain.text", value);

		return this;
	}

	public hasCurrentStatus(value: string): this {
		Logger.log(`Has status ${value}`);

		this.getCurrentStatus().should("contain.text", value);

		return this;
	}

	public hasCaseAim(value: string): this {
		Logger.log(`Has caseAim ${value}`);

		this.getCaseAim().should("contain.text", value);

		return this;
	}

	public hasDeEscalationPoint(value: string): this {
		Logger.log(`Has deEscalationPoint ${value}`);

		this.getDeEscalationPoint().should("contain.text", value);

		return this;
	}

	public hasNextSteps(value: string): this {
		Logger.log(`Has nextSteps ${value}`);

		this.getNextSteps().should("contain.text", value);

		return this;
	}

	public hasEmptyCurrentStatus(): this {
		this.getCurrentStatus().should("be.empty");

		return this;
	}

	public hasEmptyCaseAim(): this {
		this.getCaseAim().should("be.be.empty");

		return this;
	}

	public hasEmptyDeEscalationPoint(): this {
		this.getDeEscalationPoint().should("be.empty");

		return this;
	}

	public hasEmptyNextSteps(): this {
		this.getNextSteps().should("be.empty");

		return this;
	}

	public hasEmptyCaseHistory(): this {
		this.getCaseHistory().should("be.empty");

		return this;
	}

	public hasNoCaseNarritiveFields(): this {
		Logger.log("Has no case narritive fields");
		cy.getByTestId("case-narritive-fields-container").should("not.exist");

		return this;
	}

	private getCurrentStatus() {
		return cy.getByTestId(`status`);
	}

	private getCaseAim() {
		return cy.getByTestId(`case-aim`);
	}

	private getDeEscalationPoint() {
		return cy.getByTestId(`de-escalation-point`);
	}

	private getNextSteps() {
		return cy.getByTestId(`next-steps`);
	}

	private getCaseHistory() {
		return cy.getByTestId("case-history");
	}
}

export default new CaseManagementPage();
