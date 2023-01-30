import { Logger } from "../../../common/logger";
import { EditTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/editTrustFinancialForecastPage";
import { ViewTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/viewTrustFinancialForecastPage";
import { CloseTrustFinancialForecastPage } from "../../../pages/caseActions/trustFinancialForecast/closeTrustFinancialForecastPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
const axe = require("axe-core");
import "cypress-axe";


describe(("User can add trust financial forecast to an existing case"), () => {

	const editTFFPage = new EditTrustFinancialForecastPage();
	const viewTFFPage = new ViewTrustFinancialForecastPage();
	const closeTFFPage = new CloseTrustFinancialForecastPage();

    beforeEach(() => {
		cy.login();
		cy.basicCreateCase();
		addTFFToCase();
	});

    after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});


    it("Concern TFF - Creating a TFF with all values", function () {
		
		Logger.Log("Notes Exceeding allowed limit")
        editTFFPage
            .withNotesExceedingLimit()
            .save()
            .hasValidationError("Supporting notes: Exceeds maximum allowed length (2000 characters).");
		
		Logger.Log("Create a TFF will all values");
		editTFFPage
			.withForecastingTool("Current year - Spring")
			.withDayReviewHappened("26")
			.withMonthReviewHappened("01")
			.withYearReviewHappened("2023")
			.withDayTrustResponded("27")
			.withMonthTrustResponded("02")
			.withYearTrustResponded("2024")
			.withTrustResponseSatisfactory("Satisfactory")
			.withSRMAOffered("Yes")
			.withNotes("Supporting notes")
			.save();

		Logger.Log("Validate the TFF on the view page");
		cy.get("#open-case-actions td")
			.getByTestId("Trust Financial Forecast (TFF)").click();

		Logger.Log("View the created TFF with expected values");
		viewTFFPage
			.hasForecastingTool("Current year - Spring")
			.hasInitialReviewDate("26 January 2023")
			.hasTrustRespondedDate("27 February 2024")
			.hasTrustResponse("Satisfactory")
			.hasSRMABeenOffered("Yes")
			.hasNotes("Supporting notes");
	});


	it("Concern TFF - Creating a TFF with empty values", function () {
		Logger.Log("Create a TFF will empty values");
		editTFFPage
			.save();

		Logger.Log("Validate the TFF on the view page");
		cy.get("#open-case-actions td")
			.getByTestId("Trust Financial Forecast (TFF)").click();

		Logger.Log("View the created TFF with expected values");
		viewTFFPage
			.hasForecastingTool("Empty")
			.hasInitialReviewDate("Empty")
			.hasTrustRespondedDate("Empty")
			.hasTrustResponse("Empty")
			.hasSRMABeenOffered("Empty")
			.hasNotes("Empty");
	});

	it("Concern TFF - Edit a TFF", function () {
		Logger.Log("Create a TFF will empty values");
		editTFFPage
			.save();

		Logger.Log("Validate the TFF on the view page");
		cy.get("#open-case-actions td")
			.getByTestId("Trust Financial Forecast (TFF)").click();

		Logger.Log("View the created TFF with expected values");
		viewTFFPage
			.hasForecastingTool("Empty")
			.hasInitialReviewDate("Empty")
			.hasTrustRespondedDate("Empty")
			.hasTrustResponse("Empty")
			.hasSRMABeenOffered("Empty")
			.hasNotes("Empty")
			.edit();

		Logger.Log("Edit a TFF will all values");
		editTFFPage
			.withForecastingTool("Previous year - Spring")
			.withDayReviewHappened("05")
			.withMonthReviewHappened("07")
			.withYearReviewHappened("2022")
			.withDayTrustResponded("06")
			.withMonthTrustResponded("07")
			.withYearTrustResponded("2024")
			.withTrustResponseSatisfactory("Not satisfactory")
			.withSRMAOffered("No")
			.withNotes("Edited notes")
			.save();

		Logger.Log("Validate the TFF on the view page");
		cy.get("#open-case-actions td")
			.getByTestId("Trust Financial Forecast (TFF)").click();

		Logger.Log("Validate the TFF on the view page");
		viewTFFPage
			.hasForecastingTool("Previous year - Spring")
			.hasInitialReviewDate("05 July 2022")
			.hasTrustRespondedDate("06 July 2024")
			.hasTrustResponse("Not satisfactory")
			.hasSRMABeenOffered("No")
			.hasNotes("Edited notes");
	});

	it("Concern TFF - Close a TFF", function () {
		Logger.Log("Create a TFF will empty values");
		editTFFPage
			.save();

		Logger.Log("Validate the TFF on the view page");
		cy.get("#open-case-actions td")
			.getByTestId("Trust Financial Forecast (TFF)").click();

		Logger.Log("Continue to close the TFF");
		cy.getById("trust-financial-forecast-close-button").click();

		Logger.Log("Close the TFF");
		closeTFFPage
			.withNotes("TFF Closed")
			.close();

		Logger.Log("Validate the closed TFF on the view page");
		cy.get("#close-case-actions td")
			.getByTestId("Trust Financial Forecast (TFF)").click();

		viewTFFPage
			.hasNotes("TFF Closed");

	});

	function addTFFToCase()
    {
        Logger.Log("Adding Trust Financial Forecasr");
        CaseManagementPage.getAddToCaseBtn().click();
       	AddToCasePage.addToCase('TrustFinancialForecast')
    	AddToCasePage.getAddToCaseBtn().click();
    }
});