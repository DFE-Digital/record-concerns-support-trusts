class FinancialPlanPage {

    constructor() {
        //this.something = 
        this.arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];

        //this.statusSelect2 = () => {
            //let rand = Math.floor(Math.random()*2)
            //this.getStatusRadio().eq(rand).click();
            //cy.log(this.getStatusRadioLabel().eq(rand).invoke('text'));
            //return this.getStatusRadioLabel().eq(rand).invoke('text');
       // }
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

    getAddToCaseBtn() {
        return     cy.get('[data-prevent-double-click="true"]', { timeout: 30000 }).contains('Add to case');
    }


    //Current Status
    getStatusRadio() {
        return     cy.get('[id*="status"]', { timeout: 30000 });
    }

    getStatusRadioLabel() {
        return     cy.get('label.govuk-label.govuk-radios__label', { timeout: 30000 });
    }

    //Date plan requested
    getDatePlanRequestedDay() {
        return     cy.get('[id="dtr-day-plan-requested"]', { timeout: 30000 });
    }

    getDatePlanRequestedMon() {
        return     cy.get('[id="dtr-month-plan-requested"]', { timeout: 30000 });
    }

    getDatePlanRequestedYear() {
        return     cy.get('[id="dtr-year-plan-requested"]', { timeout: 30000 });
    }



    //Date viable plan received
    getDatePlanReceivedDay() {
        return     cy.get('[id="dtr-day-viable-plan"]', { timeout: 30000 });
    }

    getDatePlanReceivedMon() {
        return     cy.get('[id="dtr-month-viable-plan"]', { timeout: 30000 });
    }

    getDatePlanReceivedYear() {
        return     cy.get('[id="dtr-year-viable-plan"]', { timeout: 30000 });
    }

    getNotesTextBox() {
        return     cy.get('[id="financial-plan-notes"]', { timeout: 30000 });
    }

    getUpdateBtn() {
        return    cy.get('[id="add-srma-button"]', { timeout: 30000 });
    }
    

    //Option accepts the following args: DfESupport | FinancialForecast | FinancialPlan | FinancialReturns |
    //FinancialSupport| ForcedTermination | Nti| RecoveryPlan | Srma | Tff |
    getCaseActionRadio(option) {
        return     cy.get('[value="'+option+'"]');
    }    


    //Methods

<<<<<<< Updated upstream
=======

    //sets the Case Action status
    //Takes a string value of either "0", "1", "2" or "random"
    setStatusSelect(value) {
        //let random = false
        cy.log("value "+value)

        if(value == "random"){
            let rand = Math.floor(Math.random()*2)

            this.getStatusRadio().eq(rand).click();
            cy.log(this.getStatusRadioLabel().eq(rand).invoke('text'));
            return this.getStatusRadioLabel().eq(rand).invoke('text');

        }else{
            
            this.getStatusRadio().eq(value).click();
            cy.log(this.getStatusRadioLabel().eq(value).invoke('text'));
            return this.getStatusRadioLabel().eq(value).invoke('text');

        }
	}

>>>>>>> Stashed changes
    statusSelect() {
		let rand = Math.floor(Math.random()*2)

        this.getStatusRadio().eq(rand).click();
        cy.log(this.getStatusRadioLabel().eq(rand).invoke('text'));
        return this.getStatusRadioLabel().eq(rand).invoke('text');
	}
        
}

    export default new FinancialPlanPage();


    /*

        //cy.log(this.getStatusRadioLabel().eq(rand).invoke('text') );
        //return this.getStatusRadioLabel().eq(rand).invoke('text') ;


       // this.getStatusRadioLabel().eq(rand).invoke('text').then(term => {
        //cy.get('label.govuk-label.govuk-radios__label').eq(rand).invoke('text').then(term => {
		//return	cy.wrap(term.trim()).as('stText');
			//cy.log(self.stText);

            //return cy.wrap(term.trim());
            cy.log("Method Inside "+self.stText);
            cy.log("Method Inside "+this.stText);

            cy.log("Method Inside "+self.term);
            cy.log("Method Inside "+this.term);

		//})
        
        cy.log("Method "+self.stText);
        cy.log("Method "+this.stText);
        cy.log("Method "+this.term);
        cy.log("Method "+self.term);
    */