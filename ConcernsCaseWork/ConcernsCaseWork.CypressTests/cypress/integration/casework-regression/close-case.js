import { LogTask } from "../../support/constants";

describe("User closes a case", () => {
	let ukprn = "";
	let closedCaseId = "";
	
	before(() => {
		//cy.login();
cy.visit("/");

		cy.task(LogTask, "User creates a case");
		cy.get('[href="/case"]').click();
		cy.get("#search").should("be.visible");
		cy.randomSelectTrust().then(term => {
			ukprn = term;
		})

		cy.get("#search__option--0").click();
		cy.getById("continue").click();
		cy.selectConcernType();
		cy.selectRiskToTrust();
		cy.selectTerritory();
		cy.enterConcernDetails();

		cy.task(LogTask, "Opens an active case");
		cy.get('.buttons-topOfPage [href="/"]').click();
		//Stores the ID of the case
		cy.get('#your-casework tr:nth-child(1)  td:nth-child(1)  a').then(($el) => {
			closedCaseId = $el.text();
		});

		cy.get('#your-casework tr:nth-child(1) td:nth-child(1) a').click();
	});

	afterEach(() => {
		cy.storeSessionData();
	});

	it("Should allow a user to close a case", () => {
		cy.task(LogTask, "User can close an open concern");
		cy.closeAllOpenConcerns();

		cy.task(LogTask, "User can close an open case");
		cy.get(
			'[href*="/case/' + closedCaseId + '/management/closure"]'
		).click();
		cy.get("#case-outcomes").type("SAMPLE CLOSURE TEXT");
		cy.getById("close-case-button").click();

		cy.task(LogTask, "Case should be marked as closed and removed from active cases");
		cy.get("#main-content tr:nth-child(1) td:nth-child(1) a").then(($el) => {
			cy.wrap($el.text()).as("caseIdAfter");
			//Checks the the Case ID is no longer listed as Active
			expect(closedCaseId).to.not.equal($el.text());
		});
		cy.get('[href*="/case/closed"]').click();
		//Checks the case ID is listed as closed
		cy.get("#main-content tr:nth-child(1) td:nth-child(1)").should('contain', closedCaseId);

		cy.task(LogTask, "The Trust page should contain a closed cases table");
		cy.visitPage('/trust')
		cy.get("#search").type(ukprn);
		cy.get("#search__option--0").click();
		cy.getById("continue").click();
		cy.get('table:nth-child(6) > caption').scrollIntoView();
		cy.get('table:nth-child(6) > caption').should(($titl) => {
			expect($titl.text().trim()).equal("Closed cases");
		});

		cy.task(LogTask, "Case should be visible in the Trust page under closed cases");
		cy.get('table:nth-child(6) > tbody').children().should('contain', closedCaseId);

		cy.task(LogTask, "User is taken to the Case Page when clicking the closed case id");
		cy.get('table:nth-child(6) > tbody').children().contains(closedCaseId).click();

		cy.visit(`${Cypress.env("url")}/case/${closedCaseId}/management`);

		// Needs to be revisted as you can't see a closed case in this view anymore
		// cy.get('[id="tab_trust-overview"]').click();

		// cy.task(LogTask, "Case should be visible in the Case Page Trust Tab under closed cases");
		// cy.get('table:nth-child(4) > caption').scrollIntoView();
		// cy.get('table:nth-child(4) > caption').should(($titl) => {
		// 	expect($titl.text().trim()).equal("Closed cases")
		// });
		// cy.get('table:nth-child(4) > tbody').children().should('contain', closedCaseId)

	});

	after(function () {
		cy.clearLocalStorage();
		cy.clearCookies();
	});
});
