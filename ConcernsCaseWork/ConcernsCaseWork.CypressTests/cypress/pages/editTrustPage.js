class EditTrustPage {

    
    getEditTerritory() {
        return cy.get('[data-testid="edit_Button_SFSO"]').click();
    }

      getTerritoryNAUNorthEastOption() {
        return cy.get('#territory-North_And_Utc__North_East').click();
    }
    getTerritoryApplyBtn() {
        return cy.get('[data-testid="apply_territory_btn"]').click();
    }
    validateTerritoryOldSelection()
    {
      return cy.get('[data-testid="territory_Field"]').should("contain.text", "Midlands and West - South West");
    }
  validateTerritoryNewSelection()
  {
    return cy.get('[data-testid="territory_Field"]').should("contain.text", "North and UTC - North East");
  }
}

    
    export default new EditTrustPage();