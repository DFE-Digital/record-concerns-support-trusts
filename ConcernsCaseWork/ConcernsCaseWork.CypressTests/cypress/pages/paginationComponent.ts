export class PaginationComponent
{
    public next(): this
    {
        cy.getByTestId("next-page").click();

        return this;
    }

    public previous(): this
    {
        cy.getByTestId("previous-page").click();

        return this;
    }

    public goToPage(pageNumber: string)
    {
        cy.getByTestId(`page-${pageNumber}`).click();

        return this;
    }
}

const paginationComponent = new PaginationComponent();

export default paginationComponent;