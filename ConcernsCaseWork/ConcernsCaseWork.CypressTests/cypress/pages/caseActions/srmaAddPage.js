import caseActionsBase from "/cypress/pages/caseActions/caseActionsBasePage";

class SRMAAddPage {

    constructor() {
        //this.something = 
        this.arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];
    }

    

    //
    //ADD SRMA ELEMENTS


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


    //Status
    getStatusRadio() {
        return     cy.get('[id*="status"]', { timeout: 30000 });
    }

    getStatusRadioLabel() {
        return     cy.get('label.govuk-label.govuk-radios__label', { timeout: 30000 });
    }

    //Date Offered
    getDateOfferedDay() {
        return     cy.get('[id="dtr-day"]', { timeout: 30000 });
    }

    getDateOfferedMonth() {
        return     cy.get('[id="dtr-month"]', { timeout: 30000 });
    }

    getDateOfferedYear() {
        return     cy.get('[id="dtr-year"]', { timeout: 30000 });
    }

    getNotesTextBox() {
        return     cy.get('[id="financial-plan-notes"]', { timeout: 30000 });
    }

    getReasonRadio() {
        return     cy.get('[id^="reason"]', { timeout: 30000 });
    }

    getReasonRadioLabel() {
        return      this.getStatusRadioLabel();
    }

    //Date Offered
    getDateAcceptedDay() {
        return     cy.get('[id="dtr-day"]', { timeout: 30000 });
    }

    getDateAcceptedMonth() {
        return     cy.get('[id="dtr-month"]', { timeout: 30000 });
    }

    getDateAcceptedYear() {
        return     cy.get('[id="dtr-year"]', { timeout: 30000 });
    }

    getDateVisitStartDay() {
        return     cy.get('[id="start-dtr-day"]', { timeout: 30000 });
    }

    getDateVisitStartMonth() {
        return     cy.get('[id="start-dtr-month"]', { timeout: 30000 });
    }
    
    getDateVisitStartYear() {
        return     cy.get('[id="start-dtr-year"]', { timeout: 30000 });
    }

    
    getDateVisitEndDay() {
        return     cy.get('[id="end-dtr-day"]', { timeout: 30000 });
    }

    getDateVisitEndMonth() {
        return     cy.get('[id="end-dtr-month"]', { timeout: 30000 });
    }
    
    getDateVisitEndYear() {
        return     cy.get('[id="end-dtr-year"]', { timeout: 30000 });
    }




    //Option accepts the following args: DfESupport | FinancialForecast | FinancialPlan | FinancialReturns |
    //FinancialSupport| ForcedTermination | Nti| RecoveryPlan | Srma | Tff |
    getCaseActionRadio(option) {
        return     cy.get('[value="'+option+'"]');
    }   
    
    getNotesBox() {
        return    cy.get('[id="srma-notes"]', { timeout: 30000 });
    }
    
    getNotesInfo() {
        return    cy.get('[id="srma-notes-info"]', { timeout: 30000 });
    }

    getAddCaseActionBtn() {
        return caseActionsBase.getAddCaseActionBtn();
    }

    getDateReportSentDay() {
        return this.getDateAcceptedDay();
    }

    getDateReportSentMonth() {
        return this.getDateAcceptedMonth();
    }

    getDateReportSentYear() {
        return this.getDateAcceptedYear();
    }

    //Methods

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

    statusSelect() {

		let rand = Math.floor(Math.random()*2)

        this.getStatusRadio().eq(rand).click();
        cy.log(this.getStatusRadioLabel().eq(rand).invoke('text'));
        return this.getStatusRadioLabel().eq(rand).invoke('text');
	}

    
    setDateOffered() {
        cy.log("getDateOffered").then(() => {
                  this.getDateOfferedDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then((day) => {
                       this.getDateOfferedMonth().type(Math.floor(Math.random() *3) + 10).invoke('val').then((month) => {
                           this.getDateOfferedYear().type("2023").invoke('val').then((year) => {
                               cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                               return concat ;
                               });
                           });
                       });
                    });
            });
    }

    // Gets the element without setting vlues, useful fir instances where setting is not requiured
   getDateOffered() {
    cy.log("getDateOffered").then(() => {
              this.getDateOfferedDay().invoke('val').then((day) => {
                   this.getDateOfferedMonth().invoke('val').then((month) => {
                       this.getDateOfferedYear().invoke('val').then((year) => {
                           cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                           return concat ;
                           });
                       });
                   });

               });
        });
    }

    setReason(value) {

        cy.log("value "+value)

        if(value == "random"){
            let rand = Math.floor(Math.random()*2)

            this.getReasonRadio().eq(rand).click();
            cy.log(this.getReasonRadioLabel().eq(rand).invoke('text'));
            return this.getReasonRadioLabel().eq(rand).invoke('text');

        }else{ 
            this.getReasonRadio().eq(value).click();
            cy.log(this.getReasonRadioLabel().eq(value).invoke('text'));
            return this.getReasonRadioLabel().eq(value).invoke('text');

        }

	}

   setDateAccepted() {
    cy.log("getDateAccepted ").then(() => {
              this.getDateAcceptedDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then((day) => {
                   this.getDateAcceptedMonth().type(Math.floor(Math.random() *3) + 10).invoke('val').then((month) => {
                       this.getDateAcceptedYear().type("20"+Math.floor(Math.random() *3) + 20).invoke('val').then((year) => {
                           cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                           return concat ;
                           });
                       });
                   });

               });
        });
    }

    // Gets the element without setting vlues, useful fir instances where setting is not requiured
   getDateAccepted() {
    cy.log("getDateAccepted ").then(() => {
              this.getDateAcceptedDay().invoke('val').then((day) => {
                   this.getDateAcceptedMonth().invoke('val').then((month) => {
                       this.getDateAcceptedYear().invoke('val').then((year) => {
                           cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                           return concat ;
                           });
                       });
                   });

               });
        });
    }

    //takes a string param to set each date
    //sets the Case Action date of visit
    //Takes a string value of the date "0", "1", "2" or "random"
    setDateVisitStart() {

            cy.log("setDateVisitStart Random ").then(() => {
                this.getDateVisitStartDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then((day) => {
                     this.getDateVisitStartMonth().type(Math.floor(Math.random() *3) + 10).invoke('val').then((month) => {
                         this.getDateVisitStartYear().type("2022").invoke('val').then((year) => {
                             cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                             return concat ;
                             });
                         });
                     });
                   });
          });

            
 
    }

    setDateVisitEnd() {
        cy.log("getDateAccepted ").then(() => {
                    this.getDateVisitEndDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then((day) => {
                        this.getDateVisitEndMonth().type(Math.floor(Math.random() *3) + 10).invoke('val').then((month) => {
                            this.getDateVisitEndYear().type("2025").invoke('val').then((year) => {
                                cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                                return concat ;
                                });
                            });
                        });
    
                    });
            });
    }

    setDateReportSent() {
        cy.log("getDateAccepted ").then(() => {
                    this.getDateReportSentDay().type(Math.floor(Math.random() * 21) + 10).invoke('val').then((day) => {
                        this.getDateReportSentMonth().type(Math.floor(Math.random() *3) + 10).invoke('val').then((month) => {
                            this.getDateReportSentYear().type("2025").invoke('val').then((year) => {
                                cy.wrap(day+"-"+month+"-"+year).as("concat").then((concat) => {
                                return concat ;
                                });
                            });
                        });
    
                    });
            });
    }

    setNotes() {
        let date = new Date();

        this.getNotesBox().clear();
        this.getNotesBox().type('Cypress test run '+date).invoke('val').then((value) => {
            return value ;
            });
    }
        
}

    export default new SRMAAddPage();
