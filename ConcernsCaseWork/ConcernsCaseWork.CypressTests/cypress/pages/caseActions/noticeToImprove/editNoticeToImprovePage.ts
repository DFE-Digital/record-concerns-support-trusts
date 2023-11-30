import { Logger } from "../../../common/logger";

export class EditNoticeToImprovePage {

    public withStatus(value: string): this {
        Logger.log(`With status ${value}`);

        cy.getByTestId(`status-${value}`).check();

        return this;
    }

    public withDayIssued(value: string): this {
        Logger.log(`With day issued ${value}`);

        cy.getById('dtr-day-date-issued').clear().type(value);

        return this;
    }

    public withMonthIssued(value: string): this {
        Logger.log(`With month issued ${value}`);

        cy.getById('dtr-month-date-issued').clear().type(value);

        return this;
    }

    public withYearIssued(value: string): this {
        Logger.log(`With year issued ${value}`);

        cy.getById('dtr-year-date-issued').clear().type(value);

        return this;
    }

    public withReasonIssued(value: string): this {
        Logger.log(`With reason issued ${value}`);

        cy.getByTestId(`reason-${value}`).check();

        return this;
    }

    public withNotes(value: string): this {
        Logger.log(`With notes ${value}`);

        cy.getById('nti-notes').clear().type(value);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public withFinancialManagementConditions(value: string): this {
        Logger.log(`With financial management conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withGovernanceConditions(value: string): this {
        Logger.log(`With governance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withComplianceConditions(value: string): this {
        Logger.log(`With compliance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withSafeguardingConditions(value: string): this {
        Logger.log(`With safeguarding conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withFraudAndIrregularity(value: string): this {
        Logger.log(`With fraud and irregularity ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withStandardConditions(value: string): this {
        Logger.log(`With standard conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withAdditionalFinancialSupportConditions(value: string): this {
        Logger.log(`With additional Financial Support conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.log(`Has status ${value}`);

        cy.getByTestId(`status-${value}`).should("be.checked");

        return this;
    }

    public hasDayIssued(value: string): this {
        Logger.log(`Has day issued ${value}`);

        cy.getById('dtr-day-date-issued').should("have.value", value);

        return this;
    }

    public hasMonthIssued(value: string): this {
        Logger.log(`Has month issued ${value}`);

        cy.getById('dtr-month-date-issued').should("have.value", value);

        return this;
    }

    public hasYearIssued(value: string): this {
        Logger.log(`Has year issued ${value}`);

        cy.getById('dtr-year-date-issued').should("have.value", value);

        return this;
    }

    public hasReasonIssued(value: string): this {
        Logger.log(`Has reason issued ${value}`);

        cy.getByTestId(`reason-${value}`).should("be.checked");

        return this;
    }

    public hasNotes(value: string): this {
        Logger.log(`Has notes ${value}`);

        cy.getById('nti-notes').should("have.value", value);

        return this;
    }

    public hasFinancialManagementConditions(value: string): this {
        Logger.log(`Has financial management conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasGovernanceConditions(value: string): this {
        Logger.log(`Has governance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasComplianceConditions(value: string): this {
        Logger.log(`Has compliance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasSafeguardingConditions(value: string): this {
        Logger.log(`Has safeguarding conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasFraudAndIrregularity(value: string): this {
        Logger.log(`Has fraud and irregularity ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasStandardConditions(value: string): this {
        Logger.log(`Has standard conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasAdditionalFinancialSupportConditions(value: string): this {
        Logger.log(`Has additional Financial Support conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;

    }

    public clearDateFields(): this {
        Logger.log(`Clearing date fields`);

        cy.getById('dtr-day').clear();
        cy.getById('dtr-month').clear();
        cy.getById('dtr-year').clear();

        return this;
    }

    public clearReasonFields(): this
    {
        Logger.log("Clearing the reason fields");

        cy.get("[name='reason'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public cancelConditions()
    {
        Logger.log("Cancelling conditions");

        cy.getById("cancel-link-event").click();

        return this;
    }

    public clearConditions(): this
    {
        Logger.log("Clearing the conditions fields");

        cy.get("[name='condition'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public save(): this {
        Logger.log("Saving Notice To Improve");

        cy.getById("add-nti-wl-button").click();

        return this;
    }

    public saveConditions(): this {
        Logger.log("Saving conditions");

        cy.getById("add-nti-conditions-wl-button").click();

        return this;
    }

    public editConditions() {
        Logger.log("Editing the conditions");

        cy.getByTestId("edit-conditions-button").click();

        return this;
    }
}