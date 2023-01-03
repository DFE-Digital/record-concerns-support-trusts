import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import { Logger } from "../../../common/logger";
import { EditNtiUnderConsiderationPage } from "../../../pages/caseActions/ntiUnderConsideration/editNtiUnderConsiderationPage";
import { ViewNtiUnderConsiderationPage } from "../../../pages/caseActions/ntiUnderConsideration/viewNtiUnderConsiderationPage";
import { CloseNtiUnderConsiderationPage } from "../../../pages/caseActions/ntiUnderConsideration/closeNtiUnderConsiderationPage";

describe("Testing the NTI under consideration", () =>
{
    const editNtiUnderConsiderationPage = new EditNtiUnderConsiderationPage();
    const viewNtiUnderConsiderationPage = new ViewNtiUnderConsiderationPage();
    const closeNtiUnderConsiderationPage = new CloseNtiUnderConsiderationPage();

    beforeEach(() => {
		cy.login();

        cy.basicCreateCase();

        addNtiUnderConsiderationToCase();
	});

    describe("When adding or editing a new NTI under consideration", () =>
    {
        it("Should create an NTI with the values specified and be able to edit the values", () =>
        {
            Logger.Log("Validating notes field");
            editNtiUnderConsiderationPage
                .withNotesExceedingLimit()
                .save()
                .hasValidationError("Notes provided exceed maximum allowed length (2000 characters).");

            editNtiUnderConsiderationPage
                .withReasons("Cash flow problems")
                .withReasons("Safeguarding")
                .withNotes("These are my notes")
                .save();

            Logger.Log("Validate the NTI on the view page");
                cy.get("#open-case-actions td")
                    .getByTestId("NTI Under Consideration").click();

            viewNtiUnderConsiderationPage
                .hasReasons("Cash flow problems")
                .hasReasons("Safeguarding")
                .hasNotes("These are my notes");
                
            viewNtiUnderConsiderationPage.edit();

            editNtiUnderConsiderationPage
                .hasReasons("Cash flow problems")
                .hasReasons("Safeguarding")
                .hasNotes("These are my notes");

            Logger.Log("Validating notes field");
            editNtiUnderConsiderationPage
                .withNotesExceedingLimit()
                .save()
                .hasValidationError("Notes provided exceed maximum allowed length (2000 characters).");

            editNtiUnderConsiderationPage
                .clearReasons()
                .withReasons("Governance concerns")
                .withReasons("Risk of insolvency")
                .withNotes("Edited my notes")
                .save();

            viewNtiUnderConsiderationPage
                .hasReasons("Governance concerns")
                .hasReasons("Risk of insolvency")
                .hasNotes("Edited my notes"); 
        });
    });

    describe("When adding a blank NTI under consideration", () =>
    {
        it("Should set the fields to empty", () =>
        {
            editNtiUnderConsiderationPage
                .save();

            Logger.Log("Validate the NTI on the view page");
            cy.get("#open-case-actions td")
                .getByTestId("NTI Under Consideration").click();

            viewNtiUnderConsiderationPage
                .hasReasons("Empty")
                .hasNotes("Empty");   
        });
    });

    describe("When closing an NTI under consideration", () =>
    {
        it("Should be able to close the NTI under consideration", () =>
        {
            editNtiUnderConsiderationPage
                .withReasons("Cash flow problems")
                .withReasons("Safeguarding")
                .withNotes("These are my notes")
                .save();

            Logger.Log("Close NTI on the view page");
            cy.get("#open-case-actions td")
                .getByTestId("NTI Under Consideration").click();

            viewNtiUnderConsiderationPage.close();

            Logger.Log("Validating we cannot close without a reason")
            closeNtiUnderConsiderationPage
                .close()
                .hasValidationError("Please select a reason for closing NTI under consideration");

            closeNtiUnderConsiderationPage.hasNotes("These are my notes");

            closeNtiUnderConsiderationPage
                .withStatus("No further action being taken")
                .withNotes("These are my final notes")
                .close();

            Logger.Log("Review the closed NTI under consideration");
                cy.get("#close-case-actions td")
                    .getByTestId("NTI Under Consideration").click();

            viewNtiUnderConsiderationPage
                .hasReasons("Cash flow problems")
                .hasReasons("Safeguarding")
                .hasStatus("No further action being taken")
                .hasNotes("These are my final notes")
                .cannotEdit()
                .cannotClose();
        });
    });

    function addNtiUnderConsiderationToCase()
    {
        Logger.Log("Adding Notice To Improve");
        CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('NtiUnderConsideration');
        AddToCasePage.getAddToCaseBtn().click();
    }
});