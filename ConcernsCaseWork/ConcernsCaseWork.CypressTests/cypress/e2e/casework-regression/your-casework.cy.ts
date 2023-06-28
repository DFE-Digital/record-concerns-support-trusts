import caseworkTable from "../../pages/caseRows/caseworkTable";
import { toDisplayDate } from "../../support/formatDate";
import CaseManagementPage from "../../pages/caseMangementPage";
import caseApi from "cypress/api/caseApi";
import { EnvUsername } from "cypress/constants/cypressConstants";
import { CreateCaseRequest } from "cypress/api/apiDomain";
import { CaseBuilder } from "cypress/api/caseBuilder";
import { Logger } from "cypress/common/logger";
import paginationComponent from "cypress/pages/paginationComponent";

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
            .then((id: number) => {
                caseId = id + "";
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
                        .hasTrust(trustName)
                        .hasConcern("Governance and compliance: Compliance")
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

                const cases = new Array<CreateCaseRequest>();

                if (casesToCreate > 0)
                {
                    const interator = Array.from({ length: casesToCreate });

                    cy.wrap(interator).each(($el, index, $list) => {
                        cy.wait(10);

                        const request = CaseBuilder.buildOpenCase();

                        caseApi.post(request)
                            .then(() => {})
                    });

                    cy.reload();
                }
            });
        });

        it("Should display them in separate pages with 5 items per page and we should be able to move between them", () =>
        {
            let pageOneCases: Array<string> = [];
            let pageTwoCases: Array<string> = [];

            caseworkTable
                .getCaseIds()
                .then((caseIds: Array<string>) =>
                {
                    pageOneCases = caseIds;

                    Logger.Log("Ensure we have 5 cases on page one")
                    expect(pageOneCases.length).to.eq(5);

                    Logger.Log("Moving to the second page using the direct link");
                    paginationComponent.goToPage("2");
                    return caseworkTable.getCaseIds()
                })
                .then((caseIds: Array<string>) =>
                {
                    pageTwoCases = caseIds;

                    Logger.Log("Ensure we have 5 cases on page 2");
                    expect(pageTwoCases.length).to.equal(5);
                    
                    Logger.Log("Ensure that the cases on page one and two are different");
                    hasNoSimilarElements(pageOneCases, pageTwoCases);

                    Logger.Log("Move to the previous page, which is page 1")
                    paginationComponent.previous();
                    return caseworkTable.getCaseIds()
                })
                .then((caseIds: Array<string>) =>
                {
                    Logger.Log("On moving to page one, we should get the exact same cases");
                    expect(caseIds).to.deep.equal(pageOneCases);

                    Logger.Log("Move to the next page, which is page 2");
                    paginationComponent.next();
                    return caseworkTable.getCaseIds();
                })
                .then((caseIds: Array<string>) =>
                {
                    Logger.Log("On moving to page two, we should get the exact same cases");
                    expect(caseIds).to.deep.equal(pageTwoCases);
                });
        });
    });

    function hasNoSimilarElements(first: Array<string>, second: Array<string>)
    {
        var firstSet = new Set(first);

        const match = second.some(e => firstSet.has(e));

        expect(match).to.be.false;
    }
})