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

const concernsRgx = new RegExp(/(Compliance|Financial|Force majeure|Governance|Irregularity)/, 'i');
const trustRgx = new RegExp(/(School|Academy|Accrington)/, 'i');
const ragRgx = new RegExp(/(amber|green|red|redPlus|Red Plus)/, 'i');
const dotRgx = new RegExp(/(Deteriorating|Unchanged|Improving)/, 'i');

Cypress.Commands.add("login",()=> {
    cy.visit(Cypress.env('url')+"/login", { timeout: 30000 })
    cy.get('#username', { timeout: 30000 }).should('be.visible')
	cy.get("#username").type(Cypress.env('username'));
	cy.get("#password").type(Cypress.env('password')+"{enter}");
    cy.get('[id=your-casework]', { timeout: 30000 }).should('be.visible')
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

//TODO: make this more dynamic - current usability issue raised
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

Cypress.Commands.add('addActionItemToCase', () =>{

    cy.get('[class="govuk-heading-l"]').should('contain.text', 'Add to case');

    cy.get('[class="govuk-heading-m"]').should('contain.text', 'What action are you taking?');

    cy.get('[id='+option+']').click()
    cy.get('[id='+option+']').children().text.trim().should('contain.text', 'What action are you taking?');

    cy.get('[id='+option+']').children().should(($text) => {
        expect($text.text().trim()).to.contain('School Resource Management Adviser (SRMA)');

    });

});

