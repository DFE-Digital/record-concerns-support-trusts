import { Logger } from "../../../common/logger";

export class CloseTargetedTrustEngagementPage
{
    public withFinaliseSupportingNotes(finaliseSupportingNotes: string): this {
        Logger.log(`With Supporting Notes ${finaliseSupportingNotes}`);

        cy.getByTestId("SupportingNotes").clear();
        cy.getByTestId("SupportingNotes").type(finaliseSupportingNotes);

        return this;
    }

    public hasFinaliseSupportingNotes(finaliseSupportingNotes: string): this {
        Logger.log(`With Supporting Notes ${finaliseSupportingNotes}`);

        cy.getByTestId("SupportingNotes").should("have.value", finaliseSupportingNotes);

        return this;
    }

    public withDateEngagementEndDay(dateEngagementEndtDay: string): this {
        Logger.log(`With Date Engagement end Day ${dateEngagementEndtDay}`);

        cy.getById("dtr-day-engagement-end").clear();
        cy.getById("dtr-day-engagement-end").type(dateEngagementEndtDay);

        return this;
    }

    public hasDateEngagementEndDay(dateEngagementEndtDay: string): this {
        Logger.log(`Has Date Engagement end Day ${dateEngagementEndtDay}`);

        cy.getById("dtr-day-engagement-end").should("have.value", dateEngagementEndtDay);

        return this;
    }

    public withDateEngagementEndMonth(dateEngagementEndMonth: string): this {
        Logger.log(`With Date Engagement Month ${dateEngagementEndMonth}`);

        cy.getById("dtr-month-engagement-end").clear();
        cy.getById("dtr-month-engagement-end").type(dateEngagementEndMonth);

        return this;
    }

    public hasDateEngagementEndMonth(dateEngagementEndMonth: string): this {
        Logger.log(`Has Date Engagement Month ${dateEngagementEndMonth}`);

        cy.getById("dtr-month-engagement-end").should("have.value", dateEngagementEndMonth);

        return this;
    }

    public withDateEngagementEndYear(dateEngagementEndYear: string): this {
        Logger.log(`With Date Engagement Year ${dateEngagementEndYear}`);

        cy.getById("dtr-year-engagement-end").clear();

        if (dateEngagementEndYear.length > 0) {
            cy.getById("dtr-year-engagement-end").type(dateEngagementEndYear);
        }

        return this;
    }

    public hasDateEngagementStartYear(dateESFAYear: string): this {
        Logger.log(`Has Date Engagement Year ${dateESFAYear}`);

        cy.getById("dtr-year-engagement-start").should("have.value", dateESFAYear);

        return this;
    }

    public withOutcome(outcome: string): this {
        Logger.log(`With outcome ${outcome}`);

        cy.getByTestId(outcome).click();

        return this;
    }

    public hasOutcome(outcome: string): this {
        Logger.log(`With outcome ${outcome}`);

        cy.getByTestId(outcome).should("be.checked");

        return this;
    }

    public closeTTE(): this {
        Logger.log("Confirm closing the tte");

        cy.get('#close-engagement-button').click();

        return this;
    }

    public withSupportingNotesExceedingLimit(): this {
        Logger.log("With supporting notes exceeding the limit");

        cy.getById("SupportingNotes").clear();
        cy.getById("SupportingNotes").invoke("val", "x".repeat(2001));

        return this;
    }

    public hasValidationError(message: string): this {
        Logger.log(`Has Validation error ${message}`);

        cy.get("#errorSummary").should(
            "contain.text",
            message
        );

        return this;
    }
}
