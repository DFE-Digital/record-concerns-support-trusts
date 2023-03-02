import teamCaseworkPage from "/cypress/pages/teamCaseworkPage";
import homePage from "/cypress/pages/homePage";
import { Logger } from "cypress/common/logger";

describe("User interactions via Create Case route", () => {
	beforeEach(() => {
		//cy.login();
cy.visit("/");
	});

	it("Should allow you to select a list of colleagues on team casework", () =>
	{
		Logger.Log("User clicks Team Casework tab and is taken to the team casework page");
		homePage.getTeamCaseworkBtn().click();
		homePage.getTeamCaseworkHeading().should('contain.text', 'Your team casework');

		Logger.Log("User clicks select colleagues is taken to the Select Colleagues page");
		teamCaseworkPage.getSelectColleagesBtn().click();
		homePage.getHeadingText().should('contain.text', 'Select Colleagues to Show in Team Casework');

		Logger.Log("User can see a list of selectable colleagues");
		teamCaseworkPage.getCheckbox().its('length').should('be.gt', 0);
	});
});