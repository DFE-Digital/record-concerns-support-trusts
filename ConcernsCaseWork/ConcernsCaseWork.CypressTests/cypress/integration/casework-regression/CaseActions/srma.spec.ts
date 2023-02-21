import { Logger } from "../../../common/logger";
import { EditSrmaPage } from "../../../pages/caseActions/srma/editSrmaPage";
import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { ViewSrmaPage } from "../../../pages/caseActions/srma/viewSrmaPage";
import actionSummaryTable from "cypress/pages/caseActions/summary/actionSummaryTable";
import { toDisplayDate } from "cypress/support/formatDate";

describe("Testing the SRMA case action", () =>
{
    const editSrmaPage = new EditSrmaPage();
    const viewSrmaPage = new ViewSrmaPage();
    let now: Date;

    beforeEach(() => {
		cy.login();
        now = new Date();

        cy.basicCreateCase();

        addSrmaToCase();
	});


    it("Should create an SRMA action", () => 
    {
        Logger.Log("Checking SRMA validation");
        editSrmaPage
            .withNotesExceedingLimit()
            .save()
            .hasValidationError("Select status")
            .hasValidationError("Enter a valid date")
            .hasValidationError("Notes must be 2000 characters or less");

        Logger.Log("Filling out the SRMA form");
        editSrmaPage
            .withStatus("Trust Considering")
            .withDayTrustContacted("22")
            .withMonthTrustContacted("10")
            .withYearTrustContacted("2022")
            .withNotes("This is my notes")
            .save();
        
        Logger.Log("Add optional SRMA fields on the view page");
		actionSummaryTable
			.getOpenAction("SRMA")
			.then(row =>
			{
				row.hasName("SRMA")
				row.hasStatus("Trust considering")
				row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

        Logger.Log("Should not allow progression without a reason");

        viewSrmaPage
            .cancel()
            .hasValidationError("Enter the reason");

        Logger.Log("Configure reason");

        viewSrmaPage.addReason();
        editSrmaPage
            .save()
            .hasValidationError("Select status")

        editSrmaPage
            .withReason("Regions Group Intervention")
        cy.waitForJavascript();
        editSrmaPage.save();

        Logger.Log("Configure date accepted");
        viewSrmaPage
            .addDateAccepted();

        editSrmaPage
            .withDayAccepted("22")
            .save()
            .hasValidationError("22-- is an invalid date");

        editSrmaPage
            .withDayAccepted("22")
            .withMonthAccepted("22")
            .withYearAccepted("2022")
            .save()
            .hasValidationError("22-22-2022 is an invalid date");

        editSrmaPage
            .withDayAccepted("22")
            .withMonthAccepted("05")
            .withYearAccepted("2020")
            .save();

        Logger.Log("Configure date of visit");
        viewSrmaPage
            .addDateOfVisit();

        editSrmaPage
            .withStartDayOfVisit("22")
            .save()
            .hasValidationError("Start date 22-- is an invalid date");

        editSrmaPage
            .withStartDayOfVisit("22")
            .withStartMonthOfVisit("22")
            .withStartYearOfVisit("2022")
            .save()
            .hasValidationError("Start date 22-22-2022 is an invalid date");

        setValidStartDateOfVisit();

        editSrmaPage
            .withEndDayOfVisit("11")
            .save()
            .hasValidationError("End date 11-- is an invalid date");

        setValidStartDateOfVisit();

        editSrmaPage
            .withEndDayOfVisit("11")
            .withEndMonthOfVisit("33")
            .withEndYearOfVisit("2021")
            .save()
            .hasValidationError("End date 11-33-2021 is an invalid date");

        setValidStartDateOfVisit();

        editSrmaPage
            .withEndDayOfVisit("15")
            .withEndMonthOfVisit("01")
            .withEndYearOfVisit("2021")
            .save()
            .hasValidationError("Please ensure end date is same as or after start date.");

        setValidStartDateOfVisit();

        editSrmaPage
            .withEndDayOfVisit("15")
            .withEndMonthOfVisit("08")
            .withEndYearOfVisit("2021")
            .save();

        Logger.Log("Configuring date report sent to trust");
        viewSrmaPage.addDateReportSentToTrust();

        editSrmaPage
            .withDayReportSentToTrust("22")
            .save()
            .hasValidationError("22-- is an invalid date");

        editSrmaPage
            .withDayReportSentToTrust("05")
            .withMonthReportSentToTrust("44")
            .withYearReportSentToTrust("2021")
            .save()
            .hasValidationError("05-44-2021 is an invalid date");

        editSrmaPage
            .withDayReportSentToTrust("05")
            .withMonthReportSentToTrust("12")
            .withYearReportSentToTrust("2021")
            .save();

        viewSrmaPage
            .hasDateOpened(toDisplayDate(now))
            .hasStatus("Trust considering")
            .hasDateTrustContacted("22 October 2022")
            .hasReason("Regions Group (RG) Intervention")
            .hasDateAccepted("22 May 2020")
            .hasDateOfVisit("22 July 2021 - 15 August 2021")
            .hasDateReportSentToTrust("05 December 2021")
            .hasNotes("This is my notes");
    });

    it("Should configure an empty SRMA", () =>
    {
        editSrmaPage
            .withStatus("Trust Considering")
            .withDayTrustContacted("22")
            .withMonthTrustContacted("10")
            .withYearTrustContacted("2022")
            .save();

            actionSummaryTable
			.getOpenAction("SRMA")
			.then(row =>
			{
				row.hasStatus("Trust considering");
                row.hasCreatedDate(toDisplayDate(now))
				row.select();
			});

        viewSrmaPage
            .hasDateOpened(toDisplayDate(now))
            .hasStatus("Trust considering")
            .hasDateTrustContacted("22 October 2022")
            .hasReason("Empty")
            .hasDateAccepted("Empty")
            .hasDateOfVisit("Empty")
            .hasDateReportSentToTrust("Empty")
            .hasNotes("Empty");
    });

    it("Should edit an existing configured SRMA", () =>
    {
        fullConfigureSrma("Trust Considering");

        viewSrmaPage.addStatus();
        editSrmaPage.hasStatus("Trust Considering");
        editSrmaPage
            .withStatus("Preparing For Deployment")
            .save();

        viewSrmaPage.addDateTrustContacted();
        editSrmaPage
            .hasDayTrustContacted("22")
            .hasMonthTrustContacted("10")
            .hasYearTrustContacted("2022")

        editSrmaPage
            .withDayTrustContacted("11")
            .withMonthTrustContacted("05")
            .withYearTrustContacted("2021")
            .save();

        viewSrmaPage.addReason();
        editSrmaPage.hasReason("Regions Group Intervention");
        editSrmaPage
            .withReason("Offer Linked")
            .save();

        viewSrmaPage.addDateAccepted();
        editSrmaPage
            .hasDayAccepted("22")
            .hasMonthAccepted("05")
            .hasYearAccepted("2020");

        editSrmaPage
            .withDayAccepted("17")
            .withMonthAccepted("06")
            .withYearAccepted("2021")
            .save();

        viewSrmaPage.addDateOfVisit();

        editSrmaPage
            .hasStartDayOfVisit("22")
            .hasStartMonthOfVisit("07")
            .hasStartYearOfVisit("2021")
            .hasEndDayOfVisit("15")
            .hasEndMonthOfVisit("08")
            .hasEndYearOfVisit("2021");

        editSrmaPage
            .withStartDayOfVisit("23")
            .withStartMonthOfVisit("09")
            .withStartYearOfVisit("2022")
            .withEndDayOfVisit("27")
            .withEndMonthOfVisit("10")
            .withEndYearOfVisit("2022")
            .save();

        viewSrmaPage.addDateReportSentToTrust();
        editSrmaPage
            .hasDayReportSentToTrust("05")
            .hasMonthReportSentToTrust("12")
            .hasYearReportSentToTrust("2021");

        editSrmaPage
            .withDayReportSentToTrust("16")
            .withMonthReportSentToTrust("08")
            .withYearReportSentToTrust("2022")
            .save();

        viewSrmaPage.addNotes();

        editSrmaPage.hasNotes("This is my notes");

        editSrmaPage
        .withNotesExceedingLimit()
        .save()
        .hasValidationError("Notes must be 2000 characters or less");

        cy.waitForJavascript();

        editSrmaPage.withNotes("Editing the notes field")
            .save();

        viewSrmaPage
            .hasStatus("Preparing for deployment")
            .hasDateTrustContacted("11 May 2021")
            .hasReason("Offer linked with grant funding or other offer of support")
            .hasDateAccepted("17 June 2021")
            .hasDateOfVisit("23 September 2022 - 27 October 2022")
            .hasDateReportSentToTrust("16 August 2022")
            .hasNotes("Editing the notes field");
    });

    describe("Closing an SRMA", () =>
    {
        it("Should be able to resolve an SRMA", () =>
        {
            fullConfigureSrma("Deployed");

            viewSrmaPage.resolve();

            editSrmaPage.hasNotes("This is my notes");

            editSrmaPage
                .save()
                .hasValidationError("Confirm SRMA action is complete");

            editSrmaPage
                .confirmComplete();

            cy.waitForJavascript();

            editSrmaPage
                .withNotesExceedingLimit()
                .save()
                .hasValidationError("Notes must be 2000 characters or less");

            cy.waitForJavascript();

            editSrmaPage
                .withNotes("Resolved notes")
                .save();

            Logger.Log("View resolved SRMA");
            actionSummaryTable
			.getClosedAction("SRMA")
			.then(row =>
			{
				row.hasName("SRMA")
				row.hasStatus("SRMA Complete")
				row.hasCreatedDate(toDisplayDate(now))
                row.hasClosedDate(toDisplayDate(now))
				row.select();
			});

            viewSrmaPage
                .hasDateOpened(toDisplayDate(now))
                .hasDateClosed(toDisplayDate(now))
                .hasStatus("SRMA Complete")
                .hasDateTrustContacted("22 October 2022")
                .hasReason("Regions Group (RG) Intervention")
                .hasDateAccepted("22 May 2020")
                .hasDateOfVisit("22 July 2021 - 15 August 2021")
                .hasDateReportSentToTrust("05 December 2021")
                .hasNotes("Resolved notes");
        });

        it("Should cancel an SRMA", () =>
        {
            partiallyConfigureSrma("Trust Considering");

            viewSrmaPage.cancel();

            editSrmaPage.hasNotes("This is my notes");

            editSrmaPage
                .save()
                .hasValidationError("Confirm SRMA action was cancelled");
            
            cy.waitForJavascript();

            editSrmaPage
                .confirmCancelled()
                .withNotes("Cancelled notes")
                .save();

            Logger.Log("View cancelled SRMA");
            actionSummaryTable
			.getClosedAction("SRMA")
			.then(row =>
			{
				row.hasName("SRMA")
				row.hasStatus("SRMA Canceled")
				row.hasCreatedDate(toDisplayDate(now))
                row.hasClosedDate(toDisplayDate(now))
				row.select();
			});

            viewSrmaPage
                .hasDateOpened(toDisplayDate(now))
                .hasDateClosed(toDisplayDate(now))
                .hasStatus("SRMA Canceled")
                .hasDateTrustContacted("22 October 2022")
                .hasReason("Regions Group (RG) Intervention")
                .hasDateAccepted("")
                .hasDateOfVisit("")
                .hasDateReportSentToTrust("")
                .hasNotes("Cancelled notes");
        });

        it("Should decline an SRMA", () => {
            partiallyConfigureSrma("Trust Considering");

            viewSrmaPage.decline();

            editSrmaPage.hasNotes("This is my notes");

            editSrmaPage
                .save()
                .hasValidationError("Confirm SRMA action was declined by trust");
            
            cy.waitForJavascript();

            editSrmaPage
                .confirmDeclined()
                .withNotes("Declined notes")
                .save();

            Logger.Log("View declined SRMA");
            actionSummaryTable
			.getClosedAction("SRMA")
			.then(row =>
			{
				row.hasName("SRMA")
				row.hasStatus("SRMA Declined")
				row.hasCreatedDate(toDisplayDate(now))
                row.hasClosedDate(toDisplayDate(now))
				row.select();
			});

            viewSrmaPage
                .hasDateOpened(toDisplayDate(now))
                .hasDateClosed(toDisplayDate(now))
                .hasStatus("SRMA Declined")
                .hasDateTrustContacted("22 October 2022")
                .hasReason("Regions Group (RG) Intervention")
                .hasDateAccepted("")
                .hasDateOfVisit("")
                .hasDateReportSentToTrust("")
                .hasNotes("Declined notes");
        });
    });

    function setValidStartDateOfVisit()
    {
        editSrmaPage
            .withStartDayOfVisit("22")
            .withStartMonthOfVisit("07")
            .withStartYearOfVisit("2021");
    }

    function addSrmaToCase()
    {
        Logger.Log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('Srma')
        AddToCasePage.getAddToCaseBtn().click();
    }

    function partiallyConfigureSrma(status: string)
    {
        Logger.Log("Filling out the SRMA form");
        editSrmaPage
            .withStatus(status)
            .withDayTrustContacted("22")
            .withMonthTrustContacted("10")
            .withYearTrustContacted("2022")
            .withNotes("This is my notes")
            .save();

        Logger.Log("Add optional SRMA fields on the view page");
		actionSummaryTable
			.getOpenAction("SRMA")
			.then(row =>
			{
				row.select();
			});

        Logger.Log("Configure reason");
        viewSrmaPage.addReason();

        editSrmaPage
            .withReason("Regions Group Intervention")
            .save();
    }

    function fullConfigureSrma(status: string)
    {
        Logger.Log("Filling out the SRMA form");
        partiallyConfigureSrma(status);

        Logger.Log("Configure date accepted");
        viewSrmaPage.addDateAccepted();

        editSrmaPage
            .withDayAccepted("22")
            .withMonthAccepted("05")
            .withYearAccepted("2020")
            .save();

        Logger.Log("Configure date of visit");
        viewSrmaPage.addDateOfVisit();

        setValidStartDateOfVisit();

        editSrmaPage
            .withEndDayOfVisit("15")
            .withEndMonthOfVisit("08")
            .withEndYearOfVisit("2021")
            .save();

        Logger.Log("Configuring date report sent to trust");
        viewSrmaPage.addDateReportSentToTrust();

        editSrmaPage
            .withDayReportSentToTrust("05")
            .withMonthReportSentToTrust("12")
            .withYearReportSentToTrust("2021")
            .save();
    }
});