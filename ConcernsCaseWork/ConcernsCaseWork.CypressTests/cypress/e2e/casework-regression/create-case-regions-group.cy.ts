import { Logger } from "cypress/common/logger";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";
import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import createCaseSummary from "cypress/pages/createCase/createCaseSummary";
import {
	SourceOfConcernExternal,
} from "cypress/constants/selectorConstants";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";
import addRegionPage from "cypress/pages/createCase/addRegionPage";
import validationComponent from "cypress/pages/validationComponent";
import editRegionPage from "cypress/pages/createCase/editRegionPage";
import editConcernPage from "cypress/pages/editConcernPage";
import homePage from "cypress/pages/homePage";
import closedCasePage from "cypress/pages/closedCasePage";
import { ViewClosedCasePage } from "cypress/pages/createCase/viewClosedCasePage";
import { CaseBuilder } from "cypress/api/caseBuilder";
import { EditDecisionPage } from "cypress/pages/caseActions/decision/editDecisionPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { ViewDecisionPage } from "cypress/pages/caseActions/decision/viewDecisionPage";
import { DecisionOutcomePage } from "cypress/pages/caseActions/decision/decisionOutcomePage";
import addToCasePage from "cypress/pages/caseActions/addToCasePage";
import { EditSrmaPage } from "cypress/pages/caseActions/srma/editSrmaPage";
import { ViewSrmaPage } from "cypress/pages/caseActions/srma/viewSrmaPage";

