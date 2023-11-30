import caseworkTable from "../../pages/caseRows/caseworkTable";
import { toDisplayDate } from "../../support/formatDate";
import CaseManagementPage from "../../pages/caseMangementPage";
import caseApi from "cypress/api/caseApi";
import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCaseRequest } from "cypress/api/apiDomain";
import { CaseBuilder } from "cypress/api/caseBuilder";
import { Logger } from "cypress/common/logger";
import paginationComponent from "cypress/pages/paginationComponent";
import caseMangementPage from "../../pages/caseMangementPage";

describe("Your casework tests", () =>
{
    describe("When we create a case", () =>
    {
        let caseId: string;
        let trustName: string;
        let now: Date;
    
        beforeEach(() => {
            cy.login();
            now = new Date();
    
            cy.basicCreateCase()
            .then((caseResponse) => {
                caseId = caseResponse.urn + "";
                return CaseManagementPage.getTrust()
            })
            .then((trust: string) =>
            {
                trustName = trust.trim();
                cy.visit("/");
            });
        });

        it("Should appear in the your casework section", () =>
        {
            caseworkTable
                .getRowByCaseId(caseId)
                .then((row) =>
                {
                    row
                        .hasCaseId(caseId)
                        .hasCreatedDate(toDisplayDate(now))
                        .hasLastUpdatedDate(toDisplayDate(now))
                        .hasTrust(trustName)
                        .hasConcern("Financial compliance")
                        .hasRiskToTrust("Amber")
                        .hasRiskToTrust("Green")
                })
        });
    });

    describe("When we have many open cases", () =>
    {
        beforeEach(() =>
        {
            cy.login();

            // Ensure we have enough cases
            caseApi.getOpenCasesByOwner(Cypress.env(EnvUsername))
            .then((response) =>
            {
                const currentCases = response.paging.recordCount;
                const casesToCreate = 15 - currentCases;

                if (casesToCreate > 0)
                {
                    const cases = CaseBuilder.bulkCreateOpenCases(casesToCreate);

                    cy.wrap(cases).each((request: CreateCaseRequest, index, $list) => {
                        caseApi.post(request)
                            .then(() => {});
                    });

                    // Wait 1ms per case created
					// Each case is created one 1ms apart so that we don't break the table constraint
					// Max will be 15ms
					cy.wait(casesToCreate);

                    cy.reload();
                }
            });
        });

        it("Should display them in separate pages with 5 items per page and we should be able to move between them", () =>
        {
            let pageOneCases: Array<string> = [];
            let pageTwoCases: Array<string> = [];

            caseworkTable
                .getOpenCaseIds()
                .then((caseIds: Array<string>) =>
                {
                    pageOneCases = caseIds;

                    Logger.log("Ensure we have 5 cases on page one")
                    expect(pageOneCases.length).to.eq(5);

                    Logger.log("Moving to the second page using the direct link");
                    paginationComponent.goToPage("2");
                    return caseworkTable.getOpenCaseIds()
                })
                .then((caseIds: Array<string>) =>
                {
                    pageTwoCases = caseIds;

                    Logger.log("Ensure we have 5 cases on page 2");
                    expect(pageTwoCases.length).to.equal(5);
                    
                    Logger.log("Ensure that the cases on page one and two are different");
                    hasNoSimilarElements(pageOneCases, pageTwoCases);

                    Logger.log("Move to the previous page, which is page 1")
                    paginationComponent.previous();
                    return caseworkTable.getOpenCaseIds()
                })
                .then((caseIds: Array<string>) =>
                {
                    Logger.log("On moving to page one, we should get the exact same cases");
                    expect(caseIds).to.deep.equal(pageOneCases);

                    Logger.log("Move to the next page, which is page 2");
                    paginationComponent.next();
                    return caseworkTable.getOpenCaseIds();
                })
                .then((caseIds: Array<string>) =>
                {
                    Logger.log("On moving to page two, we should get the exact same cases");
                    expect(caseIds).to.deep.equal(pageTwoCases);

                    // We had relative instead of absolute path links so it didn't work when pagination was added
                    Logger.log("Ensure the case loads when clicking the link");
                    const caseIdToView = caseIds[0];
                    caseworkTable.getRowByCaseId(caseIdToView)
                    .then((row) =>
                    {
                        row.select();
                        caseMangementPage.getCaseIDText()
                    })
                    .then(managementCaseId =>
                    {
                        expect(caseIdToView).to.equal(managementCaseId);
                    });
                })

        });
    });

    function hasNoSimilarElements(first: Array<string>, second: Array<string>)
    {
        var firstSet = new Set(first);

        const match = second.some(e => firstSet.has(e));

        expect(match).to.be.false;
    }
})