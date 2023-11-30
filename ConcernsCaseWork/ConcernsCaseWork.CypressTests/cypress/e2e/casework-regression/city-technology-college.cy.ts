import { Logger } from "cypress/common/logger";
import { CreateCasePage } from "cypress/pages/createCase/createCasePage";

import CreateConcernPage from "cypress/pages/createCase/createConcernPage";
import AddDetailsPage from "cypress/pages/createCase/addDetailsPage";
import AddTerritoryPage from "cypress/pages/createCase/addTerritoryPage";
import AddConcernDetailsPage from "cypress/pages/createCase/addConcernDetailsPage";
import selectCaseTypePage from "cypress/pages/createCase/selectCaseTypePage";
import caseManagementPage from "cypress/pages/caseMangementPage";
import { EnvUsername } from "cypress/constants/cypressConstants";
import ctcApi from "../../api/cityTechnologyCollegeApi";
import { CreateCityTechnologyCollegeRequest } from "cypress/api/apiDomain";
import selectCaseDivisionPage from "cypress/pages/createCase/selectCaseDivisionPage";

describe("Creating a case for a city technology college", () =>
{
	const createCasePage = new CreateCasePage();
    const addTerritoryPage = new AddTerritoryPage();
    const addConcernDetailsPage = new AddConcernDetailsPage();

    const territory = "North and UTC - North East";
    let email: string;
    let name: string;
    let now: Date;

    let ctcRequest : CreateCityTechnologyCollegeRequest 

	beforeEach(() => {
		cy.login();
        now = new Date();
        email = Cypress.env(EnvUsername);
        var x = email.split("@")[0];
        if(x.includes('.'))
        {
            name = x.replace('.',' ');
        } else {
            name = x;
        }

        ctcRequest = {
            name: "Automation CTC",
            ukprn: "CTC987654321",
            companiesHouseNumber: "CTC12345",
            addressline1: "1 New Road",
            addressline2: "Old Street",
            addressline3: "Somewhere",
            town: "Old Town",
            county: "Some County",
            postcode: "AB1 2CD"
        };

        Logger.log("Creating CTC via Api");
        ctcApi.get(ctcRequest.ukprn)
            .then(response => {

                if(response.ukprn == null)
                {
                    ctcApi.post(ctcRequest);
                }
            });
	});

    it("Should create a case with only required fields", () => {
        Logger.log("Create a case");
        createCasePage
            .createCase()
            .withTrustName(ctcRequest.name)
            .selectOption()
            .confirmOption();

        Logger.log("Create a valid case division");
        selectCaseDivisionPage
            .withCaseDivision("SFSO")
            .continue();

        Logger.log("Populate territory");
        addTerritoryPage
            .withTerritory(territory)
            .nextStep();

        Logger.log("Create a valid Non-concern case type");
        selectCaseTypePage
            .withCaseType("NonConcerns")
            .continue();

        Logger.log("Add non concerns case");
        addConcernDetailsPage
            .createCase();

        Logger.log("Verify case details");
        caseManagementPage
            .hasTrustContain(ctcRequest.name)
            .hasManagedBy("SFSO", territory)
            .hasCaseOwner(name);
    });
});