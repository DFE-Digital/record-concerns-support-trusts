import { Logger } from "../common/logger";

class EditTerritoryPage {

  editTerritory() {
    Logger.Log("Editing territory");
    return cy.get('[data-testid="edit_Button_SFSO"]').click();
  }

  selectTerritoryNAUNorthEast() {
    Logger.Log(`Selecting territory north and utc north east`);
    return cy.get('#territory-North_And_Utc__North_East').click();
  }

  save() {
    Logger.Log("Saving territory");
    return cy.get('[data-testid="apply_territory_btn"]').click();
  }

  hasTerritory(territory) {
    Logger.Log(`Has territory ${territory}`);
    return cy.get('[data-testid="territory_Field"]').should("contain.text", territory);
  }
}

export default new EditTerritoryPage();