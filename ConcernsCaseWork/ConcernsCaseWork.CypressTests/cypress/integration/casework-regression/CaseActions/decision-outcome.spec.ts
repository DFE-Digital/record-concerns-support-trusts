import { Logger } from "../../../common/logger";
import { EditDecisionPage } from "../../../pages/caseActions/decision/editDecisionPage";
import { ViewDecisionPage } from "../../../pages/caseActions/decision/viewDecisionPage";
import { DecisionOutcomePage } from "../../../pages/caseActions/decision/decisionOutcomePage";


describe("Testing decision outcome", () =>{
    const viewDecisionPage = new ViewDecisionPage();
    const editDecisionPage = new EditDecisionPage();
	const decisionOutcomePage = new DecisionOutcomePage();

	
    beforeEach(() => {
		cy.login();
	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});

    it("Create a decision outcome, checking validation and view it was created correctly", () => {
        Logger.Log("Creating Empty Decision");
		cy.createCaseAndAddDecision();

		editDecisionPage
            .saveDecision();

		cy.get("#open-case-actions td")
			.should("contain.text", "Decision: No Decision Types")
			.eq(-3)
			.click();

        Logger.Log("Creating a decision outcome");
        viewDecisionPage
            .hasNoDecisionOutcome()
            .createDecisionOutcome();

        Logger.Log("Checking when no status is selected");
        decisionOutcomePage
            .saveDecisionOutcome()
            .hasValidationError("Select a decision outcome");

        Logger.Log("Checking an invalid date");
        decisionOutcomePage
            .withDecisionOutcomeStatus("Withdrawn")
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("13")
            .withDateDecisionMadeYear("2022")
            .withDecisionTakeEffectDay("12")
            .withDecisionTakeEffectMonth("22")
            .withDecisionTakeEffectYear("2023")
            .saveDecisionOutcome()
            .hasValidationError("Date decision made: 24-13-2022 is an invalid date")
            .hasValidationError("Date decision effective: 12-22-2023 is an invalid date")

        Logger.Log("Checking an incomplete dates");
        decisionOutcomePage
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("12")
            .withDateDecisionMadeYear("")
            .withDecisionTakeEffectDay("12")
            .withDecisionTakeEffectMonth("06")
            .withDecisionTakeEffectYear("")
            .saveDecisionOutcome()
            .hasValidationError("Date decision made: Please enter a complete date DD MM YYYY")
            .hasValidationError("Date decision effective: Please enter a complete date DD MM YYYY");

        Logger.Log("Create Decision Outcome");
        decisionOutcomePage
            .withDecisionOutcomeStatus("ApprovedWithConditions")
            .withTotalAmountApproved("50,000")
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("11")
            .withDateDecisionMadeYear("2022")
            .withDecisionTakeEffectDay("11")
            .withDecisionTakeEffectMonth("12")
            .withDecisionTakeEffectYear("2023")
            .withDecisionAuthouriser("DeputyDirector")
            .withBusinessArea("BusinessPartner")
            .withBusinessArea("Capital")
            .withBusinessArea("ProviderMarketOversight")
            .saveDecisionOutcome();

        Logger.Log("Select created decision")
        cy.get("#open-case-actions td")
			.should("contain.text", "Decision: No Decision Types")
			.eq(-3)
			.click();

        Logger.Log("View decision outcome")

        viewDecisionPage
            .hasBusinessArea("Business Partner")
            .hasBusinessArea("Capital")
            .hasBusinessArea("Provider Market Oversight")
            .hasDecisionOutcomeStatus("Approved with conditions")
            .hasMadeDate("24-11-2022")
            .hasEffectiveFromDate("11-12-2023")
            .hasTotalAmountApproved("£50,000")
            .hasAuthoriser("Deputy Director")
            .cannotCreateAnotherDecisionOutcome();
    });

    it("Create an decision outcome with only status, should set status but all other labels should be empty", () =>
    {
        Logger.Log("Creating Empty Decision");
		cy.addConcernsDecisionsAddToCase();

		editDecisionPage
            .saveDecision();

        cy.get("#open-case-actions td")
			.should("contain.text", "Decision: No Decision Types")
			.eq(-3)
			.click();

        viewDecisionPage
            .createDecisionOutcome();

        Logger.Log("Create Decision Outcome with only status");
        decisionOutcomePage
            .withDecisionOutcomeStatus("Withdrawn")
            .saveDecisionOutcome();

        cy.get("#open-case-actions td")
            .should("contain.text", "Decision: No Decision Types")
            .eq(-3)
            .click();

        viewDecisionPage
            .hasBusinessArea("Empty")
            .hasDecisionOutcomeStatus("Withdrawn")
            .hasMadeDate("Empty")
            .hasEffectiveFromDate("Empty")
            .hasTotalAmountApproved("£0.00")
            .hasAuthoriser("Empty");
    });
})
