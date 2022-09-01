class CaseActionsBasePage {

    constructor() {
        //this.something = 
        this.arrDate = [];
    }


    //locators
    getHeadingText() {
        return     cy.get('h1[class="govuk-heading-l"]');
    }

    getSubHeadingText() {
        return     cy.get('h2[class="govuk-heading-m"]');
    }

    getCancelBtn() {
        return     cy.get('[id="cancel-link"]', { timeout: 30000 });
    }

    getContinueBtn() {
        return     cy.get('[data-prevent-double-click="true"]', { timeout: 30000 }).contains('Add to case');
    }

    getAddCaseActionBtn() {
        return     cy.get('[id="add-srma-button"]', { timeout: 30000 });
    }

    getStatusRadio() {
        return     cy.get('[id^="status"]', { timeout: 30000 });
    }

    getStatusRadioLabel() {
        return     cy.get('label.govuk-label.govuk-radios__label', { timeout: 30000 });
    }

    //Tables

    getTableRow() {
        return    cy.get('tr.govuk-table__row', { timeout: 30000 });
    }

    getTableRowEmpty() {
        return    cy.get('tr.govuk-table__row', { timeout: 30000 }).contains('Empty');
    }

    getTableAddEditLink() {
        return    cy.get('[class="govuk-link"]', { timeout: 30000 });
    }

    getBackToCaseLink() {
        return    cy.get('[id="back-to-case-link"]', { timeout: 30000 });
    }

    //Dates
    getDateDay() {
        return     cy.get('[id="dtr-day"]', { timeout: 30000 });
    }

    getDateMonth() {
        return     cy.get('[id="dtr-month"]', { timeout: 30000 });
    }

    getDateYear() {
        return     cy.get('[id="dtr-year"]', { timeout: 30000 });
    }


    //Methods

    //sets the Case Action status
    //Takes a string value of either "0", "1", "2" or "random"
    //Automatically sets the length of the element array
    setStatusSelect(value) {

        cy.log("value is "+value)

        if(value == "random"){

            let rand

            cy.get(this.getStatusRadio()).its("length").then((elements) => {
                rand = Math.floor(Math.random() * elements);

                cy.log(this.getStatusRadio().eq(rand))
                this.getStatusRadio().eq(rand).click();

                cy.log(this.getStatusRadioLabel().eq(rand).invoke('text'));
                return this.getStatusRadioLabel().eq(rand).invoke('text');
            });

        }else{
            
            this.getStatusRadio().eq(value).click();
            cy.log(this.getStatusRadioLabel().eq(value).invoke('text'));
            return this.getStatusRadioLabel().eq(value).invoke('text');
        }
	}


    setDate() {

        cy.log("setDatePlanRequested").then(() => {

                  this.getDateDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then((day) => {

                       this.getDateMonth().type(Math.floor(Math.random() *3) + 10).invoke('val').then((month) => {

                           this.getDateYear().type("2023").invoke('val').then((year) => {

                               cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
   
                               return concat ;
                               });
                           });
                       });

                   });
           });
   }
        
}

    export default new CaseActionsBasePage();
