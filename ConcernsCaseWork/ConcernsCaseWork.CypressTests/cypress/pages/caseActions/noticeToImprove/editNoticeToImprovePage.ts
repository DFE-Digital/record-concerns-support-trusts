import { Logger } from "../../../common/logger";

export class EditNoticeToImprovePage {

    public withStatus(value: string): this {
        Logger.Log(`With status ${value}`);

        cy.getByTestId(`status-${value}`).check();

        return this;
    }

    public withDayIssued(value: string): this {
        Logger.Log(`With day issued ${value}`);

        cy.getById('dtr-day').clear().type(value);

        return this;
    }

    public withMonthIssued(value: string): this {
        Logger.Log(`With month issued ${value}`);

        cy.getById('dtr-month').clear().type(value);

        return this;
    }

    public withYearIssued(value: string): this {
        Logger.Log(`With year issued ${value}`);

        cy.getById('dtr-year').clear().type(value);

        return this;
    }

    public withReasonIssued(value: string): this {
        Logger.Log(`With reason issued ${value}`);

        cy.getByTestId(`reason-${value}`).check();

        return this;
    }

    public withNotes(value: string): this {
        Logger.Log(`With notes ${value}`);

        cy.getById('nti-notes').clear().type(value);

        return this;
    }

    public withNotesExceedingLimit(): this {
        Logger.Log(`With notes exceeding limit`);

        cy.getById('nti-notes').clear().invoke("val", "x 1".repeat(1001));

        return this;
    }

    public withFinancialManagementConditions(value: string): this {
        Logger.Log(`With financial management conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withGovernanceConditions(value: string): this {
        Logger.Log(`With governance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withComplianceConditions(value: string): this {
        Logger.Log(`With compliance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withSafeguardingConditions(value: string): this {
        Logger.Log(`With safeguarding conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withFraudAndIrregularity(value: string): this {
        Logger.Log(`With fraud and irregularity ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withStandardConditions(value: string): this {
        Logger.Log(`With standard conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public withAdditionalFinancialSupportConditions(value: string): this {
        Logger.Log(`With additional Financial Support conditions ${value}`);

        cy.getByTestId(`condition-${value}`).check();

        return this;
    }

    public hasStatus(value: string): this {
        Logger.Log(`Has status ${value}`);

        cy.getByTestId(`status-${value}`).should("be.checked");

        return this;
    }

    public hasDayIssued(value: string): this {
        Logger.Log(`Has day issued ${value}`);

        cy.getById('dtr-day').should("have.value", value);

        return this;
    }

    public hasMonthIssued(value: string): this {
        Logger.Log(`Has month issued ${value}`);

        cy.getById('dtr-month').should("have.value", value);

        return this;
    }

    public hasYearIssued(value: string): this {
        Logger.Log(`Has year issued ${value}`);

        cy.getById('dtr-year').should("have.value", value);

        return this;
    }

    public hasReasonIssued(value: string): this {
        Logger.Log(`Has reason issued ${value}`);

        cy.getByTestId(`reason-${value}`).should("be.checked");

        return this;
    }

    public hasNotes(value: string): this {
        Logger.Log(`Has notes ${value}`);

        cy.getById('nti-notes').should("have.value", value);

        return this;
    }

    public hasFinancialManagementConditions(value: string): this {
        Logger.Log(`Has financial management conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasGovernanceConditions(value: string): this {
        Logger.Log(`Has governance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasComplianceConditions(value: string): this {
        Logger.Log(`Has compliance conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasSafeguardingConditions(value: string): this {
        Logger.Log(`Has safeguarding conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasFraudAndIrregularity(value: string): this {
        Logger.Log(`Has fraud and irregularity ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasStandardConditions(value: string): this {
        Logger.Log(`Has standard conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasAdditionalFinancialSupportConditions(value: string): this {
        Logger.Log(`Has additional Financial Support conditions ${value}`);

        cy.getByTestId(`condition-${value}`).should("be.checked");

        return this;
    }

    public hasValidationError(value: string): this {
        Logger.Log(`Has validation error ${value}`);

        cy.getById("errorSummary").should("contain.text", value);

        return this;

    }

    public clearDateFields(): this {
        Logger.Log(`Clearing date fields`);

        cy.getById('dtr-day').clear();
        cy.getById('dtr-month').clear();
        cy.getById('dtr-year').clear();

        return this;
    }

    public clearReasonFields(): this
    {
        Logger.Log("Clearing the reason fields");

        cy.get("[name='reason'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public cancelConditions()
    {
        Logger.Log("Cancelling conditions");

        cy.getById("cancel-link-event").click();

        return this;
    }

    public clearConditions(): this
    {
        Logger.Log("Clearing the conditions fields");

        cy.get("[name='condition'").each(element =>
        {
            cy.wrap(element).uncheck();
        });

        return this;
    }

    public save(): this {
        Logger.Log("Saving Notice To Improve");

        cy.getById("add-nti-wl-button").click();

        return this;
    }

    public saveConditions(): this {
        Logger.Log("Saving conditions");

        cy.getById("add-nti-conditions-wl-button").click();

        return this;
    }

    public editConditions() {
        Logger.Log("Editing the conditions");

        cy.getByTestId("edit-conditions-button").click();

        return this;
    }
}