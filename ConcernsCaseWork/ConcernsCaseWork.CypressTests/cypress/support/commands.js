// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
import "cypress-localstorage-commands";
import LoginPage from "/cypress/pages/loginPage";
import HomePage from "/cypress/pages/homePage";
import CaseManagementPage from "/cypress/pages/caseMangementPage";
import caseMangementPage from "/cypress/pages/caseMangementPage";
import AddToCasePage from "/cypress/pages/caseActions/addToCasePage";


const concernsRgx = new RegExp(/(Compliance|Financial|Force majeure|Governance|Irregularity)/, 'i');
const trustRgx = new RegExp(/(School|Academy|Accrington|South|North|East|West)/, 'i');
const ragRgx = new RegExp(/(amber|green|red|redPlus|Red Plus)/, 'i');
const dotRgx = new RegExp(/(Deteriorating|Unchanged|Improving)/, 'i');

Cypress.Commands.add("login",()=> {
    cy.visit(Cypress.env('url')+"/login", { timeout: 40000 })
    cy.get('#username', { timeout: 40000 }).should('be.visible')
	cy.get("#username").type(Cypress.env('username'));
	cy.get("#password").type(Cypress.env('password')+"{enter}");
    cy.get('[id=your-casework]', { timeout: 40000 }).should('be.visible')
	cy.saveLocalStorage();
})

//example: /case/5880/management"
Cypress.Commands.add("visitPage",(slug)=> {
	cy.visit(Cypress.env('url')+slug, { timeout: 30000 });
	cy.saveLocalStorage();
})

Cypress.Commands.add('storeSessionData',()=>{
    Cypress.Cookies.preserveOnce('.ConcernsCasework.Login')
    let str = [];
    cy.getCookies().then((cookie) => {
        cy.log(cookie);
        for (let l = 0; l < cookie.length; l++) {
            if (cookie.length > 0 && l == 0) {
                str[l] = cookie[l].name;
                Cypress.Cookies.preserveOnce(str[l]);
            } else if (cookie.length > 1 && l > 1) {
                str[l] = cookie[l].name;
                Cypress.Cookies.preserveOnce(str[l]);
            }
        }
    });
})

Cypress.Commands.add('enterConcernDetails',()=>{
    let date = new Date();
    cy.get("#issue").type("Data entered at " + date);
    cy.get("#current-status").type("Data entered at " + date);
    cy.get("#case-aim").type("Data entered at " + date);
    cy.get("#de-escalation-point").type("Data entered at " + date);
    cy.get("#next-steps").type("Data entered at " + date);
    cy.get("#case-details-form  button").click();
})

Cypress.Commands.add('selectRiskToTrust',()=>{
    cy.get('[href="/case/rating"]').click();
    cy.get(".ragtag").should("be.visible");
    //Randomly select a RAG status
    cy.get(".govuk-radios .ragtag:nth-of-type(1)")
        .its("length")
        .then((ragtagElements) => {
            let num = Math.floor(Math.random() * ragtagElements);
            cy.get(".govuk-radios .ragtag:nth-of-type(1)").eq(num).click();
        });
    cy.get("#case-rating-form > div.govuk-button-group > button").click();
})

Cypress.Commands.add('editRiskToTrust',(cta, rag)=>{
    selectRagRating(rag);
    clickApplyOrCancel(cta);
})

function clickApplyOrCancel(cta) {
    switch (cta) {
        case "cancel":
           cy.get('[id=cancel-link-event]').click();
            break;
        case "apply":
            cy.get('button[data-prevent-double-click=true]').click();
            break;
        default:
            cy.log("Could not click item");
    }
}

function selectRagRating(ragStatus) {
    switch (ragStatus) {
        case 'Red plus':
            cy.get('[id=rating-3]').click();
            break;
        case "Amber":
            cy.get('[id=rating]').click();
            break;
        case "Green":
            cy.get('[id=rating]').click();
            break;
        case "Red":
            cy.get('[id=rating-2]').click();
            break;
        default:
            cy.log("Could not make selection");
    }
}

