describe('Calendar Test', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638477345117710790","Iat":"638477333117710758","Nbf":null,"Scope":"","UserHash":"TestUser","Claims":{"Role":"Admin"}},"Signature":"lrs50BAplaJBayP3LEUCYscgQjjILbJrBVSOG4V9Jwc"}'


    it('Create a Calendar LLI', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        
        cy.wait(500)
        cy.get('#calendar-link').click()
        cy.wait(1000)
        //#endregion

        //#region Act
        
        cy.get('button.add-lli-btn.add-lli-btn-10').click();
        cy.get('#create-title-input').type('A New LLI Test');
        cy.get('#create-paragraph-input').type('A New LLI Test description');
        

        cy.wait(500)
        cy.get('#create-lli-button').click();
        
        //#endregion

        //#region Assert
        cy.on('window:alert', (text) => {
            // Assert on the text of the alert
            expect(text).to.equal('The LLI is successfully created.');
          });
        //#endregion
        
    }) 
});