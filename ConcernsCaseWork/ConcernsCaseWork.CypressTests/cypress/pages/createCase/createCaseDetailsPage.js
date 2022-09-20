class CreateCaseDetailsPage {

    constructor() {
        const concernsRgx = new RegExp(/(Compliance|Financial|Force majeure|Governance|Irregularity)/, 'i');
        const trustRgx = new RegExp(/(School|Academy|Accrington|South|North|East|West)/, 'i');
        const ragRgx = new RegExp(/(amber|green|red|redPlus|Red Plus)/, 'i');
        const dotRgx = new RegExp(/(Deteriorating|Unchanged|Improving)/, 'i');

    }


    //locators
    getHeading() {
        return cy.get("#ADD HEADING ELEMENT HERE");
    }

    getIssueTextBox() {
        return cy.get("#issue");
    }

    getConcernsTable() {
        return    cy.get('[id="concerns-summary-list"]');
    }


    //methods

    validateCreateCaseDetailsComponent(){
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

    }



    validateCreateCaseInitialDetails(){
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

    }

    setIssue() {

        let date = new Date();
        this.getIssueTextBox().type("Setting issue, Data entered at " + date);
    }

    setMoR() {
    let rand = Math.floor(Math.random()*1)

    cy.get('[id="means-of-referral-urn"]').eq(Math.floor(Math.random() * rand)).click();

    cy.get('button[data-prevent-double-click="true"]').click();

        if(rand == 0){
            return "12574";
        }else{
            return "12575";
        }
    }

}
    

    export default new CreateCaseDetailsPage();