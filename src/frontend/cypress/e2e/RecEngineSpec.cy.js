describe('RecEngine Page', () => {
    beforeEach(() => {
        cy.visit('/recengine'); // replace with the actual path to your RecEngine page
    });

    it('successfully loads', () => {
        cy.url().should('include', '/recengine');
    });

    // Add more tests as needed
    // it('does something', () => {
    //   // Test code here
    // });
});