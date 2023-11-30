import { Logger } from "../../common/logger";

export default class CreateConcernPage {
	public withConcernType(value: string): this {
		Logger.log(`With concernType ${value}`);

		cy.getByTestId(value).click();

		return this;
	}

	public withConcernRating(value: string): this {
		Logger.log(`With concern Rating ${value}`);

		cy.getByTestId(value).click();

		return this;
	}

	public withMeansOfReferral(value: string): this {
		Logger.log(`With Means of referral ${value}`);

		cy.getByTestId(value).click();

		return this;
	}

	public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should("contain.text", message);

		return this;
	}

	public addConcern(): this {
		Logger.log("Click add concern button");
		cy.getByTestId("add-concern-button").click();

		return this;
	}

	public addAnotherConcern(): this {
		Logger.log("Adding another concern");

		cy.getByTestId("add-concern-button").click();

		return this;
	}

	public nextStep(): this {
		Logger.log("Click next step button");
		cy.getByTestId("next-step-button").click();

		return this;
	}

	public cancel(): this {
		Logger.log("Click cancel button");
		cy.getByTestId("cancel-button").click();

		return this;
	}
}
