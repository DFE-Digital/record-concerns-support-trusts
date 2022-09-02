import caseActionsBase from "/cypress/pages/caseActions/caseActionsBasePage";

class NTIAddPage {

    constructor() {

        this.arrDate = ["day1", "month1", "year1","day2", "month2", "year2", ];
    }

    getHeadingText() {
        return     caseActionsBase.getHeadingText();
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

    getReasonCheckboxOption() {//remove if not needed
        return     cy.get('input[id^="reason"]', { timeout: 30000 });
    }

    getCheckboxOption() {
        return     cy.get('input[type^="checkbox"]', { timeout: 30000 });
    }

    getCheckboxOptionlabel() {
        return     cy.get('.govuk-label.govuk-checkboxes__label', { timeout: 30000 });
    }

    getCheckboxOptionlabelBACK() { //remove if not needed
        return     cy.get('label[for^="reason"]', { timeout: 30000 });
    }

    getUCAddCaseActionBtn() {
        return     cy.get('[id="add-nti-uc-button"]', { timeout: 30000 });
    }

    getWLAddCaseActionBtn() {
        return     cy.get('[id="add-nti-wl-button"]', { timeout: 30000 });
    }

    getAddConditionsBtn() {
        return     cy.get('[id="add-nti-conditions-wl-button"]', { timeout: 30000 });
    }

    getEditInformationBtn() {
        return cy.get('[id="edit-nti-uc-button"]', { timeout: 30000 });
    }

    getEditWLInformationBtn() {
        return cy.get('[id*="edit-nti"]', { timeout: 30000 });
    }

    getNtiTableRow() {
        return caseActionsBase.getTableRow();
    }

    getNotesBox() {
        return    cy.get('[id="nti-notes"]', { timeout: 30000 });
    }
    
    getNotesInfo() {
        return    cy.get('[id="nti-notes-info"]', { timeout: 30000 });
    }

    getCloseNtiLink() {
        return    cy.get('a[href*="/close"]', { timeout: 30000 });
    }

    //Closed NTI Page
    getStatusRadio() {
        return caseActionsBase.getStatusRadio();
    }

    getStatusRadioLabel() {
        return caseActionsBase.getStatusRadioLabel();
    }

    //NTI WL Conditions Page
    getAddConditionsBtn() {
        return cy.get('[value="add-conditions"]', { timeout: 30000 });
    }

    getUpdateConditionsBtn() {
        return cy.get('[id="add-nti-conditions-wl-button"]', { timeout: 30000 });
    }

    getConditionCheckboxOption() {//remove if not needed
        return     cy.get('input[id^="condition"]', { timeout: 30000 });
    }

    getCancelConditionsBtn() {
        return     cy.get('[id="cancel-link-event"]', { timeout: 30000 });
    }


    


    //Methods

    reasonSelect() {
		let rand = Math.floor(Math.random()*9)

        this.getCheckboxOption().eq(rand-1).click();
        cy.log(this.getCheckboxOptionlabel().eq(rand-1).invoke('text'));

        return this.getCheckboxOptionlabel().eq(rand-1).invoke('text');
	}


    setReasonSelect(value) {
        cy.log("value "+value)

        if(value == "random"){
            let rand = Math.floor(Math.random()*9)
            this.getCheckboxOption().eq(rand-1).click();
            cy.log(this.getCheckboxOptionlabel().eq(rand-1).invoke('text'));
            return this.getCheckboxOptionlabel().eq(rand-1).invoke('text');

        }else{
            this.getCheckboxOption().eq(value).click();
            cy.log(this.getCheckboxOptionlabel().eq(value).invoke('text'));
            return this.getCheckboxOptionlabel().eq(value).invoke('text');
        }
    }

    setConditionSelect(value) {
        cy.log("value "+value)

            this.getCheckboxOption().its('length').as("len").then(() =>{
				cy.log("logging len  "+self.len)
			});

        if(value == "random"){
            let rand = Math.floor(Math.random()*6)
            this.getCheckboxOption().eq(rand-1).click();
            cy.log(this.getCheckboxOptionlabel().eq(rand-1).invoke('text'));
            return this.getCheckboxOptionlabel().eq(rand-1).invoke('text');

        }else{
            this.getCheckboxOption().eq(value).click();
            cy.log(this.getCheckboxOptionlabel().eq(value).invoke('text'));
            return this.getCheckboxOptionlabel().eq(value).invoke('text');
        }
    }

    setNotes() {
        let date = new Date();

        this.getNotesBox().clear();
        this.getNotesBox().type('Cypress test run '+date).invoke('val')
        .then((value) => {
            return value ;
            });
    }


    setCloseStatus(value) {
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

    setConditionsSelect(value) {
        cy.log("value "+value)

        if(value == "random"){
            let rand = Math.floor(Math.random()*9)
            this.getCheckboxOption().eq(rand-1).click();
            cy.log(this.getCheckboxOptionlabel().eq(rand-1).invoke('text'));
            return this.getCheckboxOptionlabel().eq(rand-1).invoke('text');

        }else{
            this.getCheckboxOption().eq(value).click();
            cy.log(this.getCheckboxOptionlabel().eq(value).invoke('text'));
            return this.getCheckboxOptionlabel().eq(value).invoke('text');
        }
    }
}

    export default new NTIAddPage();
