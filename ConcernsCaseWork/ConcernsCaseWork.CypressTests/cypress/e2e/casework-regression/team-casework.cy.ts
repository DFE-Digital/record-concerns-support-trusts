import { Logger } from "cypress/common/logger";
import { EnvUsername } from "cypress/constants/cypressConstants";
import caseworkTable from "cypress/pages/caseRows/caseworkTable";
import homePage from "cypress/pages/homePage";
import teamCaseworkPage from "cypress/pages/teamCaseworkPage";
import caseApi from "cypress/api/caseApi";
import { CaseBuilder } from "cypress/api/caseBuilder";
import { CreateCaseRequest, PutTeamRequest } from "cypress/api/apiDomain";
import paginationComponent from "cypress/pages/paginationComponent";
import caseMangementPage from "cypress/pages/caseMangementPage";
import { toDisplayDate } from "cypress/support/formatDate";

describe("Team casework tests", () => {
    describe("When we create a case for a team member", () => {
        let caseId: string;
        let email: string;
        let name: string;
        let now: Date;
        let alternativeEmail: string;
        let alternativeName: string;

        beforeEach(() => {
            // Ensure that the automation user has been registered in the system
            cy.login();

            cy.visit("/");

            alternativeEmail = "Reassign.Test@education.gov.uk";

            cy.login({
                username: alternativeEmail
            });

            cy.visit("/");

            now = new Date();

            email = Cypress.env(EnvUsername);
            name = email.split("@")[0];
            alternativeName = alternativeEmail.split("@")[0].split(".")[0];

            const caseRequest = CaseBuilder.buildOpenCase();
            caseRequest.trustUkprn = "10060447";

            let req: PutTeamRequest =
            {
                OwnerID: alternativeEmail,
                TeamMembers: []
            };
            caseApi
                .put(alternativeEmail, req)
                .then(() => {
                    return cy.basicCreateCase(caseRequest)
                })
                .then((response) => {
                    caseId = response.urn + "";
                });
        });

        it("Should be able to add and remove team member", () => {
            cy.visit("/");
            homePage.viewOtherCases();
            homePage.selectColleagues();

            Logger.log("Checking accessibility on team casework");
            cy.excuteAccessibilityTests();

            teamCaseworkPage
                .selectTeamMember(name)
                .hasTeamMember(email, name)
                .save();

            Logger.log("Ensure that the case for the user is displayed");
            caseworkTable
                .getRowByCaseId(caseId)
                .then(row => {
                    row
                        .hasCaseId(caseId)
                        .hasConcern("Financial compliance")
                        .hasRiskToTrust("Amber")
                        .hasRiskToTrust("Green")
                        .hasManagedBy("SFSO", "Midlands and West - West Midlands")
                        .hasTrust("Westfield Academy")
                        .hasLastUpdatedDate(toDisplayDate(now))
                        .hasOwner(name);
                });

            Logger.log("Checking accessibility on team casework");
            cy.excuteAccessibilityTests();

            Logger.log("Ensuring no cases are displayed when there are no selected colleagues");
            homePage.selectColleagues();

            teamCaseworkPage
                .removeAddedColleagues()
                .hasNoTeamMember(name)
                .save()
                .hasNoCases();

            Logger.log("Ensure we cannot enter a duplicated team member, when they have already been selected");
            homePage.selectColleagues();

            teamCaseworkPage
                .selectTeamMember(name);
            
            teamCaseworkPage
                .enterTeamMember(name)
                .hasNoResultsFound()
                .save();

            Logger.log("Ensure that when we go back to the home page and back to select colleagues we cannot select previously selected members");
            homePage.selectColleagues();

            teamCaseworkPage.enterTeamMember(name).hasNoResultsFound();

            Logger.log("Ensure that when the team member is removed, they can be selected again");
            teamCaseworkPage
                .removeAddedColleagues()
                .hasNoTeamMember(name);

            teamCaseworkPage
                .selectTeamMember(name);

            Logger.log("Ensure that the logged in user cannot be selected");
            teamCaseworkPage.enterTeamMember(alternativeName).hasNoResultsFound();
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

                        Logger.log("Ensure we have 5 cases on page one");
                        expect(pageOneCases.length).to.eq(5);

                        Logger.log("Moving to the second page using the direct link");
                        paginationComponent.goToPage("2");
                        return caseworkTable.getOpenCaseIds();
                    })
                    .then((caseIds: Array<string>) => {
                        pageTwoCases = caseIds;

                        Logger.log("Ensure we have 5 cases on page 2");
                        expect(pageTwoCases.length).to.equal(5);

                        Logger.log("Ensure that the cases on page one and two are different");
                        hasNoSimilarElements(pageOneCases, pageTwoCases);

                        Logger.log("Move to the previous page, which is page 1");
                        paginationComponent.previous();
                        return caseworkTable.getOpenCaseIds();
                    })
                    .then((caseIds: Array<string>) => {
                        Logger.log("On moving to page one, we should get the exact same cases");
                        expect(caseIds).to.deep.equal(pageOneCases);

                        Logger.log("Move to the next page, which is page 2");
                        paginationComponent.next();
                        return caseworkTable.getOpenCaseIds();
                    })
                    .then((caseIds: Array<string>) => {
                        Logger.log("On moving to page two, we should get the exact same cases");
                        expect(caseIds).to.deep.equal(pageTwoCases);

                        // We had relative instead of absolute path links so it didn't work when pagination was added
                        Logger.log("Ensure the case loads when clicking the link");
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
    });
});
