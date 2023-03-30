import { Logger } from "cypress/common/logger";
import { EnvUsername } from "cypress/constants/cypressConstants";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import homePage from "cypress/pages/homePage";
import teamCaseworkPage from "cypress/pages/teamCaseworkPage";

describe("User interactions via Create Case route", () => {

	let caseId: string;

	beforeEach(() => {

		cy.login({
			username: "Reassign.Test@education.gov.uk"
		});

		cy.basicCreateCase()
		.then((id) =>
		{
			caseId = id + "";
		});
	});

	it("Should allow you to select a list of colleagues on team casework", () =>
	{
		cy.visit("/TeamCasework");
		homePage.selectColleagues();

		const email = Cypress.env(EnvUsername);
		const name = email.split("@")[0];

		teamCaseworkPage
			.deselectAllTeamMembers()
			.selectTeamMember(email)
			.save();

		Logger.Log("Ensure that the case for the user is displayed");
		caseworkTable
			.getRowByCaseId(caseId)
			.then(row =>
			{
				row.hasCaseId(caseId);
				row.hasOwner(name);
			});

		homePage.selectColleagues();
		teamCaseworkPage
			.deselectAllTeamMembers()
			.save();

		teamCaseworkPage.hasNoCases();
	});
});