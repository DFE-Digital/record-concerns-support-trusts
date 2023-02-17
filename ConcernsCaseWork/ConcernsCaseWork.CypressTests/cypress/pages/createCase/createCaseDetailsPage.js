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
        cy.validateCreateCaseInitialDetails();
    }

    setIssue() {

        let date = new Date();
        this.getIssueTextBox().type("Setting issue, Data entered at " + date);
    }

    setMoR() {
    let rand = Math.floor(Math.random()*1)

    cy.get('[id="means-of-referral-id"]').eq(Math.floor(Math.random() * rand)).click();

    cy.get('button[data-prevent-double-click="true"]').click();

        if(rand == 0){
            return "12574";
        }else{
            return "12575";
        }
    }

}
    

    export default new CreateCaseDetailsPage();