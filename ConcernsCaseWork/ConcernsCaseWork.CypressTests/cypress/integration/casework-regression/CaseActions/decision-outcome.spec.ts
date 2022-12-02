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

    it("Create a decision outcome and view it was created correctly", () => {
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

        Logger.Log("Checking when no decision is selected");
        decisionOutcomePage
            .withTotalAmountApproved("50,000")
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("12")
            .withDateDecisionMadeYear("2022")
        .saveDecisionOutcome()
            .hasValidationError("Select a decision outcome");

        Logger.Log("Checking an invalid date");
        decisionOutcomePage
            .withDecisionOutcomeStatus("ApprovedWithConditions")
            .withTotalAmountApproved("50,000")
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("13")
            .withDateDecisionMadeYear("2022")
            .saveDecisionOutcome()
            .hasValidationError("24-13-2022 is an invalid date");

        Logger.Log("Checking an incomplete date");
        decisionOutcomePage
            .withTotalAmountApproved("50,000")
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("12")
            .withDateDecisionMadeYear("")
            .saveDecisionOutcome()
            .hasValidationError("Please enter a complete date DD MM YYYY");

        Logger.Log("Create Decision Outcome");
        decisionOutcomePage
            .withDecisionOutcomeStatus("ApprovedWithConditions")
            .withTotalAmountApproved("50,000")
            .withDateDecisionMadeDay("24")
            .withDateDecisionMadeMonth("11")
            .withDateDecisionMadeYear("2022")
            .withDecisionTakeEffectDay("24")
            .withDecisionTakeEffectMonth("11")
            .withDecisionTakeEffectYear("2022")
            .withDecisionAuthouriser("DeputyDirector")
            .withBusinessArea("BusinessPartner")
            .withBusinessArea("Capital")
            .withBusinessArea("ProviderMarketOversight")
            .saveDecisionOutcome();

        cy.get("#open-case-actions td")
			.should("contain.text", "Decision: No Decision Types")
			.eq(-3)
			.click();

        viewDecisionPage
            .hasBusinessArea("Funding")
            .hasDecisionOutcomeStatus("Retrospective Approval")
            .hasMadeDate("2022-01-01")
            .hasEffectiveFromDate("2022-02-02")
            .hasTotalAmountApproved("£100")
            .hasAuthoriser("G6")
            .editDecisionOutcome()
    });
})
