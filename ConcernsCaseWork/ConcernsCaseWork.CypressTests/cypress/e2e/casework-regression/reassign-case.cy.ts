import caseApi from "cypress/api/caseApi";
import { Logger } from "cypress/common/logger";
import { AdminClaim, EnvUsername } from "cypress/constants/cypressConstants";
import caseMangementPage from "../../pages/caseMangementPage";
import editCaseMangementPage from "../../pages/editCaseManagementPage";


describe("Testing reassigning cases", () =>
{
    let caseId: number;
    let email: string;
    let name: string;

    beforeEach(() =>
    {
        // Testing reassign requires admin permission
        cy.login(
            {
                role: AdminClaim
            }
        );

        cy.basicCreateCase()
        .then((id =>
        {
            caseId = id;
        }));

        email = Cypress.env(EnvUsername);
        name = email.split("@")[0];
    });

    it("Should be able to edit an existing case owner", () =>
    {
        Logger.Log("editing the existing case owner");

        // switch the case owner
        updateCaseOwner(caseId);

        caseMangementPage
            .hasCaseOwner("Automation User")
            .editCaseOwner();

        editCaseMangementPage
            .hasCaseOwner("Automation.User@education.gov.uk")
            .withCaseOwner("sv")
            .selectCaseOwnerOption()
            .hasCaseOwner(email)
            .save();

        caseMangementPage
            .hasCaseOwnerReassignedBanner()
            .hasCaseOwner(name);

        caseMangementPage.editCaseOwner();

        Logger.Log("We get no results found if there are no matching results");
            editCaseMangementPage
                .withCaseOwner("fghhj")
                .hasNoCaseOwnerResults();

        Logger.Log("We cannot set a blank case owner");
        editCaseMangementPage
            .clearCaseOwner()
            .save()
            .hasValidationError("A valid case owner must be selected")
            .hasCaseOwner(email);

        Logger.Log("We do not get a notification if we do not change the case owner");
        editCaseMangementPage.save();
        caseMangementPage
            .hasNoCaseOwnerReassignedBanner()
            .hasCaseOwner(name);
    });
    
    function updateCaseOwner(caseId: number) {
        caseApi.get(caseId)
        .then((caseResponse) =>
        {
            caseResponse.createdBy = "Automation.User@education.gov.uk";
            caseApi.patch(caseId, caseResponse);
            cy.reload();
        })
    }

});