Cypress.Commands.add('selectConcern', (expectedNumberOfRagStatus, ragStatus) => {

    switch (ragStatus) {
        case "Red plus":
            cy.get(
                ".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__redplus"
            ).should("have.length", expectedNumberOfRagStatus);
            break;
        case "Amber":
            cy.get(
                ".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__amber"
            ).should("have.length", expectedNumberOfRagStatus);
            break;
        case "Green":
            cy.get(
                ".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__green"
            ).should("have.length", expectedNumberOfRagStatus);
            break;
        case "Red":
            cy.get(
                ".govuk-table__body .govuk-table__row .govuk-ragtag-wrapper .ragtag__red"
            ).should("have.length", expectedNumberOfRagStatus);
            break;
        default:
            cy.log("Could not do the thing");
    }
    return expectedNumberOfRagStatus;

})

//TODO: make this more dynamic
Cypress.Commands.add('selectConcernType',()=>{
    cy.get(".govuk-radios__item [value=Financial]").click();
    cy.get("[id=sub-type-3]").click();
    cy.get("[id=rating-3]").click();
    cy.get(".govuk-button").click();
})


Cypress.Commands.add('validateCreateCaseDetailsComponent',()=>{
    //Trust Address
    cy.get(".govuk-summary-list__value").then(($address) =>{
        expect($address).to.be.visible
        expect($address.text()).to.match(trustRgx)
        })
    //Concerns
    cy.get("#concerns-summary-list").then(($concerns) =>{
        expect($concerns).to.be.visible
        expect($concerns.text())
        .to.match(concernsRgx)
        })
        let rtlength = 0;
        let rowlength = 0;
    cy.get('[class="govuk-grid-row"] *[class^="govuk-tag ragtag ragtag"]')
        .then(($ragtag) =>{
        expect($ragtag).to.be.visible
        expect($ragtag).to.contains(ragRgx)
         rtlength = $ragtag.length
         rowlength = 0;
        })
        //Tests each concern has at least a RAG displayed
    cy.get('*[class^="govuk-!-padding-bottom-1"]').then($elements => {
         rowlength = $elements.length
         cy.wrap(rtlength).should('be.gte', rowlength);      
         })
      //Risk to trust
    cy.get(".govuk-summary-list__row").contains("Risk")
         .then(($trustr) =>{
         expect($trustr).to.be.visible
         expect($trustr.text()).to.match(/(Risk to trust)/i);
         })
    cy.get('div:nth-child(3) > dd > div > span') //<<Horrible locators needs replacing
         .should('be.visible')
         .should('have.attr', 'class')
         .and('match', /ragtag/i)
         .and('match', /(amber|green|red|redPlus)/i); //Asserts class name range
    cy.get('div:nth-child(3) > dd > div > span').then(($conragtag) =>{
         expect($conragtag.text()).to.match(ragRgx); //Asserts text range on page
    });

})

