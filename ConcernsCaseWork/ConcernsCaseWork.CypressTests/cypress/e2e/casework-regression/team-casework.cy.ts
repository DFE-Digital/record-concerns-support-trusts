import { Logger } from "cypress/common/logger";
import { EnvUsername } from "cypress/constants/cypressConstants";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import homePage from "cypress/pages/homePage";
import teamCaseworkPage from "cypress/pages/teamCaseworkPage";
import caseApi from "cypress/api/caseApi";
import { CaseBuilder } from "cypress/api/caseBuilder";
import { CreateCaseRequest, PutTeamRequest, GetTeamByOwnerResponse } from "cypress/api/apiDomain";
import paginationComponent from "cypress/pages/paginationComponent";
import caseMangementPage from "cypress/pages/caseMangementPage";

describe("Team casework tests", () => {
  describe("When we create a case for a team member", () => {
    let caseId: string;

    beforeEach(() => {
      // Ensure that the automation user has been registered in the system
      cy.login();

      cy.login({
        username: "Reassign.Test@education.gov.uk"
      });

      cy.basicCreateCase()
        .then((response) => {
          caseId = response.urn + "";
        });
    });

    it("Should be able to add team member", () => {
      cy.visit("/TeamCasework");
      homePage.selectColleagues();

      const email = Cypress.env(EnvUsername);
      const name = email.split("@")[0];
      const ownerid = Cypress.env(EnvUsername);

      let req: PutTeamRequest =
      {
        OwnerID: ownerid,
        TeamMembers: []
      };
      caseApi.put(ownerid, req)
        .then(() => {
          teamCaseworkPage
            .selectTeamMember('Svc-rdscc-e2etest');
        
          Logger.Log("Ensure that the case for the user is displayed");
          caseworkTable
            .getRowByCaseId(caseId)
            .then(row => {
              row.hasCaseId(caseId);
              row.hasOwner(name);
            });

          Logger.Log("Checking accessibility on team casework");
          cy.excuteAccessibilityTests();
		
		  Logger.Log("Ensuring no cases are displayed when there are no cases");

   
      Logger.Log('should navigate to Team Casework, select colleagues, and update cases');
          // Visit the Team Casework page
          cy.visit('/teamcasework');
      
          // Check if there are cases
          caseworkTable
          .getRowByCaseId(caseId)
          .then(row => {
            row.hasCaseId(caseId);
            row.hasOwner(name);
          });

      
          // Click on the button to go to the Select Colleagues page
          cy.get('[data-testid="select-colleagues"]').click();

            // Remove the added colleague
            cy.get('.user-remove').first().click();
      
            // Verify that the colleague is removed
            cy.get('.user-colleagues-row').should('have.length', 0);
            cy.wait(5000);
        
            // Apply the changes
            cy.get('[data-testid="save"]').click();
            cy.wait(5000);


          // Click on the button to go to the Select Colleagues page
          cy.get('[data-testid="select-colleagues"]').click();

            // Remove the added colleague
            cy.get('.user-remove').first().click();
            Logger.Log('Check for the absence of cases after changes are applied');
            teamCaseworkPage.applyingChanges();
            teamCaseworkPage.hasNoCases();
      
      
          // // Add a colleague (replace "John Doe" with the actual name or email)
          // cy.get('#select-colleagues-input').type(name);
          // cy.get('.autocomplete-option').first().click();
      
          // // Verify that the colleague is added
          // cy.get('.user-colleagues-row').should('have.length', 1);
      
        
      
//  // Clear team members
//  const ownerid = Cypress.env(EnvUsername);
//  let req: PutTeamRequest = {
//    OwnerID: ownerid,
//    TeamMembers: [],
//  };

//  caseApi.put(ownerid, req)
//    .then(response => {
//      console.log('Team members cleared successfully');
//      teamCaseworkPage.hasNoCases();
//    });

   
    
      
  


    //  homePage.selectColleagues();
       

          Logger.Log("Checking accessibility on team casework with no cases");
          cy.excuteAccessibilityTests();
        });
    });

    describe("When we have many open cases", () => {
      const otherTeamMemberOne: string = "Automation.TeamMember1@noemail.com";
      const otherTeamMemberTwo: string = "Automation.TeamMember2@noemail.com";

      beforeEach(() => {
        cy.login();

        const ownerid = Cypress.env(EnvUsername);

        //Check we have a team
        caseApi.getTeamByTeam(ownerid)
          .then((response) => {
            const s = response.data;
            if (s.teamMembers.length === 0) {
              let req: PutTeamRequest =
              {
                OwnerID: ownerid,
                TeamMembers: [otherTeamMemberOne, otherTeamMemberTwo]
              };
              caseApi.put(ownerid, req)
                .then(() => { });
            } else {
              if (!s.teamMembers.includes(otherTeamMemberOne)) {
                let req: PutTeamRequest =
                {
                  OwnerID: ownerid,
                  TeamMembers: [otherTeamMemberOne]
                };
                caseApi.put(ownerid, req)
                  .then(() => { });
              }
            }
          });

        // Ensure we have enough cases for the owner
        caseApi.getOpenCasesForTeamByOwner(Cypress.env(EnvUsername))
          .then((response) => {
            const currentCases = response.paging.recordCount;
            const casesToCreate = 15 - currentCases;

            if (casesToCreate > 0) {
              const cases = CaseBuilder.bulkCreateOpenCasesWithOwner(casesToCreate, otherTeamMemberOne);

              cy.wrap(cases).each((request: CreateCaseRequest, index, $list) => {
                caseApi.post(request)
                  .then(() => { });
              });

              // Wait 1ms per case created
              // Each case is created one 1ms apart so that we don't break the table constraint
              // Max will be 15ms
              cy.wait(casesToCreate);

              cy.reload();
            }
          });
      });

      it("Should display them in separate pages with 5 items per page and we should be able to move between them", () => {
        cy.visit("/TeamCasework");
        let pageOneCases: Array<string> = [];
        let pageTwoCases: Array<string> = [];

        caseworkTable
          .getOpenCaseIds()
          .then((caseIds: Array<string>) => {
            pageOneCases = caseIds;

            Logger.Log("Ensure we have 5 cases on page one");
            expect(pageOneCases.length).to.eq(5);

            Logger.Log("Moving to the second page using the direct link");
            paginationComponent.goToPage("2");
            return caseworkTable.getOpenCaseIds();
          })
          .then((caseIds: Array<string>) => {
            pageTwoCases = caseIds;

            Logger.Log("Ensure we have 5 cases on page 2");
            expect(pageTwoCases.length).to.equal(5);

            Logger.Log("Ensure that the cases on page one and two are different");
            hasNoSimilarElements(pageOneCases, pageTwoCases);

            Logger.Log("Move to the previous page, which is page 1");
            paginationComponent.previous();
            return caseworkTable.getOpenCaseIds();
          })
          .then((caseIds: Array<string>) => {
            Logger.Log("On moving to page one, we should get the exact same cases");
            expect(caseIds).to.deep.equal(pageOneCases);

            Logger.Log("Move to the next page, which is page 2");
            paginationComponent.next();
            return caseworkTable.getOpenCaseIds();
          })
          .then((caseIds: Array<string>) => {
            Logger.Log("On moving to page two, we should get the exact same cases");
            expect(caseIds).to.deep.equal(pageTwoCases);

            // We had relative instead of absolute path links so it didn't work when pagination was added
            Logger.Log("Ensure the case loads when clicking the link");
            const caseIdToView = caseIds[0];
            caseworkTable.getRowByCaseId(caseIdToView)
              .then((row) => {
                row.select();
                caseMangementPage.getCaseIDText();
              })
              .then(managementCaseId => {
                expect(caseIdToView).to.equal(managementCaseId);
              });
          });
      });
    });

    function hasNoSimilarElements(first: Array<string>, second: Array<string>) {
      var firstSet = new Set(first);

      const match = second.some(e => firstSet.has(e));

      expect(match).to.be.false;
    }
  })});
