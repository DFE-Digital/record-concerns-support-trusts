import { Logger } from "../../../common/logger";

export class DecisionOutcomePage {

    public withDecisionOutcomeStatus(decisionOutcomeID: string): this {
        Logger.log(`With decision outcome status ${decisionOutcomeID}`);

        cy.getByTestId(decisionOutcomeID).click();

        return this;
    }

    public withTotalAmountApproved(totalAmountApproved: string): this {
        Logger.log(`With total Amount Approved ${totalAmountApproved}`);

        cy.getByTestId("total-amount-approved").clear();
        cy.getByTestId("total-amount-approved").type(totalAmountApproved);

        return this;
    }

    public hasNoTotalAmountApprovedField(): this {
        Logger.log("Has no total amount field");

        cy.getByTestId("container-total-amount-approved").should("not.exist");

        return this;
    }

    public withDateDecisionMadeDay(dateDecisionMadeDay: string): this {
        Logger.log(`With Decision Made Day ${dateDecisionMadeDay}`);

        cy.getByTestId("dtr-day-decision-made").clear();
        cy.getByTestId("dtr-day-decision-made").type(dateDecisionMadeDay);

        return this;
    }

    public withDateDecisionMadeMonth(dateDecisionMadeMonth: string): this {
        Logger.log(`With Decision Made Month ${dateDecisionMadeMonth}`);

        cy.getByTestId("dtr-month-decision-made").clear();
        cy.getByTestId("dtr-month-decision-made").type(dateDecisionMadeMonth);

        return this;
    }

    public withDateDecisionMadeYear(dateDecisionMadeYear: string): this {
        Logger.log(`With Decision Made Year ${dateDecisionMadeYear}`);

        cy.getByTestId("dtr-year-decision-made").clear();

        if (dateDecisionMadeYear.length > 0) {
            cy.getByTestId("dtr-year-decision-made").type(dateDecisionMadeYear);
        }

        return this;
    }

    public withDecisionTakeEffectDay(dateDecisionTakeEffectDay: string): this {
        Logger.log(`With Decision Take Effect Day ${dateDecisionTakeEffectDay}`);

        cy.getByTestId("dtr-day-take-effect").clear();
        cy.getByTestId("dtr-day-take-effect").type(dateDecisionTakeEffectDay);

        return this;
    }

    public withDecisionTakeEffectMonth(dateDecisionTakeEffectMonth: string): this {
        Logger.log(`With Decision Take Effect Month ${dateDecisionTakeEffectMonth}`);

        cy.getByTestId("dtr-month-take-effect").clear();
        cy.getByTestId("dtr-month-take-effect").type(dateDecisionTakeEffectMonth);

        return this;
    }

    public withDecisionTakeEffectYear(dateDecisionTakeEffectYear: string): this {
        Logger.log(`With Decision Take Effect Year ${dateDecisionTakeEffectYear}`);

        cy.getByTestId("dtr-year-take-effect").clear();

        if (dateDecisionTakeEffectYear.length > 0) {
            cy.getByTestId("dtr-year-take-effect").type(dateDecisionTakeEffectYear);
        }

        return this;
    }

    public withDecisionAuthouriser(authoriserID: string): this {
        Logger.log(`With decision authouriser to pick ${authoriserID}`);

        cy.getByTestId(authoriserID).click();

        return this;
    }

    public withBusinessArea(businessAreaID: string): this {
        Logger.log(`With decision business area consulted to pick ${businessAreaID}`);

        cy.getByTestId(businessAreaID).click();

        return this;
    }

    public hasBusinessAreaOptions(areas: Array<string>): this {
        Logger.log(`Has business area options ${areas.join()}`);

        cy
            .getByTestId('container-business-areas')
            .find('.govuk-checkboxes__label')
            .should("have.length", areas.length)
            .each(($elem, index) => {
                expect($elem.text().trim()).to.equal(areas[index]);
            });

        return this;
    }

    public deselectAllBusinessAreas(): this
    {
        Logger.log("Deselecting all business areas");
        cy.get(".govuk-checkboxes__input").uncheck();

        return this;
    }

    public saveDecisionOutcome(): this {
        cy.get('#add-decision-outcome-button').click();

        return this;
    }

    public hasValidationError(message: string): this {
        Logger.log(`Has Validation error ${message}`);

        cy.getById("errorSummary").should(
            "contain.text",
            message
        );

        return this;
    }

    public hasDecisionOutcomeStatus(decisionOutcomeID: string): this {
        Logger.log(`Has decision outcome status ${decisionOutcomeID}`);

        cy.getByTestId(decisionOutcomeID).should('be.checked');

        return this;
    }

    public hasTotalAmountApproved(totalAmountApproved: string): this {
        Logger.log(`Total amount approved ${totalAmountApproved}`);

        cy.getByTestId("total-amount-approved").should(
            "contain.value",
            totalAmountApproved
        );

        return this;
    }

    public hasDecisionMadeDay(decisionMadeDay: string): this {
        Logger.log(`Decision Made Date ${decisionMadeDay}`);

        cy.getByTestId("dtr-day-decision-made").should(
            "contain.value",
            decisionMadeDay
        );

        return this;
    }

    public hasDecisionMadeMonth(decisionMadeMonth: string): this {
        Logger.log(`Decision Made Date ${decisionMadeMonth}`);

        cy.getByTestId("dtr-month-decision-made").should(
            "contain.value",
            decisionMadeMonth
        );

        return this;
    }

    public hasDecisionMadeYear(decisionMadeYear: string): this {
        Logger.log(`Decision Made Date ${decisionMadeYear}`);

        cy.getByTestId("dtr-year-decision-made").should(
            "contain.value",
            decisionMadeYear
        );

        return this;
    }

    public hasDateEffectiveFromDay(dateEffectiveFromDay: string): this {
        Logger.log(`Date Effective From Day ${dateEffectiveFromDay}`);

        cy.getByTestId("dtr-day-take-effect").should(
            "contain.value",
            dateEffectiveFromDay
        );

        return this;
    }

    public hasDateEffectiveFromMonth(dateEffectiveFromMonth: string): this {
        Logger.log(`Date Effective From Month ${dateEffectiveFromMonth}`);

        cy.getByTestId("dtr-month-take-effect").should(
            "contain.value",
            dateEffectiveFromMonth
        );

        return this;
    }

    public hasDateEffectiveFromYear(dateEffectiveFromYear: string): this {
        Logger.log(`Date Effective From Year ${dateEffectiveFromYear}`);

        cy.getByTestId("dtr-year-take-effect").should(
            "contain.value",
            dateEffectiveFromYear
        );

        return this;
    }

    public hasDecisionAuthouriser(authoriserID: string): this {
        Logger.log(`Has authoriser ${authoriserID}`);

        cy.getByTestId(authoriserID).should('be.checked');

        return this;
    }

    public hasBusinessArea(businessAreaID: string): this {
        Logger.log(`Has Business Area ${businessAreaID}`);

        cy.getByTestId(businessAreaID).should('have.value', businessAreaID);

        return this;
    }
}