Cypress.Commands.add('validateCreateCaseInitialDetails',()=>{
        const lstring =
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwx';

        //Issue validation
        cy.get('[class="govuk-grid-row"] *[for="issue"]').should(($issue) => {
            expect($issue.text().trim()).equal("Issue");
          })
        cy.get('#issue').should('be.visible');
        cy.get("#issue-info").then(($issinf1) =>{
            expect($issinf1).to.be.visible
            expect($issinf1.text()).to.match(/(2000 characters remaining)/i)   

            let text = Cypress._.repeat(lstring, 40)
            expect(text).to.have.length(2000);
            
        cy.get('#issue').invoke('val', text);
        cy.get('#issue').type('{shift}{alt}'+ '1');
        cy.get("#issue-info").then(($issinf2) =>{
            expect($issinf2).to.be.visible
            expect($issinf2.text()).to.match(/(1 character too many)/i);      
            })
        })

        //Current Status validation
        cy.get('[class="govuk-grid-row"] *[for="current-status"]').should(($stat) => {
            expect($stat.text().trim()).equal("Current status (optional)")
            });
        cy.get("#current-status").should('be.visible');
        cy.get('#current-status-info').then(($statinf) =>{
            expect($statinf).to.be.visible
            expect($statinf.text()).to.match(/(4000 characters)/i)
        })

        //Case aim validation
        cy.get('[class="govuk-grid-row"] *[for="case-aim"]').should(($case) => {
             expect($case.text().trim()).equal("Case aim (optional)")
        });
        cy.get('#case-aim').should('be.visible');
        cy.get('#case-aim-info').then(($caseinf) =>{
            expect($caseinf).to.be.visible
            expect($caseinf.text()).to.match(/(1000 characters)/i);
     })
        //De-escalation validation
        cy.get('[class="govuk-grid-row"] *[for="de-escalation-point"]').should(($desc) => {
            expect($desc.text().trim()).equal("De-escalation point (optional)")
        })  
        cy.get('#de-escalation-point').should('be.visible');
        cy.get('#de-escalation-point-info').then(($descinf) =>{
            expect($descinf).to.be.visible
            expect($descinf.text()).to.match(/(1000 characters)/i);
        });
        //Next steps validation
        cy.get('[class="govuk-grid-row"] *[for="next-steps"]').should(($nxt) => {
            expect($nxt.text().trim()).equal("Next steps (optional)")
        });
        cy.get('#next-steps').should('be.visible');
        cy.get('#next-steps-info').then(($nxtinf1) =>{
            expect($nxtinf1).to.be.visible
            expect($nxtinf1.text()).to.match(/(4000 characters)/i)
        })

        cy.get('button[data-prevent-double-click^="true"]').then(($btncreate) =>{
             expect($btncreate.text()).to.match(/(Create case)/i);
        })
        cy.get('a[data-prevent-double-click^="true"]').then(($btncreate) =>{
            expect($btncreate.text()).to.match(/(Cancel)/i);
        })
        cy.get("#case-details-form  button").click();
        cy.get('#error-summary-title').then(($error) =>{
            expect($error).to.be.visible
            expect($error.text()).to.match(/(There is a problem)/i);
        })
    })

//TODO:
//Break these out into 2 or 3 separate commands per component (not page)
Cypress.Commands.add('validateCaseManagPage',()=>{

        cy.get('*[class^="govuk-button govuk-button--secondary"]')
            .should(($backcasw) => {
            expect($backcasw).to.be.visible
            expect($backcasw.text().trim()).equal("Back to casework")
        })

        cy.get('[id="close-case-button"]').should(($clscse) => {
            expect($clscse).to.be.visible
            expect($clscse.text().trim()).equal("Close case")
        })

        cy.get('[name="caseID"]').children().should(($casid) => {
            expect($casid).to.be.visible
            expect($casid.text().trim()).equal("Case ID")
            expect($casid.text()).to.match(/(Case ID)/gi);

        })
        cy.get('[name="caseID"]').then(($test) =>{
            expect($test.text()).to.match(/(\n(\d*))/gi)//matches any case number on a new line  
    })

        cy.get('[class="govuk-heading-m"]').should(($tradd) => {
            expect($tradd).to.be.visible
            expect($tradd.text()).to.match(trustRgx);
        })

        cy.get('[id="tab_case-details"]').should(($tabcd) => {
            expect($tabcd).to.be.visible
            expect($tabcd.text().trim()).equal("Case Details")
        })

        cy.get('[id="tab_trust-overview"]').should(($tabto) => {
            expect($tabto).to.be.visible
            expect($tabto.text().trim()).equal("Trust Overview")
        })
        
        cy.get('[class="govuk-table-case-details__header"]').contains("Trust")
            .should(($tru) => {
            expect($tru.text().trim()).equal("Trust")
        })

        cy.get('tr[class=govuk-table__row]').should(($row) => {
            expect($row).to.have.lengthOf.at.least(4);
            expect($row.eq(0).text().trim()).to.contain('Trust').and.to.match(trustRgx);
            expect($row.eq(1).text().trim()).to.contain('Risk to trust').and.to.match(ragRgx).and.to.match(/(Edit risk rating)/);
            expect($row.eq(2).text().trim()).to.contain('Direction of travel').and.to.match(dotRgx).and.to.match(/(Edit direction of travel)/);
            expect($row.eq(3).text().trim()).to.contain('Concerns').and.to.match(concernsRgx).and.to.match(/(Add concern)/).and.to.match(/(Edit)/); 
        })

        cy.get('[class^="govuk-accordion__section govuk-accordion"]').should(($accord) => {
            expect($accord).to.have.length(5);
            expect($accord.eq(0).text().trim()).to.contain('Issue').and.to.match(/Edit/);
            expect($accord.eq(1).text().trim()).to.contain('Current status').and.to.match(/Edit/);
            expect($accord.eq(2).text().trim()).to.contain('Case aim').and.to.match(/(Edit)/);
            expect($accord.eq(3).text().trim()).to.contain('De-escalation point').and.to.match(/(Edit)/);
            expect($accord.eq(4).text().trim()).to.contain('Next steps').and.to.match(/(Edit)/);
        })
})

