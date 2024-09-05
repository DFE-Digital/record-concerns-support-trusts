import CaseManagementPage from "../../../pages/caseMangementPage";
import AddToCasePage from "../../../pages/caseActions/addToCasePage";
import "cypress-axe";

import { DeleteCaseGroupClaim } from "cypress/constants/cypressConstants";


describe("User can add tte to an existing case", () => {


	let now: Date;
	
	beforeEach(() => {
		cy.login({
			role: DeleteCaseGroupClaim,
		});
		now = new Date();

		cy.basicCreateCase();

		CaseManagementPage.getAddToCaseBtn().click();
        AddToCasePage.addToCase('TargetedTrustEngagement');
        AddToCasePage.getAddToCaseBtn().click();
	});


});
