import { Logger } from "../../../common/logger";

export class EditTargetedTrustEngagementPage
{

	public withDateEngagementDay(dateEngagementStartDay: string): this {
		Logger.log(`With Date Engagement start Day ${dateEngagementStartDay}`);

		cy.getById("dtr-day-engagement-start").clear().type(dateEngagementStartDay);

		return this;
	}

	public hasDateEngagementStartDay(dateEngagementStartDay: string): this {
		Logger.log(`Has Date Engagement start Day ${dateEngagementStartDay}`);

		cy.getById("dtr-day-engagement-start").should("have.value", dateEngagementStartDay);

		return this;
	}

	public withDateEngagementMonth(dateEngagementMonth: string): this {
		Logger.log(`With Date Engagement Month ${dateEngagementMonth}`);

		cy.getById("dtr-month-engagement-start").clear().type(dateEngagementMonth);

		return this;
	}

	public hasDateEngagementStartMonth(dateEngagementStartMonth: string): this {
		Logger.log(`Has Date Engagement Month ${dateEngagementStartMonth}`);

		cy.getById("dtr-month-engagement-start").should("have.value", dateEngagementStartMonth);

		return this;
	}

	public withDateEngagementYear(dateEngagementStartYear: string): this {
		Logger.log(`With Date Engagement Year ${dateEngagementStartYear}`);

		const element = cy.getById("dtr-year-engagement-start");
		element.clear();

		if (dateEngagementStartYear.length > 0) {
			element.type(dateEngagementStartYear);
		}

		return this;
	}

	public hasDateEngagementStartYear(dateESFAYear: string): this {
		Logger.log(`Has Date Engagement Year ${dateESFAYear}`);

		cy.getById("dtr-year-engagement-start").should("have.value", dateESFAYear);

		return this;
	}


	public withNotesExceedingLimit(): this {
		cy.task("log", `With Notes exceeding limit`);

		cy.getById("case-tte-notes").clear().invoke("val", "x 1".repeat(1001));

		return this;
	}

	public withNotes(notes: string): this {
		Logger.log(`With Notes ${notes}`);

		cy.getById("case-tte-notes").clear().type(notes);

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





    // public withCrmEnquiry(crmNumber: string): this {
	// 	Logger.log(`With Crm enquiry ${crmNumber}`);

	// 	cy.getById("crm-enquiry-number").clear().type(crmNumber);

	// 	return this;
	// }

	// public hasCrmEnquiry(crmNumber: string): this {
	// 	Logger.log(`Has Crm enquiry ${crmNumber}`);

	// 	cy.getById("crm-enquiry-number").should("have.value", crmNumber);

	// 	return this;
	// }

	// public withRetrospectiveRequest(isRetrospectiveRequest: string): this {
	// 	Logger.log(`With retrospective request ${isRetrospectiveRequest}`);

	// 	cy.getByTestId(`retrospective-approval-${isRetrospectiveRequest}`).click();

	// 	return this;
	// }



	// public hasTypeOfDecisionOptions(types: Array<string>)
	// {
	// 	Logger.log(`Has type of decision options ${types.join()}`);

	// 	cy
	// 		.getByTestId('container-decision-types')
	// 		.find('.govuk-checkboxes__label')
	// 		.should("have.length", types.length)
	// 		.each(($elem, index) => {
	// 			expect($elem.text().trim()).to.contain(types[index]);
	// 		});

	// 	return this;
	// }

	// public withDrawdownFacilityAgreed(type: string, value: string): this {
	// 	Logger.log(`With ${type} drawdown facility agreed ${value}`);

	// 	cy.getByTestId(`${type}-${value}`).click();

	// 	return this;
	// }

	// public hasDrawdownFacilityAgreed(type: string, value: string): this {
	// 	Logger.log(`Has ${type} drawdown facility agreed ${value}`);
		
	// 	cy.getByTestId(`${type}-${value}`).should("be.checked");

	// 	return this;
	// }

	// public withFrameworkCategory(type: string, value): this {
	// 	Logger.log(`With ${type} framework category ${value}`);

	// 	cy.getByTestId(`${type}-${value}`).click();

	// 	return this;
	// }

	// public hasFrameworkCategory(type: string, value): this {
	// 	Logger.log(`Has ${type} framework category ${value}`);

	// 	cy.getByTestId(`${type}-${value}`).should("be.checked");

	// 	return this;
	// }

	// public hasNoEnabledOrSelectedSubQuestions(type: string): this {
	// 	Logger.log(`${type} sub questions are not enabled`);

	// 	var elements = cy.getByTestId(`${type}-subquestion-container`).find('input[type="radio"]');
		
	// 	elements.should("not.be.enabled");
	// 	elements.should("not.be.checked");
		
	// 	return this;
	// }


	

	public save(): this {

		Logger.log("Saving decision");

		cy.getById('add-tte-button').click();

		return this;
	}

	public cancel(): this {
		Logger.log("Cancelling edit decision");

		cy.getById("cancel-link-event").click();

		return this;
	}

	public hasValidationError(message: string): this {
		cy.task("log", `Has Validation error ${message}`);

		cy.getById("errorSummary").should(
			"contain.text",
			message
		);

		return this;
	}
}