describe("Creating a case", () => {
	const createCasePage = new CreateCasePage();
	const createConcernPage = new CreateConcernPage();
	const addDetailsPage = new AddDetailsPage();
	const addConcernDetailsPage = new AddConcernDetailsPage();
    const viewClosedCasePage = new ViewClosedCasePage();
    const editDecisionPage = new EditDecisionPage();
    const viewDecisionPage = new ViewDecisionPage();
    const decisionOutcomePage = new DecisionOutcomePage();
    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();

    describe("Case journey", () =>
    {
        beforeEach(() => {
            cy.login();
        });

        it("Should create a case with region group", () => {
            Logger.Log("Create a case");
            createCasePage
                .createCase()
                .withTrustName("Ashton West End Primary Academy")
                .selectOption()
                .confirmOption();
    
            createCaseSummary.hasTrustSummaryDetails("Ashton West End Primary Academy");
    
            Logger.Log("Create a valid case division");
            selectCaseDivisionPage
                .withCaseDivision("RegionsGroup")
                .continue();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "");
    
            Logger.Log("Check validation error if region is not selected");
            addRegionPage.nextStep();
            validationComponent.hasValidationError("Select region");
    
            cy.excuteAccessibilityTests();
    
            Logger.Log("Select valid region");
            addRegionPage
                .withRegion("London")
                .nextStep();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London");
    
            Logger.Log("Create a valid concern");
            createConcernPage
                .withConcernType("Governance")
                .withConcernRating("Amber-Green")
                .withMeansOfReferral(SourceOfConcernExternal)
                .addConcern();
    
            Logger.Log("Check Concern details are correctly populated");
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London")
                .hasConcernRiskRating("Amber Green")
                .hasConcernType("Governance");
    
            createConcernPage.nextStep();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London")
                .hasConcernRiskRating("Amber Green")
                .hasConcernType("Governance");
    
            Logger.Log("Populate risk to trust");
            addDetailsPage.withRiskToTrust("Red").nextStep();
    
            Logger.Log(
                "Check Trust, concern and risk to trust details are correctly populated"
            );
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London")
                .hasConcernType("Governance")
                .hasConcernRiskRating("Amber Green")
                .hasRiskToTrust("Red");
    
            Logger.Log("Add concern details with valid text limit");
            addConcernDetailsPage.withIssue("This is an issue").createCase();
    
            Logger.Log("Verify case details");
            caseManagementPage
                .hasTrust("Ashton West End Primary Academy")
                .hasRiskToTrust("Red")
                .hasConcerns("Governance", ["Amber", "Green"])
                .hasManagedBy("Regions Group", "London")
                .hasIssue("This is an issue");
    
            Logger.Log("Editing the existing region");
            caseManagementPage.editManagedBy();
    
            editRegionPage
                .hasRegion("London")
                .withRegion("South West")
                .apply();
    
            caseManagementPage.hasManagedBy("Regions Group", "South West");
    
            Logger.Log("Add another concern after case creation");
            caseManagementPage.addAnotherConcern();
    
            createConcernPage
                .withConcernType("Safeguarding")
                .withConcernRating("Red-Amber")
                .withMeansOfReferral(SourceOfConcernExternal)
                .addConcern();
    
            caseManagementPage
                .hasConcerns("Governance", ["Amber", "Green"])
                .hasConcerns("Safeguarding", ["Red", "Amber"]);

            Logger.Log("Check the available case actions");
            caseManagementPage.getAddToCaseBtn().click();
            addToCasePage.hasActions([
                "Decision",
                "NTI: Under consideration",
                "NTI: Warning letter",
                "NTI: Notice to improve",
                "SRMA (School Resource Management Adviser)"
            ])
            .cancel();
    
            Logger.Log("Close down the concerns");
            caseManagementPage
                .getEditConcern().first().click();
            
            editConcernPage.closeConcern().confirmCloseConcern();
    
            caseManagementPage
            .getEditConcern().first().click();
        
            editConcernPage.closeConcern().confirmCloseConcern();
    
            caseManagementPage.getCaseIDText()
            .then((caseId =>
            {
                caseManagementPage.getCloseCaseBtn().click();
                caseManagementPage.withRationaleForClosure("Closing").getCloseCaseBtn().click();
        
                Logger.Log("Viewing case is closed");
                homePage.getClosedCasesBtn().click();
        
                Logger.Log("Checking accessibility on closed case");
                cy.excuteAccessibilityTests();
        
                closedCasePage.getClosedCase(caseId).click();
        
                viewClosedCasePage.hasManagedBy("Regions Group", "South West");
            }));
        });
    });

    describe("Case actions", () =>
    {
        beforeEach(() => {
            cy.login();

            cy.basicCreateCase(CaseBuilder.buildRegionsGroupCase());
        });

        it("Should create a decision", () =>
        {
            caseManagementPage.addCaseAction("Decision");

            Logger.Log("Retrospective approval, ESFA dates and total amount requested should not be visible");
            editDecisionPage
                .hasNoRetrospectiveRequestField()
                .hasNoDateESFAField()
                .hasNoTotalAmountRequestedField();

            editDecisionPage.hasTypeOfDecisionOptions([
                "Notice to Improve (NTI)",
                "Section 128 (S128)",
                "Freedom of Information exemptions (FOI)"
            ]);

            Logger.Log("Creating a decision");
            editDecisionPage
			.withHasCrmCase("yes")
			.withCrmEnquiry("444")
			.withSubmissionRequired("yes")
			.withSubmissionLink("www.gov.uk")
			.withTypeOfDecision("NoticeToImprove")
			.withTypeOfDecision("Section128")
			.withSupportingNotes("These are some supporting notes!")
			.save();

            Logger.Log("Checking the decision values that have been set");
            actionSummaryTable
                .getOpenAction("Decision: Multiple Decision Types")
                .then(row =>
                {
                    row.select();
                });

            viewDecisionPage.createDecisionOutcome();

            Logger.Log("Decision outcome for RG does not have total amount approved");
            decisionOutcomePage.hasNoTotalAmountApprovedField();

            decisionOutcomePage.hasBusinessAreaOptions([
                "SFSO (Schools Financial Support and Oversight)",
                "RG (Regions Group)"
            ]);

            Logger.Log("Fill out the remaining fields");
            decisionOutcomePage
                .withDecisionOutcomeStatus("ApprovedWithConditions")
                .withDateDecisionMadeDay("24")
                .withDateDecisionMadeMonth("11")
                .withDateDecisionMadeYear("2022")
                .withDecisionTakeEffectDay("11")
                .withDecisionTakeEffectMonth("12")
                .withDecisionTakeEffectYear("2023")
                .withDecisionAuthouriser("DeputyDirector")
                .withBusinessArea("SchoolsFinancialSupportAndOversight")
                .withBusinessArea("RegionsGroup")
                .saveDecisionOutcome();

            Logger.Log("Viewing the decision changes");

            actionSummaryTable
                .getOpenAction("Decision: Multiple Decision Types")
                .then(row =>
                {
                    row.select();
                });

            Logger.Log("Ensure that fields that do not apply are not shown");
            viewDecisionPage
                .hasNoRetrospectiveRequestField()
                .hasNoDateESFAReceivedRequestField()
                .hasNoTotalAmountRequestedField()
                .hasNoTotalAmountApprovedField();

            viewDecisionPage
                .hasCrmEnquiry("444")
                .hasCrmCase("Yes")
                .hasSubmissionRequired("Yes")
                .hasSubmissionLink("www.gov.uk")
                .hasTypeOfDecision("Notice to Improve (NTI)")
                .hasTypeOfDecision("Section 128 (S128)")
                .hasSupportingNotes("These are some supporting notes!")
                .hasDecisionOutcomeStatus("Approved with conditions")
                .hasMadeDate("24 November 2022")
                .hasEffectiveFromDate("11 December 2023")
                .hasBusinessArea("SFSO")
                .hasBusinessArea("Regions Group")
                .hasAuthoriser("Deputy Director");
        });

        it("Should create an SRMA", () => {
        
            caseManagementPage.addCaseAction("Srma");
    
            Logger.Log("Filling out the SRMA form");
            editSrmaPage
                .withStatus("TrustConsidering")
                .withDayTrustContacted("22")
                .withMonthTrustContacted("10")
                .withYearTrustContacted("2022")
                .withNotes("This is my notes")
                .save();
    
            Logger.Log("Add optional SRMA fields on the view page");
            actionSummaryTable.getOpenAction("SRMA").then((row) => {
                row.hasName("SRMA");
                row.hasStatus("Trust considering");
                row.select();
            });
    
            Logger.Log("Checking accessibility on View SRMA");
            cy.excuteAccessibilityTests();
    
            Logger.Log("Configure reason");
            viewSrmaPage.addReason();
    
            Logger.Log("Verify hint text");
            editSrmaPage.hasReasonHintText();
        });
    });
});