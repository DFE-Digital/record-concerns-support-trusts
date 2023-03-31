class ClosedCasePage {
    getClosedCase(caseid) {
        return cy.get('a[href="/case/'+caseid+'/closed"]');
       }
}

    
    export default new ClosedCasePage();