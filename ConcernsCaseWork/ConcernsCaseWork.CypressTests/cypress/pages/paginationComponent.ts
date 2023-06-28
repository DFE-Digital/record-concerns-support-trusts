import { Logger } from "cypress/common/logger";

export class PaginationComponent
{
    public next(): this
    {
        Logger.Log("Nagivating to the next page");

        cy.getByTestId("next-page").click();

        return this;
    }

    public previous(): this
    {
        Logger.Log("Navigating to the previous page");

        cy.getByTestId("previous-page").click();

        return this;
    }

    public goToPage(pageNumber: string)
    {
        Logger.Log(`Moving to page ${pageNumber}`);

        cy.getByTestId(`page-${pageNumber}`).click();

        return this;
    }
}

const paginationComponent = new PaginationComponent();

export default paginationComponent;