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
        cy.addConcernsDecisionsAddToCase();

		editDecisionPage
            .save();

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

    it("Create a decision outcome with only status, should set status but all other labels should be empty", () =>
    {
        Logger.Log("Creating Empty Decision");
        cy.addConcernsDecisionsAddToCase();

		editDecisionPage
            .save();

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

    it("Edit a decision outcome ", () => {
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

        Logger.Log("Create Decision Outcome");
        decisionOutcomePage
            .withDecisionOutcomeStatus("Withdrawn")
            .withTotalAmountApproved("1,000")
            .withDateDecisionMadeDay("3")
            .withDateDecisionMadeMonth("5")
            .withDateDecisionMadeYear("2022")
            .withDecisionTakeEffectDay("6")
            .withDecisionTakeEffectMonth("8")
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
			.editDecisionOutcome();

        Logger.Log("Verify Existing Values");
        decisionOutcomePage
            .hasDecisionOutcomeStatus("Withdrawn")
            .hasTotalAmountApproved("1,000")
            .hasDecisionMadeDay("3")
            .hasDecisionMadeMonth("5")
            .hasDecisionMadeYear("2022")
            .hasDateEffectiveFromDay("6")
            .hasDateEffectiveFromMonth("8")
            .hasDateEffectiveFromYear("2022")
            .hasDecisionAuthouriser("DeputyDirector")
            .hasBusinessArea("BusinessPartner")
            .hasBusinessArea("Capital")
            .hasBusinessArea("ProviderMarketOversight");


            Logger.Log("Edit Decision Outcome");
            decisionOutcomePage
                .withDecisionOutcomeStatus("Approved")
                .withTotalAmountApproved("1,000,000")
                .withDateDecisionMadeDay("12")
                .withDateDecisionMadeMonth("11")
                .withDateDecisionMadeYear("2023")
                .withDecisionTakeEffectDay("14")
                .withDecisionTakeEffectMonth("1")
                .withDecisionTakeEffectYear("2024")
                .withDecisionAuthouriser("Minister")
                .saveDecisionOutcome();


            Logger.Log("View Updated Decision Outcome");
            viewDecisionPage
                .hasDecisionOutcomeStatus("Approved")
                .hasTotalAmountApproved("1,000,000")
                .hasMadeDate("12-11-2023")
                .hasEffectiveFromDate("14-01-2024")
                .hasAuthoriser("Minister")
                .hasBusinessArea("Business Partner")
                .hasBusinessArea("Capital")
                .hasBusinessArea("Provider Market Oversight (PMO)");
            
    })
})
