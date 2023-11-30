import { Logger } from "cypress/common/logger";

export class PaginationComponent
{
    constructor(private prefix: string = "")
    {

    }

    public next(): this
    {
        Logger.log("Nagivating to the next page");

        cy.getByTestId(`${this.prefix}next-page`).click();

        return this;
    }

    public previous(): this
    {
        Logger.log("Navigating to the previous page");

        cy.getByTestId(`${this.prefix}previous-page`).click();

        return this;
    }

    public goToPage(pageNumber: string)
    {
        Logger.log(`Moving to page ${pageNumber}`);

        cy.getByTestId(`${this.prefix}page-${pageNumber}`).click();

        return this;
    }

    public isCurrentPage(pageNumber: string): this {
        Logger.log(`Currently selected page is page ${pageNumber}`);

        // Used to check that we have navigated to the next page with ajax
        cy.getByTestId(`${this.prefix}page-${pageNumber}`).parent().should("have.class", "govuk-pagination__item--current");

        return this;
    }
}

const paginationComponent = new PaginationComponent();

export default paginationComponent;