import { Logger } from "cypress/common/logger";

class Header
{
    public goToHome(): this {
        Logger.Log("Go back to home");

        cy.getByTestId("go-to-home").click();

        return this;
    }
}

const headerComponent = new Header();

export default headerComponent;