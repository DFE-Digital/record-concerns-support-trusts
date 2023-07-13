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

describe("Creating a case for a city technology college", () =>
{
	const createCasePage = new CreateCasePage();
    const createConcernPage = new CreateConcernPage();
    const addDetailsPage = new AddDetailsPage();
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
            name: "Automation CTC " + now.toISOString(),
            ukprn: "CTC"+ randomIntFromInterval(0,999999999),
            companiesHouseNumber: "CTC"+ randomIntFromInterval(0,99999),
            addressline1: "1 New Road",
            addressline2: "Old Street",
            addressline3: "Somewhere",
            town: "Old Town",
            county: "Some County",
            postcode: "AB1 2CD"
        };

        Logger.Log("Creating CTC via Api");
        ctcApi.post(ctcRequest);
	});

    function randomIntFromInterval(min, max) { // min and max included 
        return Math.floor(Math.random() * (max - min + 1) + min)
    }

    it("Should create a case with only required fields", () => {
        Logger.Log("Create a case");
        createCasePage
            .createCase()
            .withTrustName(ctcRequest.name)
            .selectOption()
            .confirmOption();

        Logger.Log("Create a valid Non-concern case type");
        selectCaseTypePage
            .withCaseType("NonConcerns")
            .continue();

        Logger.Log("Populate territory");
        addTerritoryPage
            .withTerritory(territory)
            .nextStep();

        Logger.Log("Add non concerns case");
        addConcernDetailsPage
            .createCase();

        Logger.Log("Verify case details");
        caseManagementPage
            .hasTrustContain(ctcRequest.name)
            .hasTerritory(territory)
            .hasCaseOwner(name);
    });
});