Cypress.Commands.add('closeAllOpenConcerns',()=>{
    const elem = '.govuk-table-case-details__cell_no_border [href*="edit_rating"]';
if (Cypress.$(elem).length > 0) { //Cypress.$ needed to handle element missing exception

    cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]').its('length').then(($elLen) => {
        cy.log($elLen)

    while ($elLen > 0) {
            cy.get('.govuk-table-case-details__cell_no_border [href*="edit_rating"]').eq($elLen-1).click();
            cy.get('[href*="closure"]').click();
            cy.get('.govuk-button-group [href*="edit_rating/closure"]:nth-of-type(1)').click();
            $elLen = $elLen-1
            cy.log($elLen+" more open concerns")				
            }
        });
}else {
    cy.log('All concerns closed')
    }

})

Cypress.Commands.add('randomSelectTrust', () =>{
    let searchTerm =
      ["10058372", "10060045", "10060278", "10058372", "10059732", "10060186",
        "10080822", "10081341", "10058833", "10058354", 
        "10066108", "10058598", "10059919", "10057355", "10058295", "10059877",
        "10060927", "10059550", "10058417", "10059171", "10060716", "10060832",
        "10066116", "10058998", "10058772", "10059020", "10058154", "10059577",
        "10059981", "10058198", "10060069", "10059834", "10064323", "10060619",
        "10058893", "10058873", "10060447", "10057945", "10058340", "10058890",
        "10059880", "10060445", "10058715", "10059448", "10060131", "10058725",
        "10058630", "10060260", "10058560", "10058776", "10059501", "10058240",
        "10059063", "10059055", "10060233", "10058723", "10059998", "10058813",
        "10059324", "10058181", "10061208", "10060877", "10058468", "10064307"]
                        
        let num = Math.floor(Math.random()*searchTerm.length); 
        console.log(searchTerm[num]);
        cy.get("#search").type(searchTerm[num] + "{enter}");

        return cy.wrap(searchTerm[num]).as('term');


});

Cypress.Commands.add('checkForExistingCase', function (forceCreate){
        let caseExists = false
        const elem = '.govuk-link[href^="case"]';

        cy.log((elem).length )
            if (Cypress.$(elem).length > 1 ) { //Cypress.$ needed to handle element missing exception
                caseExists = true
                    if (forceCreate == true) {
                        cy.log('Force Create set, start case creation')
                        cy.createCase();
                    }else{
                        cy.get('.govuk-link[href^="case"]').eq(0).click();
                    }

            }else {
                cy.log('No cases present, start case creation')
                cy.createCase();
            }

            //return cy.wrap(caseExists).as('caseEx'); //commented out due to current limitation with cypress)

});

//description: creates a new case from the case list (home) page
//Location used: https://amsd-casework-dev.london.cloudapps.digital/
//parameters: takes no arguments
Cypress.Commands.add('createCase', () =>{

                cy.get('[href="/case"]').click();
                cy.get("#search").should("be.visible");

                cy.randomSelectTrust();
                cy.get("#search__option--0").click();

               cy.get(".govuk-summary-list__value").then(($el) =>{
                    expect($el.text()).to.match(/(school|england|academy|trust|West|East|North|South)/i)
                });
                cy.selectConcernType();

                cy.selectRiskToTrust();

                cy.enterConcernDetails();
});

