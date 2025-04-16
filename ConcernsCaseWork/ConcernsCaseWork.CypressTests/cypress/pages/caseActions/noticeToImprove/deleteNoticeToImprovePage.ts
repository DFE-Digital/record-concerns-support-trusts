import { Logger } from "../../../common/logger";

export class DeleteNoticeToImprovePage {

    public delete(): this {
        Logger.log("Deleting of Notice To Improve");

        cy.getByTestId("delete-nti-button").click();

        return this;
    }
}
