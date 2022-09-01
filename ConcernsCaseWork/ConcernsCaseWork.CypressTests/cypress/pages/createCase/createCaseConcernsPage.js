class CreateCaseConcernsPage {

    constructor() {
        const concernsRgx = new RegExp(/(Compliance|Financial|Force majeure|Governance|Irregularity)/, 'i');
        const trustRgx = new RegExp(/(School|Academy|Accrington|South|North|East|West)/, 'i');
        const ragRgx = new RegExp(/(amber|green|red|redPlus|Red Plus)/, 'i');
        const dotRgx = new RegExp(/(Deteriorating|Unchanged|Improving)/, 'i');

        this.num = "null";
    }

    //locators
    getHeading() {
        return cy.get("#ADD HEADING ELEMENT HERE");
    }

    getAddConcernBtn() {
        return 	cy.get(".govuk-button.govuk-button--secondary");
    }

    getMorRadio() {
        return 	cy.get('[id="means-of-referral-urn"]');
    }

    getAddConcernBtn() {
        return 	cy.get('button[data-prevent-double-click="true"]');
    }

    getMorRadio() {
        return 	cy.get('[id="means-of-referral-urn"]');
    }
    
    getConcernRadio() {
        return 	cy.get(".govuk-radios__item [value=Financial]");
    }

    getConcernSubRadio() {
        return 	cy.get('[id=sub-type-3]');
    }

    getConcernRatingRadio() {
        return 	cy.get("[id^=rating-]");
    }

    getAddAnotherConcernBtn() {
        return 	cy.get('[href="/case/concern"]');
    }

    getNextStepBtn() {
        return 	cy.get('a[href="/case/rating"]');
    }


    
    //methods

    selectConcern(expectedNumberOfRagStatus, ragStatus){

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
    }

    selectConcernType2(){
        //cy.get(".govuk-radios__item [value=Financial]").click();
        this.getConcernRadio().click();
        
        //cy.get("[id=sub-type-3]").click();
        this.getConcernSubRadio("3").click();

        //cy.get("[id=rating-3]").click();
        this.getConcernRatingRadio().eq(0).click();

        cy.get(".govuk-button").click();
        
    }

    selectConcernType(){
       // cy.get(".govuk-radios__item [value=Financial]").click();
       this.getConcernRadio().click();

        //cy.get("[id=sub-type-3]").click();
        this.getConcernSubRadio().click();

        //cy.get("[id=rating-3]").click();
        this.getConcernRatingRadio().eq(0).click();

        this.selectMoR();
        //cy.get(".govuk-button").click();


    }

    selectMoR(){

            let rand = Math.floor(Math.random()*1)

            this.getMorRadio().eq(Math.floor(Math.random() * 1)).click();

            cy.get('button[data-prevent-double-click="true"]').click();

    if(rand == 0){
                return "12574";
            }else{
                return "12575";
            }

    }
}
    

    export default new CreateCaseConcernsPage();