//description: selects an action item from the add to case page
//Location used: case/6325/management/action
//parameters: takes a string args from the value attribute eg: value="srma"
Cypress.Commands.add('addActionItemToCase', (option, textToVerify) =>{
    
    //cy.get('[class="govuk-heading-l"]').should('contain.text', 'Add to case');
    AddToCasePage.getHeadingText().should('contain.text', 'Add to case');
 
    //cy.get('h2[class="govuk-heading-m"]').should('contain.text', 'What action are you taking?');
    AddToCasePage.getSubHeadingText().should('contain.text', 'What action are you taking?');


    //cy.get('[value="'+option+'"]').click()
    AddToCasePage.getCaseActionRadio(option).click();

    //cy.get('[value="'+option+'"]').siblings().should('contain.text', textToVerify);
    AddToCasePage.getCaseActionRadio(option).siblings().should('contain.text', textToVerify);

    cy.get('[value="'+option+'"]').siblings().should(($text) => {
        expect($text.text().trim()).equal(textToVerify)
    });

});

Cypress.Commands.add('closeSRMA', function (){
    let SRMAExists = false

    const elem = 'table:nth-child(4) > tbody > tr > td:nth-child(1)'

    cy.log((elem).length )
        if (Cypress.$(elem).length > 0 ) { //Cypress.$ needed to handle element missing exception

            cy.get('table:nth-child(4) > tbody > tr > td:nth-child(1) > a').eq(0).scrollIntoView();
            cy.get('table:nth-child(4) > tbody > tr > td:nth-child(1) > a').eq(0).click();
            cy.wait(500)

            // Sets status to Trust Considering
            cy.get('[class="govuk-link"]').eq(0).click();
            cy.get('[id*="status"]').eq(0).click();
            cy.get('[id="add-srma-button"]').click();

            cy.get('[class="govuk-link"]').eq(2).click();
    
            let rand = Math.floor(Math.random()*2)
            cy.get('[id^="reason"]').eq(rand).click();
            cy.get('[id="add-srma-button"]').click();    
            cy.get('[id="complete-decline-srma-button"]').click();
            cy.get('[id="confirmChk"]').click();
            cy.get('[id="add-srma-button"]').click();


        }else {
            cy.log('No SRMA present');

        }
});

/*
Cypress.Commands.add('closeSRMA', function (){
    let SRMAExists = false

    const elem = 'table:nth-child(4) > tbody > tr > td:nth-child(1)'

    cy.log((elem).length )
        if (Cypress.$(elem).length > 0 ) { //Cypress.$ needed to handle element missing exception

            cy.get('table:nth-child(4) > tbody > tr > td:nth-child(1) > a').eq(0).scrollIntoView();
            cy.get('table:nth-child(4) > tbody > tr > td:nth-child(1) > a').eq(0).click();
            cy.wait(500)

            // Sets status to Trust Considering
            cy.get('[class="govuk-link"]').eq(0).click();
            cy.get('[id*="status"]').eq(0).click();
            cy.get('[id="add-srma-button"]').click();

            cy.get('[class="govuk-link"]').eq(2).click();
    
            let rand = Math.floor(Math.random()*2)
            cy.get('[id^="reason"]').eq(rand).click();
            cy.get('[id="add-srma-button"]').click();
            cy.get('[id="complete-decline-srma-button"]').click();
            cy.get('[id="confirmChk"]').click();
            cy.get('[id="add-srma-button"]').click();


        }else {
            cy.log('No SRMA present')

        }

*/
     Cypress.Commands.add('createSRMA', function (){

        cy.get('[class="govuk-button"][role="button"]').click();
		cy.addActionItemToCase('Srma', 'School Resource Management Adviser (SRMA)');
		cy.get('button[data-prevent-double-click*="true"]').click();

		//User sets SRMA status 
		cy.get('[id*="status"]').eq(0).click();
		cy.get('label.govuk-label.govuk-radios__label').eq(0);

		cy.get('[id="dtr-day"]').type(Math.floor(Math.random() * 21) + 10);
		cy.get('[id="dtr-month"]').type(Math.floor(Math.random() *3) + 10);
		cy.get('[id="dtr-year"]').type("2022");
		cy.get('[id="add-srma-button"]').click();

    });

//});