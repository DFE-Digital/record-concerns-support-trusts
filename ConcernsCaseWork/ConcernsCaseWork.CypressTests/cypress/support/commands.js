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

Cypress.Commands.add("login",()=> {
	cy.visit(Cypress.env('url')+"/login");
	cy.get("#username").type(Cypress.env('username'));
	cy.get("#password").type(Cypress.env('password')+"{enter}");
    cy.get('[id=your-casework]').should('be.visible')
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
    cy.get('[href="/case/rating"').click();
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

//TODO: make this more dynamic - current usability issue raised
//under 83452
Cypress.Commands.add('selectConcernType',()=>{
    cy.get(".govuk-radios__item [value=Financial]").click();
    cy.get("[id=sub-type-3]").click();
    cy.get("[id=rating-3]").click();
    cy.get(".govuk-button").click();
})
