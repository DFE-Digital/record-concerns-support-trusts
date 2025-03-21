import { Logger } from "cypress/common/logger";

class ValidationComponent
{
    public hasValidationError(message: string): this {
		Logger.log(`Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}

    public hasValidationErrorsInOrder(errors: string[])
    {
        Logger.log(`Has validation errors in order ${errors.join(",")}`);

        cy.getById("errorSummary")
            .find(".govuk-error-message")
            .should("have.length", errors.length)
            .each(($elem, index) => {
                expect($elem.text().trim()).to.equal(errors[index]);
            });

        return this;
    }
}

const validationComponent = new ValidationComponent();
export default validationComponent;
