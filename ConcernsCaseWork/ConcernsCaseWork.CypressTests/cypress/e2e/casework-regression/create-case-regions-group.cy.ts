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
import closeConcernPage from "cypress/pages/closeConcernPage";
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
            Logger.log("Create a case");
            createCasePage
                .createCase()
                .withTrustName("Ashton West End Primary Academy")
                .selectOption()
                .confirmOption();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy");
    
            Logger.log("Create a valid case division");
            selectCaseDivisionPage
                .withCaseDivision("RegionsGroup")
                .continue();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "");
    
            Logger.log("Check validation error if region is not selected");
            addRegionPage.nextStep();
            validationComponent.hasValidationError("Select region");
    
            cy.excuteAccessibilityTests();
    
            Logger.log("Select valid region");
            addRegionPage
                .withRegion("London")
                .nextStep();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London");


            Logger.log("Check has Regions Group specific means of referral hint text")
            createCaseSummary
                .hasHintText("Regions Group activity including SCCU, or other departmental activity")
                .hasHintText("Whistleblowing, self-reported by trust, SFSO, Ofsted or other government bodies");

            Logger.log("Create a valid concern");
            createConcernPage
                .withConcernType("Governance capability")
                .withConcernRating("Amber-Green")
                .withMeansOfReferral(SourceOfConcernExternal)
                .addConcern();
    
            Logger.log("Check Concern details are correctly populated");
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London")
                .hasConcernRiskRating("Amber Green")
                .hasConcernType("Governance capability");
    
            createConcernPage.nextStep();
    
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London")
                .hasConcernRiskRating("Amber Green")
                .hasConcernType("Governance capability");
    
            Logger.log("Populate risk to trust");
            addDetailsPage.withRiskToTrust("Red").nextStep();
    
            Logger.log(
                "Check Trust, concern and risk to trust details are correctly populated"
            );
            createCaseSummary
                .hasTrustSummaryDetails("Ashton West End Primary Academy")
                .hasManagedBy("Regions Group", "London")
                .hasConcernType("Governance capability")
                .hasConcernRiskRating("Amber Green")
                .hasRiskToTrust("Red");
    
            Logger.log("Add concern details with valid text limit");
            addConcernDetailsPage.withIssue("This is an issue").createCase();
    
            Logger.log("Verify case details");
            caseManagementPage
                .hasTrust("Ashton West End Primary Academy")
                .hasRiskToTrust("Red")
                .hasConcerns("Governance capability", ["Amber", "Green"])
                .hasManagedBy("Regions Group", "London")
                .hasIssue("This is an issue");
    
            Logger.log("Editing the existing region");
            caseManagementPage.editManagedBy();
    
            editRegionPage
                .hasRegion("London")
                .withRegion("South West")
                .apply();
    
            caseManagementPage.hasManagedBy("Regions Group", "South West");
    
            Logger.log("Add another concern after case creation");
            caseManagementPage.addAnotherConcern();
    
            createConcernPage
                .withConcernType("Safeguarding non-compliance")
                .withConcernRating("Red-Amber")
                .withMeansOfReferral(SourceOfConcernExternal)
                .addConcern();
    
            caseManagementPage
                .hasConcerns("Governance capability", ["Amber", "Green"])
                .hasConcerns("Safeguarding non-compliance", ["Red", "Amber"]);

            Logger.log("Check the available case actions");
            caseManagementPage.getAddToCaseBtn().click();
            addToCasePage.hasActions([
                "Decision",
                "NTI: Under consideration",
                "NTI: Warning letter",
                "NTI: Notice to improve",
                "SRMA (School Resource Management Adviser)"
            ])
            .cancel();
    
            Logger.log("Close down the concerns");
            caseManagementPage.closeConcern();
            closeConcernPage.confirmCloseConcern();
    
            caseManagementPage.closeConcern();
            closeConcernPage.confirmCloseConcern();
    
            caseManagementPage.getCaseIDText()
            .then((caseId =>
            {
                caseManagementPage.getCloseCaseBtn().click();
                caseManagementPage.withRationaleForClosure("Closing").getCloseCaseBtn().click();
        
                Logger.log("Viewing case is closed");
                homePage.getClosedCasesBtn().click();
        
                Logger.log("Checking accessibility on closed case");
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

            Logger.log("Retrospective approval, ESFA dates and total amount requested should not be visible");
            editDecisionPage
                .hasNoRetrospectiveRequestField()
                .hasNoDateESFAField()
                .hasNoTotalAmountRequestedField();

            editDecisionPage.hasTypeOfDecisionOptions([
                "Notice to Improve (NTI)",
                "Section 128 (S128)",
                "Freedom of Information exemptions (FOI)"
            ]);

            Logger.log("Creating a decision");
            editDecisionPage
			.withHasCrmCase("yes")
			.withCrmEnquiry("444")
			.withSubmissionRequired("yes")
			.withSubmissionLink("www.gov.uk")
			.withTypeOfDecision("NoticeToImprove")
			.withTypeOfDecision("Section128")
			.withSupportingNotes("These are some supporting notes!")
			.save();

            Logger.log("Checking the decision values that have been set");
            actionSummaryTable
                .getOpenAction("Decision: Multiple Decision Types")
                .then(row =>
                {
                    row.select();
                });

            viewDecisionPage.createDecisionOutcome();

            Logger.log("Decision outcome for RG does not have total amount approved");
            decisionOutcomePage.hasNoTotalAmountApprovedField();

            decisionOutcomePage.hasBusinessAreaOptions([
                "SFSO (Schools Financial Support and Oversight)",
                "RG (Regions Group)"
            ]);

            Logger.log("Fill out the remaining fields");
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

            Logger.log("Viewing the decision changes");

            actionSummaryTable
                .getOpenAction("Decision: Multiple Decision Types")
                .then(row =>
                {
                    row.select();
                });

            Logger.log("Ensure that fields that do not apply are not shown");
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
    
            Logger.log("Filling out the SRMA form");
            editSrmaPage
                .withStatus("TrustConsidering")
                .withDayTrustContacted("22")
                .withMonthTrustContacted("10")
                .withYearTrustContacted("2022")
                .withNotes("This is my notes")
                .save();
    
            Logger.log("Add optional SRMA fields on the view page");
            actionSummaryTable.getOpenAction("SRMA").then((row) => {
                row.hasName("SRMA");
                row.hasStatus("Trust considering");
                row.select();
            });
    
            Logger.log("Checking accessibility on View SRMA");
            cy.excuteAccessibilityTests();
    
            Logger.log("Configure reason");
            viewSrmaPage.addReason();
    
            Logger.log("Verify hint text");
            editSrmaPage.hasReasonHintText();
        });
    });
});