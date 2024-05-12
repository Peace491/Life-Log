describe('Calendar Test', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638477345117710790","Iat":"638477333117710758","Nbf":null,"Scope":"","UserHash":"TestUser","Claims":{"Role":"Admin"}},"Signature":"lrs50BAplaJBayP3LEUCYscgQjjILbJrBVSOG4V9Jwc"}'
    it('Create a Calendar LLI', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.wait(500)
        cy.get('#calendar-link').click()
        cy.wait(1000)
        //#endregion

        //#region Act
        // Type into the note input field
        cy.get('#create-paragraph-input').click();
        cy.get('#create-paragraph-input').type('Hey');
        // Click the submit button
        cy.get('#submit-note-button').click();
        // Sleep for sometime
        cy.wait(1000)
        //#endregion

        //#region Assert
        // Check that the note content is displayed
        cy.get('#create-paragraph-input').should('contain', 'Hey');
        //#endregion

        // Clean up 
        cy.wait(500)
        cy.get('#note-delete-button').click()
        
    })

   

    

    
  });