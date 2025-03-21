import { Logger } from "../../../common/logger";

export class EditTargetedTrustEngagementPage
{

    public withDateEngagementDay(dateEngagementStartDay: string): this {
        Logger.log(`With Date Engagement start Day ${dateEngagementStartDay}`);

        cy.getById("dtr-day-engagement-start").clear();
        cy.getById("dtr-day-engagement-start").type(dateEngagementStartDay);

        return this;
    }

    public hasDateEngagementStartDay(dateEngagementStartDay: string): this {
        Logger.log(`Has Date Engagement start Day ${dateEngagementStartDay}`);

        cy.getById("dtr-day-engagement-start").should("have.value", dateEngagementStartDay);

        return this;
    }

    public withDateEngagementMonth(dateEngagementMonth: string): this {
        Logger.log(`With Date Engagement Month ${dateEngagementMonth}`);

        cy.getById("dtr-month-engagement-start").clear();
        cy.getById("dtr-month-engagement-start").type(dateEngagementMonth);

        return this;
    }

    public hasDateEngagementStartMonth(dateEngagementStartMonth: string): this {
        Logger.log(`Has Date Engagement Month ${dateEngagementStartMonth}`);

        cy.getById("dtr-month-engagement-start").should("have.value", dateEngagementStartMonth);

        return this;
    }

    public withDateEngagementYear(dateEngagementStartYear: string): this {
        Logger.log(`With Date Engagement Year ${dateEngagementStartYear}`);

        cy.getById("dtr-year-engagement-start").clear();

        if (dateEngagementStartYear.length > 0) {
            cy.getById("dtr-year-engagement-start").type(dateEngagementStartYear);
        }

        return this;
    }

    public hasDateEngagementStartYear(dateESFAYear: string): this {
        Logger.log(`Has Date Engagement Year ${dateESFAYear}`);

        cy.getById("dtr-year-engagement-start").should("have.value", dateESFAYear);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With Notes exceeding limit`);

        cy.getById("case-tte-notes").clear();
        cy.getById("case-tte-notes").invoke("val", "x 1".repeat(1001));

        return this;
    }

    public withNotes(notes: string): this {
        Logger.log(`With Notes ${notes}`);

        cy.getById("case-tte-notes").clear();
        cy.getById("case-tte-notes").type(notes);

        return this;
    }

    public hasNotes(notes: string): this {
        Logger.log(`Has Notes ${notes}`);

        cy.getById("case-tte-notes").should("have.value", notes);

        return this;
    }

    public withActivity(activity: string): this {
        Logger.log(`With activity ${activity}`);

        cy.getByTestId(activity).click();

        return this;
    }

    public hasActivity(activity: string): this {
        Logger.log(`With activity ${activity}`);

        cy.getByTestId(activity).should("be.checked");

        return this;
    }

    public withActivityType(typeOfActivity: string): this {
        Logger.log(`With activity type ${typeOfActivity}`);

        cy.getByTestId(typeOfActivity).click();

        return this;
    }

    public hasActivityType(typeOfActivity: string): this {
        Logger.log(`With activity type ${typeOfActivity}`);

        cy.getByTestId(typeOfActivity).should("be.checked");

        return this;
    }

    public save(): this {

        Logger.log("Saving TTE");

        cy.getById('add-tte-button').click();

        return this;
    }

    public cancel(): this {
        Logger.log("Cancelling edit TTE");

        cy.getById("cancel-link-event").click();

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
}
