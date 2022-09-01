class CreateCaseTrustRiskPage {

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

    getConfirmCaseRatingButton() {
        return cy.get("#case-rating-form > div.govuk-button-group > button");
    }

    getStatusRadio() {
        return cy.get('[id="means-of-referral-urn"]');
    }

    getStatusRadioLabel() {
        return cy.get('.govuk-hint.govuk-radios__hint');
    }
    
    //methods


    selectTrustRisk(){
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

    }

    setMeansOfReferral(value) {
        //let random = false
        cy.log("value "+value)

        if(value == "random"){
            let rand = Math.floor(Math.random()*1)

            this.getStatusRadio().eq(rand).click();
            cy.log(this.getStatusRadioLabel().eq(rand).invoke('text'));
            return this.getStatusRadioLabel().eq(rand).invoke('text');

        }else{
            
            this.getStatusRadio().eq(value).click();
            cy.log(this.getStatusRadioLabel().eq(value).invoke('text'));
            return this.getStatusRadioLabel().eq(value).invoke('text');
        }
    }


}
    

    export default new CreateCaseTrustRiskPage();